import React from "react";
import { Paper, TableContainer } from "@material-ui/core";

import SimpleTable, { ITableProps } from "./SimpleTable";

export default ({data, onCreate, onEdit, onDelete, isAdmin}:ITableProps) => {
    return <>
        <TableContainer component={Paper}>
            <SimpleTable
                data={data}
                onCreate={onCreate}
                onEdit={onEdit}
                onDelete={onDelete}
                isAdmin={isAdmin}
            />
        </TableContainer>
    </>
}
