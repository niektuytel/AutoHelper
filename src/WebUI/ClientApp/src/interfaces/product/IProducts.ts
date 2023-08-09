import IProduct from "./IProduct";


export default interface IProducts {
    items: IProduct[];
    pageNumber:number;
    totalPages:number;
    totalCount:number;
    hasPreviousPage:boolean;
    hasNextPage:boolean;
}