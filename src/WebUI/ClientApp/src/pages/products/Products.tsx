import React, { useEffect, useState } from "react";
import PostAddIcon from '@material-ui/icons/PostAdd';
import { Button, Container, Grid } from "@material-ui/core";

import { useHistory } from "react-router-dom";
import ProductsSortingSection from "./sections/SortingSection";
import { useDispatch, useSelector } from "react-redux";
import ProductsState from "../../store/products/ProductsState";
import { requestCardProducts as requestProductCards } from "../../store/products/ProductsActions";
import ProductCard from "./components/ProductCard";
import FooterPagination from "./components/LargePaginator";
import ProgressBox from "../../components/progress/ProgressBox";
import IProductCardDto from "../../interfaces/product/IProductCardDto";

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    const history = useHistory();
    const dispatch = useDispatch();
    const {isLoading, products: products}: ProductsState = useSelector((state:any) => state.products);
    const [sortOnTags, setSortOnTags] = useState<string[]>([]);
    const [sortOnPrice, setSortOnPrice] = useState<boolean>(false);
    const [sortOnPopularity, setSortOnPopularity] = useState<boolean>(true);
    const [pageNumber, setPageNumber] = useState<number>(0);
    
    useEffect(() => {
        if(!products && !isLoading)
        {
            dispatch(requestProductCards("TODO: Admin Azure account ID from azure!!!", pageNumber, sortOnTags, sortOnPopularity, sortOnPrice));
        }
    });
    
    const onNewProduct = () => {
        history.push(`/product/-1`)
    }

    return <>
        <Container maxWidth="lg" style={{padding:"0"}}>
            <ProductsSortingSection
                sortOnTags={sortOnTags}
                sortOnPrice={sortOnPrice}
                setSortOnPrice={setSortOnPrice}
                sortOnPopularity={sortOnPopularity}
                setSortOnPopularity={setSortOnPopularity}
                pageNumber={pageNumber}
                setPageNumber={setPageNumber} 
                hasPrev={products ? products.hasPreviousPage : false}
                hasNext={products ? products.hasNextPage : false}
            />
            {/* <PersonalFilterSection
                setSortOnTags={setSortOnTags}
                sortOnPrice={sortOnPrice}
                sortOnPopularity={sortOnPopularity}
            /> */}
            {isAdmin &&
                <Button onClick={onNewProduct}>
                    <PostAddIcon fontSize="medium"/>
                </Button>
            }
            <Grid style={{ flexGrow: 1, marginTop:2, marginBottom:2}} container spacing={2}>
                <Grid item xs={12}>
                    <Grid container justifyContent="center" spacing={2}>
                        {isLoading || !products ?
                            <ProgressBox/>
                            :
                            products.items.map((product:IProductCardDto, index:number) =>
                                <Grid key={`product-card-${index}`} item>
                                    <ProductCard index={index} product={product}/>
                                </Grid>
                            )
                        }
                    </Grid>
                </Grid>
            </Grid>
            <FooterPagination 
                sortOnTags={sortOnTags}
                sortOnPrice={sortOnPrice}
                sortOnPopularity={sortOnPopularity}
                pageNumber={pageNumber}
                setPageNumber={setPageNumber} 
                hasPrev={products ? products.hasPreviousPage : false}
                hasNext={products ? products.hasNextPage : false}
            />
        </Container>
    </>
}
