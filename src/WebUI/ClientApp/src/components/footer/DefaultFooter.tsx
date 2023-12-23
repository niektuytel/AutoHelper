import React, { useEffect } from "react";
import { Box, Container, Grid, MenuItem, Select } from "@mui/material";
import { useTranslation } from "react-i18next";
import useCookies from "react-cookie/cjs/useCookies";
import { NL, EN } from "../../i18n/LanguageKeys";
import TypedLogo from "../logo/TypedLogo";
import CopyRight from "./components/CopyRight";

const Footer = () => {
    const [cookies, setCookie] = useCookies(['language']);
    const { t, i18n } = useTranslation();

    useEffect(() => {
        const language = cookies.language || NL;
        if (language !== i18n.language) {
            i18n.changeLanguage(language);
        }
    }, [cookies.language, i18n.language]); // Add dependencies to useEffect

    const handleChange = (event: any) => {
        setCookie("language", event.target.value, { path: '/' });
        i18n.changeLanguage(event.target.value);
    };

    return (
        <footer>
            <Container maxWidth="lg" style={{ backgroundColor: "#f7f7f7", padding: "20px" }}>
                <Box ml={4} mr={4}>
                    {/* Use flexbox on the container Grid to center the items */}
                    <Grid container justifyContent="center" alignItems="center" style={{ height: '100%' }}>
                        {/* Set Grid item to use flex and center content horizontally and vertically */}
                        <Grid item xs={12} sm={6} style={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                            <TypedLogo small />
                            <Select
                                style={{ marginLeft: '10px' }}
                                value={cookies.language || NL}
                                onChange={handleChange}
                                size="small"
                            >
                                <MenuItem value={NL}>{t("dutch")}</MenuItem>
                                <MenuItem value={EN}>{t("english")}</MenuItem>
                            </Select>
                        </Grid>
                    </Grid>
                </Box>
                <CopyRight />
            </Container>
        </footer>
    )
}

export default Footer;
