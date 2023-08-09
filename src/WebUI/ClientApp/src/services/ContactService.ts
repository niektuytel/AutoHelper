import axios from "axios";
import { msalInstance } from "..";
import IContactEmail from "../interfaces/IContactEmail";
import IContactQuestion from "../interfaces/IContactQuestion";
import { loginRequest } from "../msalConfig";

export const httpSendContactMail = (
    data:IContactEmail,
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    axios.post(`/api/contact/send/mail`, data)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpGetContactFAQs = (
    onSuccess: (data:any) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/contact/faq/all`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostContactFAQ = (
    data:IContactQuestion,
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/contact/faq`, data, { headers: { Authorization: 'Bearer ' + token } })
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
    })
}

export const httpPutContactFAQ = (
    data: IContactQuestion,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/contact/faq`, {  ...data }, { headers: { Authorization: 'Bearer ' + token } })
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
    })
}

export const httpDeleteContactFAQ = (
    id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/contact/faq/${id}`,
            { headers: { Authorization: 'Bearer ' + token } }
        )
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
    })
}
