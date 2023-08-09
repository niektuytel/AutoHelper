
export const emptyProductTypeStock:IProductTypeStock = {
    id: 0,
    stockAmount: 0,
    totalSold: 0
}

export default interface IProductTypeStock {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Available amount of stock on this product
    */
    stockAmount: number;
     
    /**
     * Total amount of stock that this product type has been sold
    */
    totalSold: number;
}