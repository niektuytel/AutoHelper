import { Box, Typography } from "@material-ui/core";
import React from "react";
import TypedLogo from "../../../../../../components/logo/TypedLogo";

const Company = () => {
    return <>
        <Typography style={{padding:"50px", paddingTop:"20px", paddingBottom:"20px", textAlign:"start"}}>
            Wij zijn een klein start-up bedrijf wat streeft naar een gezondere samenleving.<br/>
            Er zijn 2 dingen waar wij aandacht aan willen geven:
            <li><b>Handig en makkelijk</b></li>
            <li><b>Gezondere oplossing</b></li>

            <br/>
            <b>Handig en makkelijk</b><br/>
            Alles wat makkelijk te eten is. Is meestal ongezond, omdat er teveel van gegeten wordt.<br/>
            Fruit en groente is niet een makkelijk product om mee te nemen of op te eten. 
            Het is vochtig, bederf snel en je heb altijd rest afval na het eten.
            Wij nemen het vocht en rest afval uit het product zodat je makkelijk iets gezonds kan eten.
            
            <br/>
            <br/>

            <b>Gezondere oplossing</b><br/>
            Over de laatste 100 jaar zijn al de vitamine en miniralen die nu bestaan ondekt en over tijd ook aangepast en nieuwe aan toegevoegd.
            Er gebeuren nog steeds veel ontwikkelingen op dit gebied.
            Als je Vitamine pillen koopt worden deze meestal gemaakt met een gemische process wat niet natuurlijk is en daar zit alleen de vitamine in waarvan wij weten dat dit een goed effect heeft op je lichaam.
            Fruit en groente bevatten ook deze vitamine en miniralen maar bevatten ook dingen die belangerijk zijn voor je lichaam waarvan wij nog niet weten wat het doet.
            
            <br/>
            <br/>
            
            De droom van autohelper is dat wij zoveel mogelijk informatie ontdekken over de werking van natuur met het menselijk lichaam.
            En Het gaat hier niet om groot worden of aanzien hebben, maar een eerlijk beeld schetsen met alle dingen die wij weten over elk soort fruit of groente soort.
            En hiermee willen wij ziektes en tekort koming in een menselijk lichaam willen toevoegen met natuurlijke producten.
            
        </Typography>
        <Box style={{padding:"30px", textAlign:"center"}}>
            <Typography style={{padding:"10px", marginLeft:"10px", marginRight:"10px"}}>
                Met vriendelijke groet,<br/>
                <b>Niek Tuytel</b>
            </Typography>
            {/* <TypedLogo very_large/> */}
        </Box>
    </>
}

export default Company;