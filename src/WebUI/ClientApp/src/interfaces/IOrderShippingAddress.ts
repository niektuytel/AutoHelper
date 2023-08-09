
export const emptyOrderShippingAddress: IOrderShippingAddress = {
    address: "",
    city: "",
    email: "",
    name: "",
    postalCode: ""
}

export default interface IOrderShippingAddress {
    address:string;
    city:string;
    email:string;
    name:string;
    postalCode:string;
}
