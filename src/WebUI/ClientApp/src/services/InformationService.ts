import axios from "axios";
import { msalInstance } from "..";
import IInformationItem from "../interfaces/IInformationItem";
import { loginRequest } from "../msalConfig";

export const httpGetInformationItems = (
    pageIndex: number,
    onSuccess: (data:any) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/information/all?PageNumber=${pageIndex}`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostInformationItem = (
    data:IInformationItem,
    onSuccess: (response:any) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        
        axios.post(`/api/information`, data, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPostInformationItemImage = (
    image: Blob,
    onSuccess: (filename:string) => void,
    onError: (message:string) => void
) => {
    const formData = new FormData();
    formData.append("file", image);

    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/information/upload/image`, formData, {
            headers: { 
                'Content-Type': 'multipart/form-data',
                "Authorization": 'Bearer ' + token
            }
        })
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

export const httpPutInformationItem = (
    data: IInformationItem,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {

    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/information`, {  ...data }, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpDeleteInformationItem = (
    id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/information/${id}`, { headers: { Authorization: 'Bearer ' + token } })
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


