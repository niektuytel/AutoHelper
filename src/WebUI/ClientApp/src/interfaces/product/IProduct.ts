import IProductSpecLine from "./IProductSpecLine";
import IProductType from "./IProductType";
import ITagSupplement from "../tag/ITagSupplement";
import IProductInfoLine from "./IProductInfoLine";

export const emptyProduct:IProduct = {
    id: 0,
    title: "",
    description: "",
    supplements: [],
    types: [],
    specifications: [],
    isOnline: false,
    extraInformation: []
}

export default interface IProduct {
    /**
     * Id that represents parent POCO identifier
    */
    id:number;
    
    /**
     * Product name
    */
    title: string;
    
    /**
     * Descibe product [who, what, where, why] explanations
    */
    description: string;
    
    /**
     * Supplements that are inside this product
    */
    supplements: ITagSupplement[];
    
    /**
     * The same product but all different packaging types
    */
    types: IProductType[];
    
    /**
     * All specifications inside this product
    */
    specifications: IProductSpecLine[];
    
    /**
     * Extra information about this product
    */
    extraInformation: IProductInfoLine[];
    
    /**
     * When set to True, The product is published to all customers 
    */
    isOnline: boolean;
}