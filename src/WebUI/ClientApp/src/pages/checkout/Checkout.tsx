import * as React from 'react';
import { useSelector } from 'react-redux';
import { Container, Paper } from '@material-ui/core';

import CheckoutState from '../../store/checkout/CheckoutState';
import PaymentReturn from './sections/PaymentReturn';
import CheckoutStepper from './components/CheckoutStepper';
import ShippingForm from './components/ShippingForm';
import PaymentForm from './components/PaymentForm';
import CheckoutStyle from './CheckoutStyle';

export default () => {
    const classes = CheckoutStyle();
    const {currentStep}:CheckoutState = useSelector((state:any) => state.checkout);
    
    const EnterForm = () => {
        switch (currentStep) {
            case 0:
                return <ShippingForm/>
            case 1:
                return <PaymentForm />
            default:
                return <></>
        }
    }
    
    if(location.search.startsWith("?redirect")) 
    {
        return <PaymentReturn/>
    }

    return <>
        <Container component="main" maxWidth="sm" className={classes.container}>
            <Paper variant="outlined" className={classes.paper}>
                <CheckoutStepper currentStep={currentStep} onError={false}/>
                {EnterForm()}
            </Paper>
        </Container> 
    </>
}
