import { ICookieProduct } from "../../interfaces/ICookieProduct";

export default interface CartState {
    type: string|undefined;
    items: ICookieProduct[];
}