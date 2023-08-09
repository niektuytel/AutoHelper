import { Action, Reducer } from 'redux';

import ProductsState from './ProductsState';
import { KnownAction } from './ProductsActions';
import IProductCards from '../../interfaces/product/IProductCards';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: ProductsState = { type: undefined, isLoading: false, products: undefined };

export const reducer: Reducer<ProductsState> = (state: ProductsState | undefined, incomingAction: Action): ProductsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'PRODUCTS_REQUEST':
            return {
                type: action.type,
                products: state.products,
                isLoading: true
            };
        case 'PRODUCTS_RESPONSE':
            return {
                type: action.type,
                products: action.products,
                isLoading: false
            };
        case "REMOVE_PRODUCT":
            if(!state.products || !state.products.items) return state;
            var products:IProductCards = {
                ...state.products,
                items: state.products.items.filter((item:any) => item.id !== action.id)
            }

            return {
                type: action.type,
                products: products,
                isLoading: false
            };
        default: 
            break;
    }

    return state;
};
