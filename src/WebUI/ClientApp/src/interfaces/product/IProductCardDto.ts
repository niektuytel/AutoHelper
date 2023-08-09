import ITagSupplement from "../tag/ITagSupplement";
import IProductCardType from "./IProductCardType";


export default interface ICardProductDto {
    id:number;
    title:string;
    description:string;
    productType:IProductCardType;
    supplements:ITagSupplement[];
}