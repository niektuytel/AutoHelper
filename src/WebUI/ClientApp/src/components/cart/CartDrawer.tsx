import React from "react";
import { Box, Button, Divider, Drawer, Grid, Hidden, Typography } from "@material-ui/core";
import LocalShippingIcon from '@material-ui/icons/LocalShipping';
import { useHistory } from "react-router";
import { ICookieProduct } from "../../interfaces/ICookieProduct";
import CartItem from "./CartItem";
import CartState from "../../store/cart/CartState";
import { useSelector } from "react-redux";
import { useTranslation } from "react-i18next";
import SimpleToolTip from "../tooltip/SimpleToolTip";
import CartDrawerStyle from "./CartDrawerStyle";
import IconButton from '@material-ui/core/IconButton';
import DeleteIcon from '@material-ui/icons/Delete';
import ArrowBackIosIcon from '@material-ui/icons/ArrowBackIos';
import CloseIcon from '@material-ui/icons/Close';

interface IProps {
    cartOpen: boolean;
    setCartOpen: (value:boolean) => void;
}

export default ({cartOpen, setCartOpen}:IProps) => {
    const history = useHistory();
    const { t } = useTranslation();
    const classes = CartDrawerStyle();
    const { items }:CartState = useSelector((state:any) => state.cart);

    var totalPrice = 0;
    items.forEach((item:ICookieProduct) => {
        totalPrice += (item.quantity * item.currentType.price);
    })

    var deliveryPrice = Number(process.env.REACT_APP_DELIVERY_PRICE);
    if(totalPrice >= Number(process.env.REACT_APP_FREE_DELIVERY_FROM))
    {
        deliveryPrice = 0.00;
    }

    const finishOrder = () => {
        if(items.length === 0) return;
        history.push("/checkout")
        setCartOpen(false);
    }
    
    return <>
        <Drawer 
            anchor="right" 
            open={cartOpen} 
            className={classes.total_height}
            onClose={() => setCartOpen(false)}
        >
            <Hidden mdUp>
                <Box>
                    <IconButton aria-label="delete" onClick={() => setCartOpen(false)} style={{float:"right"}}>
                        <CloseIcon style={{color:"black"}}/>
                    </IconButton>
                </Box>
            </Hidden>
            <Box style={items.length === 0 ? {width:"390px"} : {}}>
                {items.map((item:ICookieProduct, index:number) => 
                    <CartItem key={index} item={item} show_text controllable/>
                )}
            </Box>
            <Box className={classes.delivery_box}>
                <Grid container style={{padding:"10px"}}>
                    <Grid xs={4} item>
                        <Typography variant="h6">
                            {t("price")}:
                        </Typography>
                    </Grid>
                    <Grid xs={8} item>
                        <Typography variant="h6" className={classes.align_right}>
                            €{totalPrice.toFixed(2)}
                        </Typography>
                    </Grid>
                    <Grid xs={6} item>
                        <Typography variant="h6">
                            {t("delivery")}:
                            <SimpleToolTip text={`${t("free_from")} €${process.env.REACT_APP_FREE_DELIVERY_FROM}`}/>
                        </Typography>
                    </Grid>
                    <Grid xs={6} item>
                        <Typography variant="h6" className={classes.align_right}>
                            €{deliveryPrice.toFixed(2)}
                        </Typography>
                    </Grid>
                    <Grid xs={4} item>
                        <Divider/>
                        <Typography variant="h6">
                            {t("total")}:
                        </Typography>
                    </Grid>
                    <Grid xs={8} item>
                        <Divider/>
                        <Typography variant="h6" className={classes.align_right}>
                            €{(totalPrice + deliveryPrice).toFixed(2)}
                        </Typography>
                    </Grid>
                </Grid>
                <Button variant="outlined" size="large" className={classes.delivery_button} onClick={() => finishOrder()}>
                    {t("finish_order")}
                    <LocalShippingIcon className={classes.margin_left}/>
                </Button>
            </Box>
        </Drawer>
    </>
}

