import { Box, makeStyles, MenuItem, Select, Typography } from "@material-ui/core";
import React from "react"
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router";
import { TypedIconStyle } from "../../../../components/logo/TypedLogo";
import { HashValues } from "../../../../i18n/HashValues";
import { GotoProductsStyle } from "../../AboutStyle";
import Company from "./sections/company/Company";
import Delivery from "./sections/delivery(deprecated)/Delivery";
import PaymentMethods from "./sections/payments-methods(deprecated)/PaymentMethods";
import TermsAndConditions from "./sections/terms-and-conditions/TermsAndConditions";


const TypeStyle = makeStyles(() => ({
    root: {
        color:"black", 
        fontFamily:"'Nunito', sans-serif",
    }
})) 

interface IProps {
    hash: string;
}

const About = ({ hash }: IProps) => {
    const classes = TypedIconStyle();
    const { t } = useTranslation();
    const history = useHistory();
            

    return <>
    
        {hash.startsWith(HashValues.about) && 
            <Select
                value={hash}
                onChange={(a) => history.push(`/${a.target.value as string}`)}
                style={{marginLeft: "15px"}}
                displayEmpty
            >
                <MenuItem value={HashValues.about}>
                    <Typography 
                        // variant={"h5"} 
                        className={classes.root}
                        style={{
                            fontSize: "2vmax"
                        }}
                    >
                        {t("about")} {t("our_company")}
                    </Typography>
                </MenuItem>
                {/* <MenuItem value={HashValues.about_delivery}>{t("our_delivery")}</MenuItem>
                <MenuItem value={HashValues.about_payment}>{t("our_payments")}</MenuItem> */}
                <MenuItem value={HashValues.about_conditions}>
                    
                    <Typography 
                        // variant={"h5"} 
                        className={classes.root}
                        style={{
                            fontSize: "2vmax"
                        }}
                    >
                        {t("our_conditions")}
                    </Typography>
                </MenuItem>
            </Select>
        }

        {hash == HashValues.about && <Company />}
        {/* privacy policy */}
        {/* {hash == HashValues.about_delivery && <Delivery />} */}
        {/* {hash == HashValues.about_payment && <PaymentMethods />} */}
        {hash == HashValues.about_conditions && <TermsAndConditions />}
    </>
}

export default About;
