import { ICookieProduct } from "../interfaces/ICookieProduct";

export const AddStorageProduct = (product:ICookieProduct) => {
    let products:ICookieProduct[] = GetStorageProducts();
    
    products.push(product);
    localStorage.setItem("cart", JSON.stringify(products))
}

export const UpdateStorageProduct = (product:ICookieProduct) => {
    let products:ICookieProduct[] = GetStorageProducts();

    let items = products.map((entity:ICookieProduct) => 
        (entity.id === product.id && entity.currentType.id === product.currentType.id) 
        ? 
            product 
        :
            entity
    );

    localStorage.setItem("cart", JSON.stringify(items))
}

export const RemoveStorageProduct = (product:ICookieProduct) => {
    let products:ICookieProduct[] = GetStorageProducts();
    let items = products.filter(
        (entity:ICookieProduct) => (entity.id !== product.id && entity.currentType.id !== product.currentType.id)
    );
    
    localStorage.setItem("cart", JSON.stringify(items))
}

export const RemoveAllStorageProducts = () => {
    localStorage.setItem("cart", JSON.stringify([]))
}

export const GetStorageProducts = ():ICookieProduct[] => {
    const data = localStorage.getItem("cart");

    if(data)
    {
        const items:ICookieProduct[] = JSON.parse(data);
        return items;
    }
    
    return [];
}

export const GetPaymentCookie = () => {
    const data = localStorage.getItem("payment");
    if(data === null)
    {
        return undefined;
    }
    
    return JSON.parse(data);
}

export const SetOrderCookie = (data:any) => {
    localStorage.setItem("payment", JSON.stringify(data))
}


