import { Box } from "@material-ui/core";
import { CSSProperties } from "@material-ui/core/styles/withStyles";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useHistory, useParams } from "react-router";

import ConfirmDialog from "../../../components/dialog/ConfirmDialog";
import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import CollabsableTable from "../../../components/table/CollabsableTable";
import PaperTable from "../../../components/table/PaperTable";
import { ITableData } from "../../../components/table/SimpleTable";
import IProductInfoLine, { emptyInfoLine } from "../../../interfaces/product/IProductInfoLine";
import { httpPostProduct } from "../../../services/ProductService";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";


const TableRowStyleLeft:CSSProperties = {
    display: "inline-flex", 
    marginLeft: "20px"
}

const TableRowStyleRight:CSSProperties = {
    display: "inline-flex", 
    marginRight: "20px"
}

interface IProps {
    className?: string | undefined;
    default_data: IProductInfoLine[];
    isAdmin: boolean|undefined;
    collapsible?: boolean|undefined;
}

export default ({className, default_data, isAdmin, collapsible}:IProps) => {
    const { t } = useTranslation();
    const { id }:any = useParams();
    const history = useHistory();
    const dispatch = useDispatch();
    const [ loading, setLoading ] = useState<boolean>(false);
    const [ extraInformation, setExtraInformation ] = useState<IProductInfoLine[]>(default_data);
    const [ currentInfoLine, setCurrentInfoLine ] = useState<IProductInfoLine|undefined>(
        default_data.length > 0 ? default_data[0] : undefined
    );
    const [ newInfoLine, setNewInfoLine ] = useState<IProductInfoLine>(emptyInfoLine);
    const [ visableEditInfoLine, setVisableEditInfoLine ] = React.useState(false);
    const [ visableCreateInfoLine, setVisableCreateInfoLine ] = React.useState(false);
    const [ visableConfirm, setVisableConfirm ] = React.useState(false);

    
    const onHttpEditSpecs = (content: IProductInfoLine[], onResponse: () => void) => {
        const data = { extraInformation:content };
        httpPostProduct(id, data, 
            (id:number, new_item:boolean) => {
                if(new_item) history.push(`/product/${id}`);
                dispatch(setSuccessStatus("On Success"));
                setExtraInformation(extraInformation);
                onResponse();
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        ); 
    };

    const onCreate = () => {
        setLoading(true);

        const data = [...extraInformation, newInfoLine];
        onHttpEditSpecs(data,
            () => {
                setVisableCreateInfoLine(false);
                setCurrentInfoLine(data.length > 0 ? data[data.length-1] : undefined);
                setExtraInformation(data);
                setNewInfoLine(emptyInfoLine);
                setLoading(false);
            }
        );
    }
    
    const onEdit = () => {
        if(!currentInfoLine) return;
        setLoading(true);

        var data = extraInformation.map(
            spec => (spec.id == currentInfoLine.id) ? currentInfoLine : spec
        );
        onHttpEditSpecs(data,
            () => {
                setVisableEditInfoLine(false);
                setCurrentInfoLine(currentInfoLine);
                setExtraInformation(data);
                setLoading(false);
            }
        );
    }
    
    const onDelete = () => {
        if(!currentInfoLine) return;
        setLoading(true);

        const data = extraInformation.filter(
            specification =>  specification.id !== currentInfoLine.id
        );


        onHttpEditSpecs(data,
            () => {
                setVisableConfirm(false);
                setCurrentInfoLine(data.length > 0 ? data[data.length-1] : undefined);
                setExtraInformation(data);
                setNewInfoLine(emptyInfoLine);
                setLoading(false);
            }
        );
    }

    const onTableIndex = (index:number) => {
        setVisableEditInfoLine(true);
        setCurrentInfoLine(extraInformation[index])
    }

    const getTableData = () => {
        var rows:any[] = [];

        extraInformation.forEach(spec => {
            const subject = <div style={TableRowStyleLeft}>
                <b>{spec.subject}</b>
            </div>;
            const value = <div style={TableRowStyleRight}>
                {spec.value}
            </div>;

            rows.push([subject, value]);
           
        })

        return rows;
    }
    const tableData:ITableData = {
        header: [t("extra_information"), ""],
        data : getTableData()
    }

    if(id === "-1") return <></>
    return <Box className={className}>
        {
            collapsible ? 
                <CollabsableTable 
                    data={tableData}
                    onCreate={() => setVisableCreateInfoLine(true)}
                    onEdit={onTableIndex}
                    onDelete={() => setVisableConfirm(true)}
                    isAdmin={isAdmin}
                />
            :
                <PaperTable 
                    data={tableData}
                    onCreate={() => setVisableCreateInfoLine(true)}
                    onEdit={onTableIndex}
                    onDelete={() =>  setVisableConfirm(true)}
                    isAdmin={isAdmin}
                />
        }
        <EditKeyValueDialog 
            title={t("edit")} 
            data={currentInfoLine} 
            setData={setCurrentInfoLine} 
            open={visableEditInfoLine} 
            setOpen={setVisableEditInfoLine} 
            isLoading={loading}
            onConfirm={onEdit}
            disableForm
        />
        <EditKeyValueDialog 
            title={t("create")} 
            data={newInfoLine} 
            setData={setNewInfoLine} 
            open={visableCreateInfoLine} 
            setOpen={setVisableCreateInfoLine} 
            isLoading={loading}
            onConfirm={onCreate} 
            disableForm
        />
        <ConfirmDialog 
            open={visableConfirm} 
            setOpen={setVisableConfirm} 
            isLoading={loading}
            onConfirm={onDelete} 
        />
    </Box>
}