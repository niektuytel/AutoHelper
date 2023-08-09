import axios from "axios";
import ICardProduct from "../interfaces/product/IProductCard";
import IProduct from "../interfaces/product/IProduct";
import IProducts from "../interfaces/product/IProducts";
import IProductTypeStock from "../interfaces/product/IProductTypeStock";
import { msalInstance } from "..";
import { loginRequest } from "../msalConfig";


export const httpGetProductCards = (
    pageIndex: number,
    onSuccess: (product:ICardProduct) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/product/all/card?PageNumber=${pageIndex}`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpGetProducts = (
    pageIndex: number,
    onSuccess: (products:IProducts) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.get(`/api/product/all?PageNumber=${pageIndex}`, 
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

export const httpGetProduct = (
    id: number,
    onSuccess: (product:IProduct) => void,
    onError: (message:string) => void
) => {
    axios.get(`/api/product/${id}`)
    .then(response => {
        console.log(response);
        onSuccess(response.data);
    })
    .catch(error => {
        console.log(error);
        onError(JSON.stringify(error.response.data));
    })
}

export const httpDeleteProduct = (
    id: string,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {

    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;        
        axios.delete(`/api/product/${id}`, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPostProduct = (
    id: string,
    data: any,
    onSuccess: (id:number, new_item:boolean) => void,
    onError: (message:string) => void
) => {
    if (id === "-1") 
    {
        msalInstance.acquireTokenSilent({ ...loginRequest })
        .then((res:any) => {
            var token = res.accessToken;
            axios.post(`/api/product`, data, { headers: { Authorization: 'Bearer ' + token } })
            .then(response => {
                console.log(response);
                if(response.data !== id)
                {
                    onSuccess(response.data, true);
                }
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
    else
    {
        httpPutProduct(id, data, (data:any) => onSuccess(data, false), onError);
    }
}

export const httpPutProduct = (
    id: string,
    data: ICardProduct,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/product`, { ...data, id:id }, { headers: { Authorization: 'Bearer ' + token } })
        .then(response => {
            console.log(response);
            if(response.data !== id)
            {
                onSuccess(response.data);
            }
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

export const httpPostProductImage = (
    image: Blob,
    onSuccess: (filename:string) => void,
    onError: (message:string) => void
) => {
    const formData = new FormData();
    formData.append("file", image);
    
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/product/upload`, formData, {
            headers: { 
                'Content-Type': 'multipart/form-data',
                'Authorization': 'Bearer ' + token
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
    })
}

export const httpPostProductType = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {

    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/product/type`, data, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPutProductType = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/product/type`, { ...data }, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpDeleteProductType = (
    id: string,
    typeId: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;        
        axios.delete(`/api/product/type/${id}/${typeId}`, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPostProductTag = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.post(`/api/product/tag`, data, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPutProductTag = (
    data: any,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/product/tag`, { ...data }, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpDeleteProductTag = (
    product_id: string,
    tag_id: number,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.delete(`/api/product/tag/${product_id}/${tag_id}`, { headers: { Authorization: 'Bearer ' + token } })
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

export const httpPutProductTypeStock = (
    productId: string,
    productTypeId: string,
    data: IProductTypeStock,
    onSuccess: (id:number) => void,
    onError: (message:string) => void
) => {
    msalInstance.acquireTokenSilent({ ...loginRequest })
    .then((res:any) => {
        var token = res.accessToken;
        axios.put(`/api/product/stock`, 
            {  stock:data, productId: productId, productTypeId:productTypeId }, 
            { headers: { Authorization: 'Bearer ' + token } }
        )
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
