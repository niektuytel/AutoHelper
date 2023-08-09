import { AppThunkAction } from '..';
import IDelivery from '../../interfaces/IDelivery';
import IOrderLine from '../../interfaces/IPaymentLine';
import { httpGetOrderValidation, httpPostOrder } from '../../services/OrderService';
import { SetOrderCookie } from '../localStorageManager';
import { KnownStatusAction } from '../status/StatusActions';

interface SetCurrentStepAction {
    type: 'SET_CURRENT_STEP';
    currentStep: number;
    loading:boolean;
}

interface UpdateCheckoutAction {
    type: 'UPDATE_CHECKOUT';
    delivery: IDelivery;
    loading:boolean;
}

interface SetMethodAction {
    type: 'SET_METHOD';
    method: string;
    loading:boolean;
}

interface CreatePaymentAction {
    type: 'CREATE_PAYMENT';
    loading:boolean;
}

interface ValidatePaymentAction {
    type: 'VALIDATE_PAYMENT';
    valid_payment?: boolean;
    loading: boolean;
}

export type KnownCheckoutAction = SetCurrentStepAction | UpdateCheckoutAction | SetMethodAction | CreatePaymentAction | ValidatePaymentAction;

export const SetCheckoutStep = (step:number): AppThunkAction<KnownCheckoutAction> => 
(dispatch) => {
    // on redux
    dispatch({ type: 'SET_CURRENT_STEP', currentStep:step, loading:false });
};

export const UpdateDelivery = (delivery: IDelivery): AppThunkAction<KnownCheckoutAction> => 
(dispatch) => {
    // on redux
    dispatch({ type: 'UPDATE_CHECKOUT', delivery:delivery, loading:false });
};
    
export const SetMethod = (method: string): AppThunkAction<KnownCheckoutAction> => 
(dispatch) => {
    // on redux
    dispatch({ type: 'SET_METHOD', method:method, loading:false });
};
    
export const CreatePayment = (delivery:IDelivery, method:string): AppThunkAction<KnownCheckoutAction|KnownStatusAction> => 
(dispatch, getState) => {
    // set is loading
    dispatch({ type: 'CREATE_PAYMENT', loading:true });

    // create payment
    const state = getState();
    var items = state.cart.items;
    let body:IOrderLine[] = items.map(element => ({
        productId:element.id,
        typeId: element.currentType.id,
        quantity: element.quantity
    }));

    httpPostOrder(delivery, body, method, 
        (response:any) => {
            SetOrderCookie(response);

            // NOTE: no need to set loading false as wel will reload the website on the redirect with defult loading: false
            window.location.href = response.checkoutUrl;
        },
        (message:string) => {
            dispatch({ type: 'SET_ERROR_STATUS', error_message:message });
        }
    )
};

export const CheckOrder = (id:string): AppThunkAction<KnownCheckoutAction|KnownStatusAction> => 
(dispatch) => {
    dispatch({ type: 'VALIDATE_PAYMENT', loading:true });

    httpGetOrderValidation(id, 
        (response) => {
            const valid_payment = response;
            dispatch({ type: 'VALIDATE_PAYMENT', valid_payment:valid_payment, loading:false });
        },
        (message) => {
            dispatch({ type: 'SET_ERROR_STATUS', error_message:message });
        }
    );
};


