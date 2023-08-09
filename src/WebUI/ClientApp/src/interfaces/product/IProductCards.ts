import ICardProductDto from "./IProductCardDto";

export default interface ICardProducts {
    items: ICardProductDto[];
    pageNumber:number;
    totalPages:number;
    totalCount:number;
    hasPreviousPage:boolean;
    hasNextPage:boolean;
}