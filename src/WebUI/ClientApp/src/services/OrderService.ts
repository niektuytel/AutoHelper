import axios from "axios";
import { msalInstance } from "..";
import IDelivery from "../interfaces/IDelivery";
import IOrderLine from "../interfaces/IPaymentLine";
import { loginRequest } from "../msalConfig";

export const httpPutShippingOrders = (
    ids: string[],
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/order/shipping`, {Ids: ids}, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error);
            onError(JSON.stringify(error.response.data));
        });
    })
    .catch((error) => {
        console.log(error);
    });

}

export const httpGetAllOrders = (
    includeDeliveredItems: boolean,
    onSuccess: (response:any) => void,
    onError: (message:string) => void,
) => {
    
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.get(`/api/order/all?IncludeDeliveredItems=${includeDeliveredItems}`, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    });

    
}

export const httpPostOrder = (
    delivery:IDelivery,
    lines: IOrderLine[],
    method:string,
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    axios.post(`/api/order`, { Lines: lines, Method:method, Shipping:delivery })
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpGetOrderValidation = (
    id:string,
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/order/${id}`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}