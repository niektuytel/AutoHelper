import { AppThunkAction } from '..';
import { ICookieProduct } from '../../interfaces/ICookieProduct';
import { AddStorageProduct, RemoveAllStorageProducts, RemoveStorageProduct, UpdateStorageProduct } from '../localStorageManager';

interface AddCartItemAction {
    type: 'ADD_CART_ITEM';
    item: ICookieProduct;
}

interface UpdateCartItemAction {
    type: 'UPDATE_CART_ITEM';
    item: ICookieProduct;
}

interface RemoveCartItemAction {
    type: 'REMOVE_CART_ITEM';
    item: ICookieProduct;
}

interface RemoveAllCartItemsAction {
    type: 'REMOVE_ALL_CART_ITEMS';
}

export type KnownCartAction = AddCartItemAction | UpdateCartItemAction | RemoveCartItemAction | RemoveAllCartItemsAction;
    
export const addCartItem = (item: ICookieProduct): AppThunkAction<KnownCartAction> => 
    (dispatch) => {
        if(!item.currentType) return;
        
        // on cookie
        AddStorageProduct(item);

        // on redux
        dispatch({ type: 'ADD_CART_ITEM', item:item });
    };
    
export const updateCartItem = (item: ICookieProduct): AppThunkAction<KnownCartAction> => 
    (dispatch) => {
        // on cookie
        UpdateStorageProduct(item);

        // on redux
        dispatch({ type: 'UPDATE_CART_ITEM', item:item });
    };

    
export const removeCartItem = (item: ICookieProduct): AppThunkAction<KnownCartAction> => 
(dispatch) => {
    // on cookie
    RemoveStorageProduct(item);

    // on redux
    dispatch({ type: 'REMOVE_CART_ITEM', item:item });
};


    
export const removeAllCartItems = (): AppThunkAction<KnownCartAction> => 
(dispatch) => {
    // on cookie
    RemoveAllStorageProducts();

    // on redux
    dispatch({ type: 'REMOVE_ALL_CART_ITEMS' });
};




