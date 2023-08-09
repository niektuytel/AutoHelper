import { AppThunkAction } from '..';

interface SetOnErrorAction {
    type: 'SET_ERROR_STATUS';
    error_message: string;
}

interface SetOnSuccessAction {
    type: 'SET_SUCCESS_STATUS';
    success_message: string;
}

export type KnownStatusAction = 
    SetOnErrorAction | 
    SetOnSuccessAction;
    
export const setErrorStatus = (message: string): AppThunkAction<KnownStatusAction> => 
    (dispatch) => {
        dispatch({ type: 'SET_ERROR_STATUS', error_message:message });
    };
    
export const setSuccessStatus = (message: string): AppThunkAction<KnownStatusAction> => 
    (dispatch) => {
        dispatch({ type: 'SET_SUCCESS_STATUS', success_message:message });
    };
