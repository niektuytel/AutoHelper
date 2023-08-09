import React, { useState } from "react";
import { useHistory, useParams } from "react-router-dom";
import { Box, Button, IconButton, TextField, Typography } from "@material-ui/core";
import AddShoppingCartIcon from '@material-ui/icons/AddShoppingCart';
import { useTranslation } from "react-i18next";

import ProductOrderButtonsStyle from "./ProductOrderButtonsStyle";
import { ICookieProduct } from "../../../interfaces/ICookieProduct";
import { useDispatch } from "react-redux";
import { addCartItem, updateCartItem } from "../../../store/cart/CartActions";
import { GetStorageProducts } from "../../../store/localStorageManager";
import IProductType from "../../../interfaces/product/IProductType";
import IProduct from "../../../interfaces/product/IProduct";

interface IProps {
    currentType: IProductType;
    product: IProduct;
}

export default ({currentType, product}:IProps) => {
    const { t } = useTranslation();
    const { id }:any = useParams();
    const history = useHistory();
    const dispatch = useDispatch();
    const classes = ProductOrderButtonsStyle();
    const [quantity, setQuantity] = useState(1);
    
    const addToCart = (gotoCart?:boolean) => {
        var items = GetStorageProducts();
        let result = items.filter(a => (a.id === product.id && a.currentType.id === currentType.id));
        if(result.length === 0)
        {
            let item:ICookieProduct = { 
                id: product.id, 
                currentType: {
                    id: currentType.id,
                    title: currentType.title,
                    image: currentType.image,
                    price: currentType.price,
                }, 
                quantity: quantity,
                description: product.description,
                supplements: product.supplements
            };
            dispatch(addCartItem(item));
        }
        else
        {
            let item = result[0];
            item.quantity += quantity;
            dispatch(updateCartItem(item));
        }

        if(gotoCart === true) history.push("/checkout");
    }

    const onQuantityChange = (event:any) => {
        let isnan = isNaN(event.target.value);
        return !isnan && setQuantity(event.target.value)
    }

    if(!currentType) return <></>;
    return <>
        <Box className={classes.box}>
            <Box className={classes.quantity_parent}>
                <Typography variant="body1" className={classes.quantity_label}>
                    {t("amount")}:
                </Typography>
                <TextField
                    className={classes.quantity_input}
                    inputProps={{ style: { padding: 5 }, className:'digitsOnly'}}
                    name="numberformat"
                    value={quantity}
                    variant="outlined"
                    onChange={onQuantityChange}
                />
            </Box>
            <Box>
                <Typography variant="h6" className={classes.price}>
                    {t("price_prefix")}{(currentType.price * quantity).toFixed(2)}
                </Typography>
                <div className={classes.flexGrow}>
                    <Button 
                        disabled={quantity < 1}
                        variant="contained"
                        component="span"
                        onClick={() => addToCart(true)}
                    >
                        {t("pay")}
                    </Button>
                    <IconButton 
                        disabled={quantity < 1}
                        aria-label="upload picture" 
                        component="span"
                        onClick={() => addToCart()}
                    >
                        <AddShoppingCartIcon />
                    </IconButton>
                </div>
            </Box>
        </Box>
    </>
}