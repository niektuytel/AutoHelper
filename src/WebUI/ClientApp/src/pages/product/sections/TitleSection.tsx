import React, { useState } from "react";
import { useHistory, useParams } from "react-router";
import { useDispatch } from "react-redux";
import { useTranslation } from "react-i18next";
import { Typography } from "@material-ui/core";

import ControlButtons from "../../../components/control_buttons/ControlButtons";
import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import { httpPostProduct } from "../../../services/ProductService";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";

interface IProps {
    default_data: string;
    isAdmin: boolean|undefined;
}

export default ({default_data, isAdmin}:IProps) => {
    const history = useHistory();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const { id }:any = useParams();
    const [ loading, setLoading ] = useState<boolean>(false);
    const [ title, setTitle ] = useState<string>(default_data);
    const [visableEditTitle, setVisableEditTitle] = React.useState(false);

    const onHttpEditProductTitle = (title: string, onResponse: () => void) => {
        httpPostProduct(id, {title:title}, 
            (id:number, new_item:boolean) => {
                if(new_item) history.push(`/product/${id}`);
                dispatch(setSuccessStatus("On Success"));
                onResponse();
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        ); 
    }
    
    const onEditTitle = () => {
        setLoading(true);
        onHttpEditProductTitle(title,
            () => {
                setVisableEditTitle(false);
                setLoading(false);
            }
        );
    }
    
    return <>
        <Typography variant="h4" gutterBottom>
            {title} 
        </Typography>
        <ControlButtons 
            onEdit={() => setVisableEditTitle(true)} 
            isAdmin={isAdmin}
            containStyle={false}
        />
        <EditKeyValueDialog 
            title={t("edit")}
            data={{ title: title }}
            setData={(data:any) => setTitle(data.title)}
            open={visableEditTitle} 
            setOpen={setVisableEditTitle} 
            isLoading={loading}
            onConfirm={onEditTitle}
            disableForm
        />
    </>
}