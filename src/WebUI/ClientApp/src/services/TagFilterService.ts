import axios from "axios";
import { msalInstance } from "..";
import ITag from "../interfaces/tag/ITag";
import ITagFilter from "../interfaces/tag/ITagFilter";
import ITagFilterSituation from "../interfaces/tag/ITagFilterSituation";
import ITagFilterTarget from "../interfaces/tag/ITagFilterTarget";
import ITagFilterWithSituations from "../interfaces/tag/ITagFilterWithSituations";
import { loginRequest } from "../msalConfig";

export const httpGetTagNextFilters = (
    target:ITagFilterTarget, 
    situations:ITagFilterSituation[],
    onSuccess: (filters:ITagFilterWithSituations[]) => void,
    onError: (message:string) => void
) => {
    let payload = {
        responseSize:3,
        targetId: target.id,
        situationIds: situations.map(item => item.id)
    };
    
    axios.post(`/api/tagfilter/next`, payload)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error.message);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpGetTagFilters = (
    onSuccess: (filters:ITagFilter[]) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/tagfilter/all`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error.message);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostTagFilter = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/tagfilter`, data, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpPutTagFilter = (
    id: number,
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/tagfilter`, { ...data, id:id }, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpDeleteTagFilter = (
    id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/tagfilter/${id}`, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}




export const httpGetTagFilterTargets = (
    onSuccess: (targets:ITagFilterTarget[]) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/tagfilter/target/all`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error.message);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostTagFilterTarget = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/tagfilter/target`, data, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    });
}

export const httpPutTagFilterTarget = (
    id: number,
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/tagfilter/target`, { ...data, id:id }, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpDeleteTagFilterTarget = (
    id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken; 
        axios.delete(`/api/tagfilter/target/${id}`, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}




export const httpPostTagFilterTargetSupplement = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/tagfilter/target/supplement`, data, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpPutTagFilterTargetSupplement = (
    id: number,
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/tagfilter/target/supplement`, { ...data, id:id }, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        });
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpDeleteTagFilterTargetSupplement = (
    id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/tagfilter/target/supplement/${id}`, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}




export const httpGetTagFilterSituations = (
    onSuccess: (Answers:ITagFilterSituation[]) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/tagfilter/situation/all`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error.message);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpPostTagFilterSituation = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/tagfilter/situation`, data, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        });
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpPutTagFilterSituation = (
    id: number,
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/tagfilter/situation`, { ...data, id:id }, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    })
}

export const httpDeleteTagFilterSituation = (
    situation: ITagFilterSituation,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/tagfilter/situation/${situation.id}`, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            onSuccess(response.data);
        })
        .catch(error => {
            console.log(error.message);
            onError(JSON.stringify(error.response.data));
        })
    })
    .catch((error) => {
        console.log(error);
    });
}



