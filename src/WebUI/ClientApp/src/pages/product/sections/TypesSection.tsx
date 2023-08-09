import React, { useEffect, useState } from "react"; 
import { Box, Paper, Typography } from "@material-ui/core";
import { useLocation, useParams } from "react-router";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";

import ControlButtons from "../../../components/control_buttons/ControlButtons";
import ProductOrderButtons from "../components/ProductOrderButtons";
import ProductTypes from "../components/ProductTypes";
import ConfirmDialog from "../../../components/dialog/ConfirmDialog";
import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import ProductOrderLabels from "../components/ProductOrderLabels";
import ProductOrderSectionStyle from "./TypesSectionStyle";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";
import { httpDeleteProductType, httpPostProductImage, httpPostProductType, httpPutProductType } from "../../../services/ProductService";
import IProduct from "../../../interfaces/product/IProduct";
import IProductType, { emptyProductType } from "../../../interfaces/product/IProductType";

interface IProps {
    product: IProduct,
    setImage: (image: string) => void;
    isAdmin?: boolean|undefined;
}

export default ({product, setImage, isAdmin}:IProps) => {
    const classes = ProductOrderSectionStyle();
    const dispatch = useDispatch();
    const { id }:any = useParams();
    const { t } = useTranslation();
    const [loading, setLoading] = useState<boolean>(false);
    const [types, setTypes] = useState<IProductType[]>([]);
    const [newType, setNewType] = useState<IProductType>(emptyProductType);
    const [currentType, setCurrentType] = useState<IProductType|undefined>(undefined);
    const [currentLocalFile, setCurrentLocalFile] = useState<Blob|undefined>(undefined);
    const [visableCreate, setVisableCreate] = React.useState(false);
    const [visableEdit, setVisableEdit] = React.useState(false);
    const [visableDelete, setVisableDelete] = React.useState(false);
    
    const search = useLocation().search;
    const type = new URLSearchParams(search).get('type');
    
    // Initialize
    useEffect(() => {
        if(product.types.length > 0 && currentType === undefined)
        {
            let item = product.types[0];
            if(type)
            {
                var value = product.types.find(item => String(item.id) === type);
                if(value) item = value;
            }
            
            setTypes(product.types);
            setCurrentType(item);
            setImage(item.image);
        }
    });

    const onHttpUploadFile = (onResponse: (filename?:string) => void) => {
        if(currentLocalFile !== undefined)
        {
            httpPostProductImage(currentLocalFile, 
                (filename:string) => {
                    if(!currentType) return;
                    setCurrentLocalFile(undefined);
                    onResponse(filename);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message));
                }
            )
        }
        else
        {
            onResponse();
        }
    }

    const onAddType = () => {
        setLoading(true);
        onHttpUploadFile(
            (filename?:string) => {
                var item = filename ? {...newType, image:filename} : newType;
                httpPostProductType({ ...item, productId:id },
                    (_id:number) => {
                        item = { ...item, id:_id };
                        const items = [...types, item];

                        setTypes(items);
                        setNewType(emptyProductType);
                        setCurrentType(item);
                        setImage(item.image);

                        dispatch(setSuccessStatus(`On Success: ${_id}`));
                        setVisableCreate(false);
                        setLoading(false);
                    },
                    (message:string) => {
                        dispatch(setErrorStatus(message));
                        setLoading(false);
                    }
                )
            }
        )
    }

    const onEditType = () => {
        if(!currentType) return;
        setLoading(true);

        onHttpUploadFile(
            (filename?:string) => {
                var item = filename ? {...currentType, image:filename} : currentType;
                httpPutProductType(item,
                    (_id:number) => {
                        const items = types.map(type => type.id === _id ? item : type);
                        
                        setTypes(items);
                        setCurrentType(item);
                        setImage(item.image);
                        setNewType(emptyProductType);

                        dispatch(setSuccessStatus(`On Success: ${id}`));
                        setVisableEdit(false);
                        setLoading(false);
                    },
                    (message:string) => {
                        dispatch(setErrorStatus(message));
                        setLoading(false);
                    }
                )
            }
        )
    }

    const onDeleteType = () => {
        if(!currentType) return;
        setLoading(true);

        const items:IProductType[] = types.filter(
            item =>  item.id !== currentType.id
        );

        httpDeleteProductType(id, currentType.id,
            (_id:number) => {
                let item = items.length > 0 ? items[items.length-1] : emptyProductType;

                setTypes(items);
                setCurrentType(item);
                setImage(item.image);

                dispatch(setSuccessStatus(`On Success: ${id}`));
                setVisableDelete(false);
                setLoading(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
                setLoading(false);
            }
        )
    }

    const uploadImage = (file:any) => {
        var reader = new FileReader();
        setCurrentLocalFile(file);

        // set image
        reader.onload = (event:any) => {
            let img = event.target.result;
            updateImage(img);
        };
        reader.readAsDataURL(file);
    }

    const updateImage = (url:string) => {
        let item = currentType ? currentType : emptyProductType;
        item = {...item, image:url}
        
        const orders = types.filter(
            type => (type.title === item.title) ? item : type
        );
        setTypes(orders);
        setCurrentType(item);
        setImage(item.image);
    }

    if(id === "-1") return <></>
    return <>
        <Box className={classes.root}>
            <ProductOrderLabels/>
            <Paper variant="outlined">
                <Typography variant="h6" className={classes.typography}>
                    {t("product_options")}
                </Typography>
                <ProductTypes
                    currentType={currentType}
                    setCurrentType={setCurrentType}
                    orderTypes={types} 
                    setImage={setImage}
                />
                {isAdmin ? 
                    <ControlButtons
                        onCreate={() => setVisableCreate(true)}
                        onEdit={() => setVisableEdit(true)}
                        onDelete={() => setVisableDelete(true)}
                        isAdmin={isAdmin}
                    />
                    :
                    <ProductOrderButtons currentType={currentType!} product={product}/>
                }
            </Paper>
        </Box>
        <EditKeyValueDialog 
            title={t("create")}
            data={newType}
            setData={setNewType}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading}
            onConfirm={onAddType}
            onAttachFile={uploadImage}
            disableForm
        />
        <EditKeyValueDialog 
            title={t("edit")}
            data={currentType}
            setData={setCurrentType}
            open={visableEdit} 
            setOpen={setVisableEdit} 
            isLoading={loading}
            onConfirm={onEditType}
            onAttachFile={uploadImage}
            disableForm
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading}
            onConfirm={onDeleteType}
        />
    </>
}