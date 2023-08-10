import React from "react";
import { Box, Button, Divider, Drawer, Grid, Hidden, Typography, IconButton } from "@mui/material";
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import { useNavigate } from "react-router-dom";
import { ICookieProduct } from "../../interfaces/ICookieProduct";
import CartItem from "./CartItem";
//import CartState from "../../store/cart/CartState";
import { useSelector } from "react-redux";
import { useTranslation } from "react-i18next";
import SimpleToolTip from "../tooltip/SimpleToolTip";
//import CartDrawerStyles from "./CartDrawerStyles";
import DeleteIcon from '@mui/icons-material/Delete';
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos';
import CloseIcon from '@mui/icons-material/Close';

const styles = {
    total_height: {
        height: "100%"
    },
    delivery_box: {
        bottom: "0%",
        position: "fixed",
        backgroundColor: "#FFFFFF"
    },
    delivery_button: {
        margin: "20px",
        width: "-webkit-fill-available",
        color: "black",
        borderColor: "black"
    },
    align_right: {
        textAlign: "right",
    },
    margin_left: {
        marginLeft: "20px"
    }
}

interface IProps {
    cartOpen: boolean;
    setCartOpen: (value:boolean) => void;
}

export default ({cartOpen, setCartOpen}:IProps) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    //const { items }:CartState = useSelector((state:any) => state.cart);
    const items: ICookieProduct[] = [];


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
        navigate("/checkout")
        setCartOpen(false);
    }
    
    return <>
        <Drawer 
            anchor="right" 
            open={cartOpen} 
            sx={styles.total_height}
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
            <Box sx={styles.delivery_box}>
                <Grid container style={{padding:"10px"}}>
                    <Grid xs={4} item>
                        <Typography variant="h6">
                            {t("price")}:
                        </Typography>
                    </Grid>
                    <Grid xs={8} item>
                        <Typography variant="h6" sx={styles.align_right}>
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
                        <Typography variant="h6" sx={styles.align_right}>
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
                        <Typography variant="h6" sx={styles.align_right}>
                            €{(totalPrice + deliveryPrice).toFixed(2)}
                        </Typography>
                    </Grid>
                </Grid>
                <Button variant="outlined" size="large" sx={styles.delivery_button} onClick={() => finishOrder()}>
                    {t("finish_order")}
                    <LocalShippingIcon sx={styles.margin_left}/>
                </Button>
            </Box>
        </Drawer>
    </>
}

