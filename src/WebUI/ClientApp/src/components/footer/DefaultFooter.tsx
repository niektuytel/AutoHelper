import React, { useEffect } from "react";
import { Box, Container, Grid, Hidden, MenuItem, Select, Typography } from "@mui/material";
import { SxProps, Theme } from "@mui/system";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

import { HashValues } from "../../i18n/HashValues";
import CopyRight from "./components/CopyRight";
import useCookies from "react-cookie/cjs/useCookies";
import { NL, EN } from "../../i18n/LanguageKeys";
import TypedLogo from "../logo/TypedLogo";

const styles = {
    contactTitle: {
        cursor: "pointer",
        color: "black"
    },
    center: {
        textAlign: "center"
    },
    imageHeight: {
        height: "50px"
    }
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
                {/*<Grid container style={{marginTop:"30px"}}>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/melon.svg"} style={styles.imageHeight} />*/}
                {/*    </Grid>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/arugula.svg"} style={styles.imageHeight} />*/}
                {/*    </Grid>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item lg={1} sx={styles.center}>*/}
                {/*            <img src={"/background/avacado.svg"} style={styles.imageHeight}/>*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/broccoli.svg"} style={styles.imageHeight}/>*/}
                {/*    </Grid>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item lg={1} sx={styles.center}>*/}
                {/*            <img src={"/background/cauliflower.svg"} style={styles.imageHeight}/>*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/chayote.svg"} style={styles.imageHeight}/>*/}
                {/*    </Grid>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item lg={1} sx={styles.center}>*/}
                {/*            <img src={"/background/spinach.svg"} style={styles.imageHeight}/>*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item lg={1} sx={styles.center}>*/}
                {/*            <img src={"/background/dill.svg"} style={styles.imageHeight}/>*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/leek.svg"} style={styles.imageHeight}/>*/}
                {/*    </Grid>*/}
                {/*    <Grid item xs={2} md={2} lg={1} sx={styles.center}>*/}
                {/*        <img src={"/background/pear.svg"} style={styles.imageHeight}/>*/}
                {/*    </Grid>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item lg={1} sx={styles.center}>*/}
                {/*            <img src={"/background/fennel.svg"} style={styles.imageHeight}/>*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*    <Hidden mdDown>*/}
                {/*        <Grid item sx={styles.center}>*/}
                {/*            <img src={"/background/lettuce.svg"} style={styles.imageHeight} />*/}
                {/*        </Grid>*/}
                {/*    </Hidden>*/}
                {/*</Grid>*/}
                <Box ml={4} mr={4} mb={4}>
                    <Container maxWidth="lg">
                        <Grid container spacing={5}>
                            <Grid item xs={12} sm={4}>
                                <Typography variant="h6" component="h6">
                                    <Link style={styles.contactTitle} to={`/${HashValues.contact}`}>
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