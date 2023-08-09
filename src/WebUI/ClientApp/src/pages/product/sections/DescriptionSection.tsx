import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useHistory, useParams } from "react-router";

import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import LargeText from "../../../components/textfields/LargeText";
import IProductDescription from "../../../interfaces/product/IProductDescription";
import { httpPostProduct } from "../../../services/ProductService";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";

interface IProps {
    default_data: string;
    isAdmin: boolean|undefined;
}

export default 
({default_data, isAdmin}:IProps) => {
    const { t } = useTranslation();
    const { id }:any = useParams();
    const history = useHistory();
    const dispatch = useDispatch();

    const [loading, setLoading] = useState<boolean>(false);
    const [description, setDescription] = useState<string>(default_data);
    const [visableEditDescription, setVisableEditDescription] = React.useState(false);
    
    const onHttpEditOrderTypes = (data: IProductDescription, onResponse: () => void) => {
        httpPostProduct(id, data, 
            (id:number, new_item:boolean) => {
                if(new_item) history.push(`/product/${id}`);
                onResponse();
                dispatch(setSuccessStatus("On Success"));
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        ); 
    }
    
    const onEditDescription = () => {
        if(!description) return;
        setLoading(true);
        
        onHttpEditOrderTypes({description:description},
            () => {
                setVisableEditDescription(false);
                setLoading(false);
            }
        );
    }

    return <>
        <LargeText 
            title={t("description")} 
            text={description}
            onEdit={() => setVisableEditDescription(true)}
            isAdmin={isAdmin}
        />
        <EditKeyValueDialog 
            title={t("edit")}
            data={{ description: description }}
            setData={(data:any) => setDescription(data.description)}
            open={visableEditDescription} 
            setOpen={setVisableEditDescription} 
            isLoading={loading}
            onConfirm={onEditDescription}
            multilines={["description"]}
            disableForm
        />
    </>
}