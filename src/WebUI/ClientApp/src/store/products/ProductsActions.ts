import axios from 'axios';
import { AppThunkAction } from '..';
import IProductCards from '../../interfaces/product/IProductCards';

const pageSize = 12;

interface ProductsRequestAction {
    type: 'PRODUCTS_REQUEST';
}

interface ProductsResponseAction {
    type: 'PRODUCTS_RESPONSE';
    products: IProductCards;
}

interface RemoveProductAction {
    type: 'REMOVE_PRODUCT';
    id: number;
}

export type KnownAction = ProductsRequestAction | ProductsResponseAction | RemoveProductAction;
export const requestCardProducts = (
    userID: string,
    pageNumber:number,
    sortOnTags?:string[], 
    sortOnPopularity?: boolean, 
    sortOnPrice?: boolean
): AppThunkAction<KnownAction> => 
(dispatch) => {
    dispatch({ type: 'PRODUCTS_REQUEST' });

    // create uri
    let uri = `/api/product/all/card`;
    let useQuery = false;

    if(userID)
    {
        uri += ((useQuery)? "&" : "?") + `userID=${sortOnPopularity}`;
        useQuery = true;
    }

    if(sortOnTags && sortOnTags.length > 0)
    {
        sortOnTags.forEach(item => {
            uri += ((useQuery)? "&" : "?") + `&SortOnTags=${item}`;
            useQuery = true;
        });
    }

    if(sortOnPopularity)
    {
        uri += ((useQuery)? "&" : "?") + `SortOnPopularity=${sortOnPopularity}`;
        useQuery = true;
    }
    
    if(sortOnPrice)
    {
        uri += ((useQuery)? "&" : "?") + `SortOnPrice=${sortOnPrice}`;
        useQuery = true;
    }
    
    if(pageNumber)
    {
        uri += ((useQuery)? "&" : "?") + `PageNumber=${pageNumber}`;
        useQuery = true;
    }

    if(pageSize)
    {
        uri += ((useQuery)? "&" : "?") + `PageSize=${pageSize}`;
        useQuery = true;
    }

    axios.get(uri)
    .then(response => {
        console.log(response)
        dispatch({ type: 'PRODUCTS_RESPONSE', products: response.data });
    })
    .catch(error => {
        console.log(error)
    })
};

export const removeProduct = (id: number): AppThunkAction<KnownAction> => 
(dispatch) => {
    dispatch({ type: 'REMOVE_PRODUCT', id: id });
};
