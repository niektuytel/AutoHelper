import { Action, Reducer } from 'redux';
import { ICookieProduct } from '../../interfaces/ICookieProduct';
import { GetStorageProducts } from '../localStorageManager';
import { KnownCartAction } from './CartActions';
import CartState from './CartState';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: CartState = { type: undefined, items: [] };

export const reducer: Reducer<CartState> = (state: CartState | undefined, incomingAction: Action): CartState => {
    if (state === undefined) {
        let value = {...unloadedState, items:GetStorageProducts()}
        return value;
    }

    const action = incomingAction as KnownCartAction;
    switch (action.type) {
        case 'ADD_CART_ITEM':
            return {
                type: action.type,
                items: [ ...state.items, action.item]
            };
        case 'UPDATE_CART_ITEM':
            let items = state.items.map((item: ICookieProduct) => item.currentType.id === action.item.currentType.id ? action.item : item);
            return {
                type: action.type,
                items: items
            };
        case 'REMOVE_CART_ITEM':
            let removed_items = state.items.filter((item: ICookieProduct) => item.currentType.id !== action.item.currentType.id);
            return {
                type: action.type,
                items: removed_items
            };
        case 'REMOVE_ALL_CART_ITEMS':
            return {
                type: action.type,
                items: []
            }
        default: 
            break;
    }

    return state;
};
