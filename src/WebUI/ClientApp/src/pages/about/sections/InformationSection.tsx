import { Box, Grid, Typography } from "@material-ui/core";
import React, { useEffect, useState } from "react"
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import ControlButtons from "../../../components/control_buttons/ControlButtons";
import ConfirmDialog from "../../../components/dialog/ConfirmDialog";
import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import DisplayImage from "../../../components/display/DisplayImage";
import ProgressBox from "../../../components/progress/ProgressBox";
import IInformationItem, { emptyNewsItem } from "../../../interfaces/IInformationItem";
import { httpDeleteInformationItem, httpGetInformationItems, httpPostInformationItem, httpPostInformationItemImage, httpPutInformationItem } from "../../../services/InformationService"
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";

const getCurrentDate = (value='') => {
    let dateTime = new Date(value);
    let hours = dateTime.getHours();
    let minutes = dateTime.getMinutes();
    let seconds = dateTime.getSeconds();
    let date = dateTime.getDate();
    let month = dateTime.getMonth();
    let year = dateTime.getFullYear();
    
    return `${date}-${month}-${year}T${hours < 10 ? `0${hours}` : `${hours}`}:${minutes < 10 ? `0${minutes}` : `${minutes}`}:${seconds < 10 ? `0${seconds}` : `${seconds}`}`
}

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const [pageIndex, setPageIndex] = useState(0);
    const [newItem, setNewItem] = useState<IInformationItem>(emptyNewsItem);
    const [currentItem, setCurrentItem] = useState<IInformationItem>(emptyNewsItem);
    const [items, setItems] = useState<IInformationItem[]|undefined>(undefined);
    const [loading, setLoading] = useState<boolean|undefined>(undefined);
    const [loadingDialog, setLoadingDialog] = useState<boolean>(false);
    const [visableCreate, setVisableCreate] = useState<boolean>(false);
    const [visableUpdate, setVisableUpdate] = useState<boolean>(false);
    const [visableDelete, setVisableDelete] = useState<boolean>(false);
    const [currentLocalFile, setCurrentLocalFile] = useState<Blob|undefined>(undefined);

    useEffect(() => {
        if(loading === undefined && !items)
        {
            setLoading(true);
            httpGetInformationItems(pageIndex,
                (data) => {
                    console.log(data);

                    setItems(data.items);
                    setLoading(false);
                },
                (message:string) => {
                    console.log(message);

                    setLoading(false);
                }
            );
        }
    });
    
    const onHttpUploadFile = (onResponse: (filename?:string) => void) => {
        if(currentLocalFile !== undefined)
        {
            httpPostInformationItemImage(currentLocalFile, 
                (filename:string) => {
                    onResponse(filename);
                    setCurrentLocalFile(undefined);
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
    
    const onCreate = () => {
        if(!items) return;
        setLoadingDialog(true);

        onHttpUploadFile(
            (filename?:string) => {
                var item = filename ? {...newItem, image:filename} : newItem;
                httpPostInformationItem(item,
                    (_id:number) => {
                        let now = new Date().toDateString();
                        item = { ...item, id:_id, createdAt:now };
                        const data = [item, ...items];
                        setItems(data);
                        setNewItem(emptyNewsItem);

                        setVisableCreate(false);
                        setLoadingDialog(false);
                        dispatch(setSuccessStatus(`On Success: ${_id}`));
                    },
                    (message:string) => {
                        setLoadingDialog(false);
                        dispatch(setErrorStatus(message));
                    }
                )
            }
        )
    }

    const onUpdate = () => {
        if(!items) return;
        setLoadingDialog(true);

        onHttpUploadFile(
            (filename?:string) => {
                let item:IInformationItem = filename ? {...currentItem, image:filename} : currentItem;
                httpPutInformationItem(item,
                    (_id:number) => {
                        item = { ...item, id:_id };
                        const data = items.map(elem => (elem.id === item.id) ? item : elem);
                        
                        setItems(data);
                        setCurrentItem(emptyNewsItem);

                        setVisableUpdate(false);
                        setLoadingDialog(false);
                        dispatch(setSuccessStatus(`On Success: ${_id}`));
                    },
                    (message:string) => {
                        setLoadingDialog(false);
                        dispatch(setErrorStatus(message));
                    }
                )
            }
        )
    }

    const onDelete = () => {
        if(!items) return;
        setLoadingDialog(true);

        httpDeleteInformationItem(currentItem.id,
            (_id:number) => {
                const data = items.filter(elem => (elem.id !== currentItem.id));
                
                setItems(data);
                setCurrentItem(emptyNewsItem);

                setVisableDelete(false);
                setLoadingDialog(false);
                dispatch(setSuccessStatus(`On Success: ${_id}`));
            },
            (message:string) => {
                setLoadingDialog(false);
                dispatch(setErrorStatus(message));
            }
        );
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
        let item = newItem
        item = {...item, image:url};
        setNewItem(item);
    }
    
    const visualizeUpdateItem = (index:number) => {
        if(!items) return;
        setCurrentItem(items[index]);
        setVisableUpdate(true);
    }

    const visualizeDeleteItem = (index:number) => {
        if(!items) return;
        setCurrentItem(items[index]);
        setVisableDelete(true);
    }

    return <>
        <Box style={{backgroundColor:"white"}}>
            {loading || !items ? 
                <Box style={{marginBottom:"1000px"}}>
                    <ProgressBox/>
                </Box>
                :
                <Box>
                     {/* style={{marginTop:"8px"}}> */}
                    <ControlButtons 
                        onCreate={() => setVisableCreate(true)} 
                        isAdmin={isAdmin}
                    />
                    {items.map((item:IInformationItem, index:number) => 
                        <>
                            <Box style={{ backgroundColor:(index % 2 === 0) ? "#F3F3F3" : "#FFFFFF", padding:"10px"}}>
                                <ControlButtons 
                                    onEdit={() => visualizeUpdateItem(index)} 
                                    onDelete={() => visualizeDeleteItem(index)} 
                                    isAdmin={isAdmin}
                                />
                                <Grid container>
                                    <Grid item xs={9} style={{textAlign:"left"}}>
                                        <Typography variant="h5">{item.title}</Typography>
                                    </Grid>
                                    <Grid item xs={3} style={{textAlign:"right", color:"gray"}}>
                                        <Typography variant="caption">{getCurrentDate(item.createdAt)}</Typography>
                                    </Grid>
                                </Grid>
                                <Typography variant="body1" style={{textAlign:"left", marginBottom:"10px"}}>
                                    {item.content}
                                </Typography>
                                { item.image && <DisplayImage image={item.image} dependOnHeight/> }
                            </Box>
                        </>
                    )}
                </Box>

            }
        </Box>
        <EditKeyValueDialog 
            title={t("create")}
            data={newItem}
            setData={(data:any) => setNewItem(data)}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loadingDialog}
            onConfirm={onCreate}
            onAttachFile={uploadImage}
            multilines={[
                "content"
            ]}
            ignoredKeys={[
                "createdAt"
            ]}
        />
        <EditKeyValueDialog 
            title={t("update")}
            data={currentItem}
            setData={setCurrentItem}
            open={visableUpdate} 
            setOpen={setVisableUpdate} 
            isLoading={loadingDialog}
            onConfirm={onUpdate}
            onAttachFile={uploadImage}
            multilines={[ "content" ]}
            ignoredKeys={[ "createdAt" ]}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loadingDialog}
            onConfirm={onDelete}
        />
    </>
}