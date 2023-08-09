import * as React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';
import { Box, Button, FormControl, InputLabel, makeStyles, MenuItem, Select } from '@material-ui/core';

import CartState from '../../../store/cart/CartState';
import CheckoutState from '../../../store/checkout/CheckoutState';
import { ICookieProduct } from '../../../interfaces/ICookieProduct';
import { CreatePayment, SetCheckoutStep, SetMethod } from '../../../store/checkout/CheckoutActions';
import { DeliveryPrice, FreeDeliveryPriceThreshold } from '../../../i18n/ProductValues';
import { useState } from 'react';

const PaymentFormStyle = makeStyles((theme:any) => ({
    box_style: {
        textAlign:"center",
        marginTop:"50px",
        marginBottom:"50px"
    },
    formControl: {
        margin: theme.spacing(1),
        minWidth: 120,
    },
    selectEmpty: {
        marginTop: theme.spacing(2),
    },
    btn_box: { 
        display: 'flex', 
        justifyContent: 'flex-end', 
        paddingTop:"5px" 
    },
    btn_back: { 
        marginTop: 1, 
        marginLeft: 1 
    },
    btn_next: { 
        marginTop: 1, 
        marginLeft: 1 
    }
}));

export default () => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const classes = PaymentFormStyle();
    const {currentStep, method, delivery}:CheckoutState = useSelector((state:any) => state.checkout);
    const { items }:CartState = useSelector((state:any) => state.cart);
    const [buttonText, setButtonText] = useState<string>(t("confirm"));

    var totalPrice = 0;
    items.forEach((item:ICookieProduct) => {
        totalPrice += (item.quantity * item.currentType.price);
    })

    if(totalPrice < FreeDeliveryPriceThreshold)
    {
        totalPrice += DeliveryPrice
    }

    const handleNext = () => {
        switch (currentStep) {
            case 0:
                dispatch(SetCheckoutStep(currentStep + 1));
                break;
            case 1:
                setButtonText(t("loading"))
                dispatch(CreatePayment(delivery, method));
                break;
        }
    };

    const handleBack = () => {
        dispatch(SetCheckoutStep(currentStep - 1));
    };

    const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
        dispatch(SetMethod(event.target.value as string));
    };

    return <>
        <form onSubmit={handleNext}>
            <Box className={classes.box_style}>
                <FormControl variant="outlined" className={classes.formControl}>
                    <InputLabel id="select-outlined-label">{t("payment_method")}</InputLabel>
                    <Select
                        labelId="select-outlined-label"
                        id="select-outlined"
                        value={method}
                        onChange={handleChange}
                        label={t("payment_method")}
                    >
                        <MenuItem value={"ideal"}>iDEAL</MenuItem>
                    </Select>
                </FormControl>
            </Box>
            <Box style={{textAlign:"right"}}>
                {t("to_pay")}: €{totalPrice.toFixed(2)}
            </Box>
            <Box className={classes.btn_box}>
                { currentStep !== 0 && 
                    <Button 
                        className={classes.btn_back}
                        onClick={handleBack}
                    >
                        {t("back")}
                    </Button>
                }
                <Button 
                    className={classes.btn_next}
                    disabled={(buttonText === t("loading"))}
                    variant="outlined" 
                    type="submit"
                >
                    {buttonText}
                </Button>
            </Box>
        </form>
    </>
}
