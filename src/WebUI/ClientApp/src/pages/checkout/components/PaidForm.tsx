import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import { useDispatch, useSelector } from 'react-redux';
import CartState from '../../../store/cart/CartState';
import { useEffect, useState } from 'react';
import { ICookieProduct } from '../../../interfaces/ICookieProduct';
import { removeAllCartItems } from '../../../store/cart/CartActions';
import CartItem from '../../../components/cart/CartItem';
import { Box, Divider, makeStyles } from '@material-ui/core';
import { useTranslation } from 'react-i18next';
import { SetCheckoutStep } from '../../../store/checkout/CheckoutActions';

const PaidFormStyle = makeStyles(() => ({
    item_box: { 
        height:"300px", 
        overflow: "overlay" 
    },
    padding_bottom_0: { 
        paddingBottom:"0px" 
    },
    padding_top_0: { 
        paddingTop:"0px" 
    },
    weight_700: { 
        fontWeight: 700 
    }
}));

export default () => {
    const {t} = useTranslation();
    const classes = PaidFormStyle()
    const dispatch = useDispatch();
    const { items }:CartState = useSelector((state:any) => state.cart);
    const [products, setProducts] = useState<ICookieProduct[]>([]);

    useEffect(() => {
        if (items.length > 0 && products.length === 0)
        {
            setProducts(items);
            dispatch(removeAllCartItems());
            SetCheckoutStep(0);
        }
    })
    
    var totalPrice = 0;
    products.forEach((item:ICookieProduct) => {
        totalPrice += (item.quantity * item.currentType.price);
    })

    var deliveryPrice = 3.00;
    var freeDeliveryAbove = 30.00;
    if(totalPrice >= freeDeliveryAbove)
    {
        deliveryPrice = 0.00;
    }

    return <>
        <List disablePadding>
            <Divider/>
            <Box className={classes.item_box}>
                {products.map((item, index) => 
                    <CartItem key={index} item={item} show_text show_tags/>
                )}
            </Box>
            <Divider/>
            <ListItem className={classes.padding_bottom_0}>
                <ListItemText primary={t("delivery_cost")} />
                <Typography variant="subtitle1" className={classes.weight_700}>
                    + €{(deliveryPrice).toFixed(2)}
                </Typography>
            </ListItem>
            <Divider/>
            <ListItem className={classes.padding_top_0}>
                <ListItemText primary={t("total")} />
                <Typography variant="subtitle1" className={classes.weight_700}>
                €{(totalPrice + deliveryPrice).toFixed(2)}
                </Typography>
            </ListItem>
        </List>
        <Typography variant="subtitle1" style={{textAlign:"center"}}>
            <div dangerouslySetInnerHTML={{ __html: t("paid_message") }} />
        </Typography>
    </>
}
