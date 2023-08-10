import React, { useState } from "react";
import { Checkbox, Table, TableBody, TableCell, TableHead, TableRow, Typography } from "@mui/material";
import ControlButtons from "../control_buttons/ControlButtons";
import { colorOnIndex } from "../../i18n/ColorValues";

const StyledTableRow = {
    root: {
        '&:nth-of-type(odd)': {
            backgroundColor: "#c8c8c8",
        },
    },
}

const styles = {
    table: {
        width: "100%",
    }
}


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
    const [selected, setSelected] = useState(-1);

    return <>
        <Table sx={styles.table} aria-label="simple table">
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
                {/*{data.data.map((row: React.Component[], rowIndex: number) => (*/}
                {/*    <div key={`StyledTableRow-${rowIndex}`} className={StyledTableRow}>*/}
                {/*        {row.map((cell: React.Component, cellIndex: number) =>*/}
                {/*            (cellIndex === 0) ?*/}
                {/*                <TableCell component="th" scope="row" key={`table-cell=${cellIndex}`}>*/}
                {/*                    {isAdmin &&*/}
                {/*                        <Checkbox*/}
                {/*                            checked={rowIndex === selected}*/}
                {/*                            onChange={() => setSelected(rowIndex)}*/}
                {/*                            inputProps={{ 'aria-label': 'select all desserts' }}*/}
                {/*                            style={{ color: colorOnIndex(rowIndex) }}*/}
                {/*                        />*/}
                {/*                    }*/}
                {/*                    {cell}*/}
                {/*                </TableCell>*/}
                {/*                :*/}
                {/*                <TableCell align="right" key={`table-cell=${cellIndex}`}>*/}
                {/*                    {cell}*/}
                {/*                </TableCell>*/}
                {/*        )}*/}
                {/*    </div>*/}
                {/*))}*/}
            </TableBody>

        </Table>
    </>
}
