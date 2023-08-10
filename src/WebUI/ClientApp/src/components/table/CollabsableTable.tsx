import React from "react";
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Typography from '@mui/material/Typography';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
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
