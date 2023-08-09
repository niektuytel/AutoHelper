import React, { useState } from "react";
import { Checkbox, Table, TableBody, TableCell, TableHead, TableRow, Typography } from "@material-ui/core";

import ControlButtons from "../control_buttons/ControlButtons";
import { StyledTableRow, UseStyles } from "./SimpleTableStyle";
import { colorOnIndex } from "../../i18n/ColorValues";

export interface ITableData {
    header: string[],
    data: any[][]
}

export interface ITableProps {
    data: ITableData,
    onCreate?: () => void,
    onEdit?: (index:number) => void,
    onDelete?: () => void,
    fromAccordion?: boolean,
    isAdmin?: boolean|true
}

export default ({data, onCreate, onEdit, onDelete, fromAccordion, isAdmin}:ITableProps) => {
    const classes = UseStyles();
    const [selected, setSelected] = useState(-1);

    return <>
        <Table className={classes.table} aria-label="simple table">
            <TableHead>
                <TableRow>
                    {data.header.map((cell:string, cell_index:number) => 
                        (cell_index === 0) ? 
                            <TableCell key={`table-cell=${cell_index}`}>
                                {!fromAccordion &&
                                    <Typography variant="h6" gutterBottom>
                                        {cell}
                                    </Typography>
                                }
                            </TableCell>
                        : (cell_index === 1 && isAdmin) ?
                            <TableCell align="right" key={`table-cell=${cell_index}`}>
                                <ControlButtons 
                                    onCreate={onCreate} 
                                    onEdit={(onEdit && selected !== -1) ? () => onEdit(selected) : undefined}
                                    onDelete={(onDelete && selected !== -1) ? () => onDelete() : undefined} 
                                    containStyle={false}
                                    isAdmin={isAdmin}
                                />
                            </TableCell>
                        :
                            <TableCell align="right" key={`table-cell=${cell_index}`}>
                                {cell}
                            </TableCell>
                    )}
                </TableRow>
            </TableHead>
            <TableBody>
                {data.data.map((row:React.Component[], index:number) => (
                    <StyledTableRow key={`StyledTableRow-${index}`}>
                         {/* style={{backgroundColor: (index % 2 === 0) ? colorOnIndex(index) + "39" : "none"}}> */}
                        {
                            row.map((row:React.Component, cell_index:number) => 
                                (cell_index === 0) ? 
                                    <TableCell component="th" scope="row" key={`table-cell=${cell_index}`}>
                                        {isAdmin &&
                                            <Checkbox
                                                checked={index === selected}
                                                onChange={() => setSelected(index)}
                                                inputProps={{ 'aria-label': 'select all desserts' }}
                                                style={{color:colorOnIndex(index)}}
                                            />
                                        }
                                        {row}
                                    </TableCell>
                                :
                                    <TableCell align="right" key={`table-cell=${cell_index}`}>
                                        {row}
                                    </TableCell>
                            )
                        }
                    </StyledTableRow>
                ))}
            </TableBody>
        </Table>
    </>
}
