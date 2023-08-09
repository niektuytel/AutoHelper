import React from "react";
import { Button, Card, CardActionArea, CardContent, CardMedia, Grid, Typography } from "@material-ui/core";
import AddShoppingCartIcon from '@material-ui/icons/AddShoppingCart';

import { useHistory } from "react-router";

import ProductCardStyle from "./ProductCardStyle";
import { useDispatch } from "react-redux";
import { addCartItem, updateCartItem } from "../../../store/cart/CartActions";
import { GetStorageProducts } from "../../../store/localStorageManager";
import TagItems from "../../../components/tags/TagItems";
import IProductCardDto from "../../../interfaces/product/IProductCardDto";

interface IProps {
    index:number;
    product: IProductCardDto;
}

export default ({index, product}:IProps) => {
    const classes = ProductCardStyle();
    const history = useHistory();
    const dispatch = useDispatch();

    const addToCart = (event:any) => {
        var items = GetStorageProducts();
        let result = items.filter(a => (a.id === product.id && a.currentType.id === product.productType.id));
        if(result.length === 0)
        {
            let item = { 
                id: product.id, 
                currentType: product.productType, 
                quantity: 1,
                description: product.description,
                supplements: product.supplements
            };
            dispatch(addCartItem(item));
        }
        else
        {
            let item = result[0];
            item.quantity += 1;
            dispatch(updateCartItem(item));
        }

        event.stopPropagation();
        event.preventDefault();
    }
    
    const gotoPage = () => {
        history.push(`product/${product.id}`);
    }

    return <>
        <Card className={classes.product}>
            <div onClick={gotoPage} className={classes.card}>
                <CardMedia
                    className={classes.media}
                    image={product.productType ? product.productType.image : process.env.REACT_APP_EMPTY_IMAGE}
                    title={product.description}
                />
                <CardContent>
                    <Typography gutterBottom variant="h5" component="h2">
                        {product.title ? product.title : "........."}
                    </Typography>
                    <TagItems ellipsis data={product.supplements}/>
                </CardContent>
                <Grid container spacing={3}>
                    <Grid item xs={6} className={classes.price_to_left}>
                        <Typography variant="h6" className={classes.margin_left}>
                            €{product.productType ? product.productType.price.toFixed(2) : "?.??"}
                        </Typography>
                    </Grid>
                    <Grid item xs={6} className={classes.btn_to_right}>
                        <Button onClick={(event:any) => addToCart(event)}>  
                            <AddShoppingCartIcon fontSize="medium"/>
                        </Button>
                    </Grid>
                </Grid>
            </div>
        </Card>
    </>
}


