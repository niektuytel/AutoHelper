

export const emptyDelivery:IDelivery = {
    firstName: "",
    lastName: "",
    email: "",
    address: "",
    city: "",
    postalCode: ""
}


export default interface IDelivery {
    firstName:string;
    lastName:string;
    email:string;
    address: string;
    city: string;
    postalCode: string;
}