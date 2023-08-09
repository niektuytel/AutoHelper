import { Action, Reducer } from 'redux';
import { emptyDelivery } from '../../interfaces/IDelivery';
import { KnownCheckoutAction } from './CheckoutActions';
import CheckoutState from './CheckoutState';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: CheckoutState = { 
    type: undefined, 
    valid_payment: false,
    loading: false,
    method: "ideal",
    currentStep: 0,
    delivery: emptyDelivery
};

export const reducer: Reducer<CheckoutState> = (state: CheckoutState | undefined, incomingAction: Action): CheckoutState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownCheckoutAction;
    switch (action.type) {
        case 'SET_CURRENT_STEP':
            return {
                ...state,
                ...action
            };
        case 'UPDATE_CHECKOUT':
            return {
                ...state,
                ...action
            };
        case 'SET_METHOD':
            return {
                ...state,
                ...action
            };
        case 'CREATE_PAYMENT':
            return {
                ...state,
                ...action
            };
        case 'VALIDATE_PAYMENT':
            return {
                ...state,
                ...action
            };
        default: 
            break;
    }

    return state;
};
