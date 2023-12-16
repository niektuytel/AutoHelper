import React, { useEffect } from "react";
import { Box, Container, Grid, Hidden, MenuItem, Select, Typography } from "@mui/material";
import { SxProps, Theme } from "@mui/system";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

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
    // TODO: There is an margin overflow on footer and body
    return (
        <footer>
            <Container maxWidth="lg" style={{ padding: "0" }}>
                <Box ml={4} mr={4}>
                    <Grid container spacing={5}>
                        <Grid item xs={12} sm={4}>
                            <Typography variant="h6" component="h6">
                                <Link style={styles.contactTitle} to={`/#contact`}>
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
                                <Link to={`/about`}>
                                    {t("our_company")}
                                </Link>
                            </Box>
                            <Box>
                                <Link to={`/about#info`}>
                                    {t("development")}
                                </Link>
                            </Box>
                            <Box>
                                <Link to={`/about#conditions`}>
                                    {t("our_conditions")}
                                </Link>
                            </Box>
                        </Grid>
                    </Grid>
                </Box>
                <CopyRight/>
            </Container>
        </footer>
    )
}

export default Footer;