import * as React from 'react';
import { Box, Button, Container, Paper } from '@material-ui/core';
import { useHistory } from 'react-router';
import { useDispatch, useSelector } from 'react-redux';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import Header from '../../../components/header/Header';
import CheckoutStepper from '../components/CheckoutStepper';
import PaidForm from '../components/PaidForm';
import ProgressBox from '../../../components/progress/ProgressBox';
import CheckoutState from '../../../store/checkout/CheckoutState';
import ErrorForm from '../components/ErrorForm';
import { HashValues } from '../../../i18n/HashValues';
import { CheckOrder } from '../../../store/checkout/CheckoutActions';
import { GetPaymentCookie } from '../../../store/localStorageManager';
import PaymentReturnStyle from './PaymentReturnStyle';

const isAdmin = true;


export default () => {
    const history = useHistory();
    const dispatch = useDispatch();
    const classes = PaymentReturnStyle();
    const {t} = useTranslation();
    const { type, loading, valid_payment }:CheckoutState = useSelector((state:any) => state.checkout)
    const [paymentId, setPaymentId] = useState("");

    const to = (location:string) => () => {
        history.push(location)
    };

    useEffect(() => {
        if(!loading && !type)
        {
            const payment = GetPaymentCookie();
            if(payment && payment.orderId)
            {
                setPaymentId(payment.orderId);
                dispatch(CheckOrder(payment.orderId));
            }
        }
    })
    
    return <>
        <Header isAdmin={isAdmin}/>
        <Container component="main" maxWidth="sm" className={classes.container}>
            <Paper variant="outlined" className={classes.paper}>
                <CheckoutStepper currentStep={2} onError={!valid_payment}/>
                {loading ? <ProgressBox/> : valid_payment ? <PaidForm /> : <ErrorForm paymentId={paymentId}/>}
                <Box className={classes.btn_box}>
                    { !valid_payment && 
                        <Button variant="outlined" onClick={to(`/${HashValues.contact}`)} className={classes.btn}>
                            {t("contact")}
                        </Button>
                    }
                    <Button variant="outlined" onClick={to("/products")} className={classes.btn}>
                        {t("back_to_products")}
                    </Button>
                </Box>
            </Paper>
        </Container> 
  </>
}
