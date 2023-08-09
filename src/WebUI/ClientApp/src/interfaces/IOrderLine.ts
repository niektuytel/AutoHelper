import IAmount, { emptyAmount } from "./IAmount";
import IUri, { emptyUri } from "./IUri";


export const emptyOrderLine:IOrderLine = {
    description: "",
    quantity: 0,
    amountTotal: 0.00
    // status: "",
    // unitPrice: emptyAmount,
    // vateRate: "",
    // imageUrl: emptyUri
}

export default interface IOrderLine {
    description:string;
    quantity:number;
    amountTotal: number;
    // status:string;
    // totalAmount: IAmount;
    // vateRate: string;
    // imageUrl: IUri;
}