import { Divider } from "@material-ui/core";
import React, { useEffect, useState } from "react";
import ProductLine from "../components/ProductLine";
import { useDispatch } from "react-redux";
import ProgressBox from "../../../components/progress/ProgressBox";
import { httpGetProducts } from "../../../services/ProductService";
import { setSuccessStatus } from "../../../store/status/StatusActions";
import IProducts from "../../../interfaces/product/IProducts";
import IProduct from "../../../interfaces/product/IProduct";
import IProductType from "../../../interfaces/product/IProductType";

export default () => {
    const dispatch = useDispatch();
    const [isLoading, setIsLoading] = useState<boolean|undefined>(undefined);
    const [products, setProducts] = useState<IProducts|undefined>(undefined);
    const [pageNumber, setPageNumber] = useState<number>(0);
    
    useEffect(() => {
        if(!products && !isLoading)
        {
            setIsLoading(true);
            httpGetProducts(pageNumber, 
                (products) => {
                    setProducts(products);
                    setIsLoading(false);
                },
                (message) => {
                    dispatch(setSuccessStatus(message))
                    setIsLoading(false);
                }
            );
        }
    });
    
    return <>
        {/* MIN Stock Threshold */}
        <Divider/>
        {isLoading || !products ?
            <ProgressBox/>
            :
            products.items.map((product:IProduct, index1:number) =>
                product.types.map((productType:IProductType, index2:number) =>
                    <ProductLine 
                        key={`product-${index1}-type-${index2}`}
                        productType={productType}
                        productId={product.id}
                        title={product.title}
                        supplements={product.supplements}
                    />
                )
            )
        }
        <Divider/>
    </>
}

