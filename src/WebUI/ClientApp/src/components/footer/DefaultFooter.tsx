import { Box, Container, Grid, Hidden, MenuItem, Select, Typography } from "@material-ui/core";
import { CSSProperties } from "@material-ui/core/styles/withStyles";
import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

import { HashValues } from "../../i18n/HashValues";
import CopyRight from "./components/CopyRight";
import useCookies from "react-cookie/cjs/useCookies";
import { NL, EN } from "../../i18n/LanguageKeys";
import TypedLogo from "../logo/TypedLogo";

const ContactTitleStyle: CSSProperties = {
    cursor: "pointer",
    color: "black"
}

const CenterStyle: CSSProperties = {
    textAlign: "center"
}

const ImageHeightStyle: CSSProperties = {
    height: "50px"
}

const Footer = () => {
    const [cookies, setCookie] = useCookies(['language']);
    const { t, i18n } = useTranslation();

    useEffect(() => {
        if(cookies.language !== i18n.language)
        {
            const language = cookies.language !== undefined ? cookies.language : NL;
            i18n.changeLanguage(language);
        }
    }, [])

    const handleChange = (event: any) => {
        setCookie("language", event.target.value)
        i18n.changeLanguage(event.target.value);
    };
    
    return (
        <footer>
            <Container maxWidth="lg" style={{padding:"0"}}>
                <Grid container style={{marginTop:"30px"}}>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/melon.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/arugula.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Hidden mdDown>
                        <Grid item lg={1} style={CenterStyle}>
                            <img src={"/background/avacado.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/broccoli.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Hidden mdDown>
                        <Grid item lg={1} style={CenterStyle}>
                            <img src={"/background/cauliflower.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/chayote.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Hidden mdDown>
                        <Grid item lg={1} style={CenterStyle}>
                            <img src={"/background/spinach.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                    <Hidden mdDown>
                        <Grid item lg={1} style={CenterStyle}>
                            <img src={"/background/dill.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/leek.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Grid item xs={2} md={2} lg={1} style={CenterStyle}>
                        <img src={"/background/pear.svg"} style={ImageHeightStyle}/>
                    </Grid>
                    <Hidden mdDown>
                        <Grid item lg={1} style={CenterStyle}>
                            <img src={"/background/fennel.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                    <Hidden mdDown>
                        <Grid item style={CenterStyle}>
                            <img src={"/background/lettuce.svg"} style={ImageHeightStyle}/>
                        </Grid>
                    </Hidden>
                </Grid>
                <Box ml={4} mr={4} mb={4}>
                    <Container maxWidth="lg">
                        <Grid container spacing={5}>
                            <Grid item xs={12} sm={4}>
                                <Typography variant="h6" component="h6">
                                    <Link style={ContactTitleStyle} to={`/${HashValues.contact}`}>
                                        {t("contact")}
                                    </Link>
                                </Typography>
                                <Box>
                                    <a href={`tel:${t("autohelper_phone")}`}>{t("autohelper_phone")}</a>
                                </Box>
                                <Box>
                                    <a href={`mailto:${t("autohelper_email")}`}>{t("autohelper_email")}</a>
                                </Box>
                                <Box>{t("autohelper_street")}</Box>
                                <Box>{t("autohelper_city")}, {t("autohelper_postalcode")}</Box>
                            </Grid>
                            <Grid 
                                style={{textAlign:"center"}}
                                item 
                                xs={12} 
                                sm={4}
                            >
                                <TypedLogo large/>
                                <Select
                                    value={cookies.language === undefined ? NL : cookies.language}
                                    onChange={handleChange}
                                >
                                    <MenuItem value={NL}>{t("dutch")}</MenuItem>
                                    <MenuItem value={EN}>{t("english")}</MenuItem>
                                </Select>
                            </Grid>
                            <Grid 
                                style={{textAlign:"right"}}
                                item xs={12} sm={4}>
                                <Typography variant="h6" component="h6">
                                    {t("information")}
                                </Typography>
                                <Box>
                                    <Link to={`/about${HashValues.about}`}>
                                        {t("our_company")}
                                    </Link>
                                </Box>
                                <Box>
                                    <Link to={`/about${HashValues.info}`}>
                                        {t("development")}
                                    </Link>
                                </Box>
                                <Box>
                                    <Link to={`/about${HashValues.about_conditions}`}>
                                        {t("our_conditions")}
                                    </Link>
                                </Box>
                            </Grid>
                        </Grid>
                    </Container>
                </Box>
                <CopyRight/>
            </Container>
        </footer>
    )
}

export default Footer;