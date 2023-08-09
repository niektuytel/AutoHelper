import React from "react"
import { TableCell, Typography } from "@material-ui/core"
import IOrder from "../../../interfaces/IOrder";
import IOrderLine from "../../../interfaces/IOrderLine";

interface IProps {
    order: IOrder;
}

const validItem = (name:string) => {
    if (name === "Bezorg kosten")
    {
        return false;
    }

    return true;
}

export default ({order}:IProps) => {


    return <TableCell align="left">
        {order.lines.map((line:IOrderLine, index:number) => validItem(line.description) && 
            <Typography key={`order-line-${index}`}>
                {`${line.quantity} X ${line.description}`}
            </Typography>
        )}
    </TableCell>
}




