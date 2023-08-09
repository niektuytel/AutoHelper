import * as React from 'react';
import Grid from '@material-ui/core/Grid';
import TextField from '@material-ui/core/TextField';
import { useDispatch, useSelector } from 'react-redux';
import CheckoutState from '../../../store/checkout/CheckoutState';
import IDelivery from '../../../interfaces/IDelivery';
import { SetCheckoutStep, UpdateDelivery } from '../../../store/checkout/CheckoutActions';
import { useTranslation } from 'react-i18next';
import { Box, Button, makeStyles } from '@material-ui/core';
import { useState } from 'react';

const useStyles = makeStyles(() => ({
    container: {
        marginBottom:"100px"
    },
    paper: {
        padding:"15px"
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
    const dispatch = useDispatch();
    const {t} = useTranslation();
    const classes = useStyles();
    const {currentStep, method, delivery}:CheckoutState = useSelector((state:any) => state.checkout);
    const [invalidItems, setInvalidItems] = useState<string[]>([]);

    const validInput = ():boolean => {
        let errors:string[] = [];

        if (delivery.firstName.length === 0) errors.push("firstName");
        if (delivery.lastName.length === 0) errors.push("lastName");
        if (delivery.email.length === 0) errors.push("email");
        if (delivery.address.length === 0) errors.push("address");
        if (delivery.city.length === 0) errors.push("city");
        if (delivery.postalCode.length === 0) errors.push("postalCode");

        setInvalidItems(errors);
        return errors.length === 0;
    }

    const onChange = (data:IDelivery) => {
        dispatch(UpdateDelivery(data))
    }
    
    const handleNext = () => {
        if(validInput())
        {
            dispatch(SetCheckoutStep(currentStep + 1));
        }
    };

    const handleBack = () => {
        dispatch(SetCheckoutStep(currentStep - 1));
    };
    
    return <>
        <form onSubmit={handleNext}>
            <Grid container spacing={3} style={{marginBottom:"20px"}}>
                <Grid item xs={12} sm={6}>
                <TextField
                    required
                    error={invalidItems.includes("firstName")}
                    id="firstName"
                    name="firstName"
                    label={t("first_name")}
                    fullWidth
                    variant="standard"
                    autoComplete="home given-name"
                    value={delivery.firstName}
                    onChange={(event:any) => onChange({...delivery, firstName:event.target.value})}
                />
                </Grid>
                <Grid item xs={12} sm={6}>
                <TextField
                    required
                    error={invalidItems.includes("lastName")}
                    id="lastName"
                    name="lastName"
                    label={t("last_name")}
                    fullWidth
                    variant="standard"
                    autoComplete="home family-name"
                    value={delivery.lastName}
                    onChange={(event:any) => onChange({...delivery, lastName:event.target.value})}
                />
                </Grid>
                <Grid item xs={12}>
                <TextField
                    required
                    error={invalidItems.includes("email")}
                    id="email"
                    name="email"
                    label={t("email_address")}
                    fullWidth
                    variant="standard"
                    autoComplete="home email"
                    value={delivery.email}
                    onChange={(event:any) => onChange({...delivery, email:event.target.value})}
                />
                </Grid>
                <Grid item xs={12}>
                <TextField
                    required
                    error={invalidItems.includes("address")}
                    id="address"
                    name="address"
                    label={t("street_plus_number")}
                    fullWidth
                    variant="standard"
                    autoComplete="home address-line1"
                    value={delivery.address}
                    onChange={(event:any) => onChange({...delivery, address:event.target.value})}
                />
                </Grid>
                <Grid item xs={12} sm={6}>
                <TextField
                    required
                    error={invalidItems.includes("city")}
                    id="city"
                    name="city"
                    label={t("city")}
                    fullWidth
                    variant="standard"
                    autoComplete="home city"
                    value={delivery.city}
                    onChange={(event:any) => onChange({...delivery, city:event.target.value})}
                />
                </Grid>
                <Grid item xs={12} sm={6}>
                <TextField
                    required
                    error={invalidItems.includes("postalCode")}
                    id="zip"
                    name="zip"
                    label={t("postal_code")}
                    fullWidth
                    variant="standard"
                    autoComplete="home postal-code"
                    value={delivery.postalCode}
                    onChange={(event:any) => onChange({...delivery, postalCode:event.target.value})}
                />
                </Grid>
            </Grid>
            <Box className={classes.btn_box}>
                {currentStep !== 0 && 
                    <Button 
                        className={classes.btn_back} 
                        onClick={handleBack} 
                    >
                        {t("back")}
                    </Button>
                }
                <Button 
                    className={classes.btn_next} 
                    variant="outlined" 
                    type="submit" 
                >
                    {t("confirm")}
                </Button>
            </Box>
        </form>
    </>
    
}
