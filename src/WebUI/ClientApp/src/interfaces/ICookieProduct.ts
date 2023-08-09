
import IProductCardType from "./product/IProductCardType";
import ITagSupplement from "./tag/ITagSupplement";


export interface ICookieProduct {
    id:number, 
    quantity: number,
    currentType:IProductCardType,
    description: string,
    supplements: ITagSupplement[]
}
