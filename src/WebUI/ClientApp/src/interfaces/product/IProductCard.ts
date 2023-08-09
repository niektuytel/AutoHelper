import IProductDescription from "./IProductDescription";
import ICardProductType from "./IProductCardType";
import IProductSpecLine from "./IProductSpecLine";
import ITagSupplement from "../tag/ITagSupplement";

export const emptyCardProduct:ICardProduct = {
    id: 0,
    title: "",
    description: "",
    supplements: [],
    types: [],
    specifications: []
}

export default interface ICardProduct {
    id:number;
    title: string;
    description: string;
    supplements: ITagSupplement[];
    types: ICardProductType[];
    specifications: IProductSpecLine[];
}