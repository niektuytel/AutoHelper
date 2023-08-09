import axios from "axios";
import { msalInstance } from "..";
import ITag from "../interfaces/tag/ITag";
import ITagFilter from "../interfaces/tag/ITagFilter";
import ITagFilterSituation from "../interfaces/tag/ITagFilterSituation";
import ITagFilterTarget from "../interfaces/tag/ITagFilterTarget";
import { loginRequest } from "../msalConfig";

export const httpGetTags = (
    onSuccess: (data:ITag[]) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/tag/all`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostTag = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/tag`, data, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPutTag = (
    id: string,
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/tag`, { id:id, ...data }, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpDeleteTag = (
    id: string,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/tag/${id}`, { headers: { Authorization: 'Bearer ' + token } })
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