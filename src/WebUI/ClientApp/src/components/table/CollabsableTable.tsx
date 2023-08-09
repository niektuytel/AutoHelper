import React from "react";
import { Accordion, AccordionDetails, AccordionSummary, Typography } from "@material-ui/core";
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import SimpleTable, { ITableProps } from "./SimpleTable";

export default ({ data, onCreate, onEdit, onDelete, isAdmin}:ITableProps) => {
    return <>
        <Accordion>
            <AccordionSummary
                id="panel1a-header"
                expandIcon={<ExpandMoreIcon />}
                aria-controls="panel1a-content"
            >
                <Typography variant="h6" gutterBottom>
                    {data.header[0]}
                </Typography>
            </AccordionSummary>
            <AccordionDetails>
                <SimpleTable
                    data={data}
                    onCreate={onCreate}
                    onEdit={onEdit}
                    onDelete={onDelete}
                    fromAccordion={true}
                    isAdmin={isAdmin}
                />
            </AccordionDetails>
        </Accordion>
    </>
}
