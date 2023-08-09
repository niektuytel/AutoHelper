import IOrderShippingAddress, { emptyOrderShippingAddress } from "./IOrderShippingAddress";
import IOrderLine, { emptyOrderLine } from "./IOrderLine";

export const emptyOrder:IOrder = {
    id: "",
    paidAt: "",
    address: emptyOrderShippingAddress,
    lines: [emptyOrderLine],
    status: ""
}

export default interface IOrder {
    id:string;
    status:string;
    paidAt: string;
    address: IOrderShippingAddress;
    lines: IOrderLine[];
}