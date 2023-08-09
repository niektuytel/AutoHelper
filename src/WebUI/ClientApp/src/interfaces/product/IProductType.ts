import IProductTypeStock, { emptyProductTypeStock } from "./IProductTypeStock";

export const emptyProductType:IProductType = {
    id: 0,
    title: "",
    image: "",
    price: 0,
    stock: emptyProductTypeStock
}

export default interface IProductType {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Name of a specific product type from it's parent product
    */
    title: string,

    /**
     * HTTP url to the represented image
    */
    image: string,

    /**
     * Precise price, this is required to get a accurate result
    */
    price: number;

    /**
     * The stock amount (etc.) of this product type
    */
    stock: IProductTypeStock;
}