import { Box, Typography } from "@material-ui/core";
import React from "react";
import TypedLogo from "../../../../../../components/logo/TypedLogo";

const Delivery = () => {
    return <>
        <Typography style={{padding:"50px", paddingTop:"20px", paddingBottom:"20px", textAlign:"start"}}>
            {/* U heeft de keuze uit de volgende betaalmethode:<br/>
            <li><b>IDeal</b></li>
            <br/>
            <b>Betalen met iDeal</b><br/>
            IDeal is een van de meest bekende betaalmethodes in Nederland en is voor iedereen in Nederland gratis beschikbaar. Met deze betaalmethode rekent u uw bestelling ter plekke af binnen de vertrouwde internetomgeving van uw eigen bank. Wanneer u het aankoopbedrag bevestigd wordt het gelijk van uw bankrekening afgeschreven. Vervolgens wordt u weer teruggeleid naar onze website. Uw bestelling en betaling zijn geslaagd. */}
            Wanneer u de pakking open maakt, raden wij u aan om het droog onder de 18 graden te bewaren.
            
            Verzending
            Wanneer wij uw bestelling en betaling op werkdagen voor 15:00 ’s middags in goede orde hebben ontvangen starten wij met het samenstellen van uw pakket. Uw bestelling wordt dan de gekozen dagen van Dinsdag, Donderdag of Zaterdag verzonden.

            Wij bezorgen het self in de buurt en voor buiten de buurt verturen wij via PostNL onze pakketten.

            Verzendkosten
            Verzendkosten Nederland:

            € 3,50 tot een gewicht van 500 gram
            € 7,50 vanaf het gewicht van 500 gram
            Gratis verzending vanaf € 30,-

            <Box style={{padding:"30px", textAlign:"center"}}>
                <TypedLogo very_large/>
            </Box>
        </Typography>
    </>
}

export default Delivery;