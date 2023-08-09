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
import IProductSpecLine, { emptySpecLine } from "../../../interfaces/product/IProductSpecLine";
import { httpPostProduct } from "../../../services/ProductService";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";


const TableRowStyleLeft:CSSProperties = {
    display: "inline-flex", 
    marginLeft: "20px", 
    color: "#545454"
}

const TableRowStyleRight:CSSProperties = {
    display: "inline-flex", 
    marginRight: "20px", 
    color: "#545454"
}

interface IProps {
    className?: string | undefined;
    default_data: IProductSpecLine[];
    isAdmin: boolean|undefined;
    collapsible?: boolean|undefined;
}

export default ({className, default_data, isAdmin, collapsible}:IProps) => {
    const { t } = useTranslation();
    const { id }:any = useParams();
    const history = useHistory();
    const dispatch = useDispatch();
    const [ loading, setLoading ] = useState<boolean>(false);
    const [ specifications, setSpecifications ] = useState<IProductSpecLine[]>(default_data);
    const [ currentSpecification, setCurrentSpecification ] = useState<IProductSpecLine|undefined>(
        default_data.length > 0 ? default_data[0] : undefined
    );
    const [ newSpecification, setNewSpecification ] = useState<IProductSpecLine>(emptySpecLine);
    const [ visableEditSpecifications, setVisableEditSpecifications ] = React.useState(false);
    const [ visableCreateSpecifications, setVisableCreateSpecifications ] = React.useState(false);
    const [ visableConfirm, setVisableConfirm ] = React.useState(false);

    
    const onHttpEditSpecs = (content: IProductSpecLine[], onResponse: () => void) => {
        const data = { specifications:content };
        httpPostProduct(id, data, 
            (id:number, new_item:boolean) => {
                if(new_item) history.push(`/product/${id}`);
                dispatch(setSuccessStatus("On Success"));
                setSpecifications(specifications);
                onResponse();
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        ); 
    };

    const onCreate = () => {
        setLoading(true);

        const data = [...specifications, newSpecification];
        onHttpEditSpecs(data,
            () => {
                setVisableCreateSpecifications(false);
                setCurrentSpecification(data.length > 0 ? data[data.length-1] : undefined);
                setSpecifications(data);
                setNewSpecification(emptySpecLine);
                setLoading(false);
            }
        );
    }
    
    const onEdit = () => {
        if(!currentSpecification) return;
        setLoading(true);

        var data = specifications.map(
            spec => (spec.id == currentSpecification.id) ? currentSpecification : spec
        );
        onHttpEditSpecs(data,
            () => {
                setVisableEditSpecifications(false);
                setCurrentSpecification(currentSpecification);
                setSpecifications(data);
                setLoading(false);
            }
        );
    }
    
    const onDelete = () => {
        if(!currentSpecification) return;
        setLoading(true);

        const data = specifications.filter(
            specification =>  specification.id !== currentSpecification.id
        );


        onHttpEditSpecs(data,
            () => {
                setVisableConfirm(false);
                setCurrentSpecification(data.length > 0 ? data[data.length-1] : undefined);
                setSpecifications(data);
                setNewSpecification(emptySpecLine);
                setLoading(false);
            }
        );
    }

    const onTableIndex = (index:number) => {
        setVisableEditSpecifications(true);
        setCurrentSpecification(specifications[index])
    }

    const getTableData = () => {
        var rows:any[] = [];

        specifications.forEach(spec => {
            if(spec.childOf.length > 0)
            {
                const subject = <div style={TableRowStyleLeft}>
                    <i>{spec.subject}</i>
                </div>;
                const value = <div style={TableRowStyleRight}>
                <i>{spec.value}</i>
            </div>;

                rows.push([subject, value]);
            }
            else
            {
                rows.push([<b>{spec.subject}</b>, spec.value]);
            }
        })

        return rows;
    }

    const tableData:ITableData = {
        header: [t("food_value"), t("per_100_gram")],
        data : getTableData()
    }

    console.log(specifications)

    if(id === "-1") return <></>
    return <Box className={className}>
        {
            collapsible ? 
                <CollabsableTable 
                    data={tableData}
                    onCreate={() => setVisableCreateSpecifications(true)}
                    onEdit={onTableIndex}
                    onDelete={() => setVisableConfirm(true)}
                    isAdmin={isAdmin}
                />
            :
                <PaperTable 
                    data={tableData}
                    onCreate={() => setVisableCreateSpecifications(true)}
                    onEdit={onTableIndex}
                    onDelete={() =>  setVisableConfirm(true)}
                    isAdmin={isAdmin}
                />
        }
        <EditKeyValueDialog 
            title={t("edit")} 
            data={currentSpecification} 
            setData={setCurrentSpecification} 
            open={visableEditSpecifications} 
            setOpen={setVisableEditSpecifications} 
            isLoading={loading}
            onConfirm={onEdit} 
            specifications={default_data}
            noteOnKeys={[
                { key:"value", message:t("per_100_gram") }
            ]}
            disableForm
        />
        <EditKeyValueDialog 
            title={t("create")} 
            data={newSpecification} 
            setData={setNewSpecification} 
            open={visableCreateSpecifications} 
            setOpen={setVisableCreateSpecifications} 
            isLoading={loading}
            onConfirm={onCreate} 
            specifications={default_data}
            noteOnKeys={[
                { key:"value", message:t("per_100_gram") }
            ]}
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