import React from "react"
import { Checkbox, TableCell } from "@material-ui/core"
import { colorOnStatus } from "../../../i18n/ColorValues";

interface IProps {
    status: string;
    checked: boolean;
}

export default ({status, checked}:IProps) => {

    const isCompleted = status === "delivered";
    return <TableCell 
        padding="checkbox"
        style={{
            backgroundColor: colorOnStatus(status)
        }}
    >
        { !isCompleted &&  
            <Checkbox
                checked={checked}
                inputProps={{ 
                    'aria-labelledby': `order-table-checkbox` 
                }}
                style={{
                    color: "white"
                }}
            />
        }
    </TableCell>
}




