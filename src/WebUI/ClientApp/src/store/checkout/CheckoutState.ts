import IDelivery from "../../interfaces/IDelivery";

export default interface CheckoutState {
    type: string|undefined;
    loading: boolean;
    valid_payment: boolean;
    currentStep: number;
    method:string;
    delivery: IDelivery;
}