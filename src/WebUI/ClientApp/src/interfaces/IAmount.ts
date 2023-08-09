
export const emptyAmount:IAmount = {
    value: "0.00",
    currency: "EUR"
}

export default interface IAmount {
    value:string;
    currency:string;
}