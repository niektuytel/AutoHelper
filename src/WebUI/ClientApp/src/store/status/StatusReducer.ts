import { Action, Reducer } from 'redux';
import { KnownStatusAction } from './StatusActions';

import StatusState from './StatusState';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: StatusState = { type: undefined, error_message: "", success_message: "" };

export const reducer: Reducer<StatusState> = (state: StatusState | undefined, incomingAction: Action): StatusState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownStatusAction;
    switch (action.type) {
        case 'SET_ERROR_STATUS':
            return {
                type: action.type,
                success_message: "",
                error_message: action.error_message
            };
        case 'SET_SUCCESS_STATUS':
            return { 
                type: action.type,
                error_message: "",
                success_message: action.success_message
            };
        default: 
            break;
    }

    return state;
};
