import React from "react"
import { TableCell, Typography } from "@material-ui/core"
import IOrder from "../../../interfaces/IOrder";

interface IProps {
    order: IOrder;
}

export default ({order}:IProps) => {
    let splittedT = order.paidAt.split("T");
    let date = splittedT[0]
    let time = splittedT[1].split("+")[0];
    
    return <TableCell align="left">
        <Typography>
            status: {`${order.status}`}
        </Typography>
        <Typography>{date}</Typography>
        <Typography>{time}</Typography>
    </TableCell>
}




