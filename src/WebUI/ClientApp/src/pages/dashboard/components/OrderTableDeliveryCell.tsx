import React from "react"
import { TableCell, Typography } from "@material-ui/core"
import IOrder from "../../../interfaces/IOrder";


interface IProps {
    order: IOrder;
}

export default ({order}:IProps) => {
    const name = order.address.name;
    const address = order.address.address;
    const city = order.address.city + " " + order.address.postalCode;

    return <TableCell 
        component="th" 
        id={`order-products-cell`} 
        scope="row" 
        padding="none"
        style={{
            paddingLeft:"10px"
        }}
    >
        <Typography>{name}</Typography>
        <Typography>{address}</Typography>
        <Typography>{city}</Typography>
    </TableCell>
}




