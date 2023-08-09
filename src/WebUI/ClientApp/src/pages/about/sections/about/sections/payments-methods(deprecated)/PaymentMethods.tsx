import { Box, Typography } from "@material-ui/core";
import React from "react";
import TypedLogo from "../../../../../../components/logo/TypedLogo";

const PaymentMethods = () => {
    return <>
        <Typography style={{padding:"50px", paddingTop:"20px", paddingBottom:"20px", textAlign:"start"}}>
            U heeft de keuze uit de volgende betaalmethode:<br/>
            <li><b>IDeal</b></li>
            <br/>
            <b>Betalen met iDeal</b><br/>
            IDeal is een van de meest bekende betaalmethodes in Nederland en is voor iedereen in Nederland gratis beschikbaar. Met deze betaalmethode rekent u uw bestelling ter plekke af binnen de vertrouwde internetomgeving van uw eigen bank. Wanneer u het aankoopbedrag bevestigd wordt het gelijk van uw bankrekening afgeschreven. Vervolgens wordt u weer teruggeleid naar onze website. Uw bestelling en betaling zijn geslaagd.
            
            <Box style={{padding:"30px", textAlign:"center"}}>
                <TypedLogo very_large/>
            </Box>
        </Typography>
    </>
}



export default PaymentMethods;