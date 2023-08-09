import IProductCards from "../../interfaces/product/IProductCards";


export default interface ProductState {
    type: string|undefined;
    isLoading: boolean;
    products: IProductCards | undefined;
}