import React from "react";
import { Box, Container } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import TextFieldPlaces from "./components/TextFieldPlaces";
import TextFieldLicencePlates from "./components/TextFieldLicencePlates";

interface IProps {
    isAdmin: boolean;
}

export default ({ isAdmin }: IProps) => {
    const path = window.location.pathname.split('/');
    // 3>= is that we are don't count the first empty string for a path like "/one/two"
    const showLicensePlate = path.length >= 3; 

    return (
        <Box
            style={{
                position: "relative",
                marginLeft: "10px",
                marginRight: "10px"
            }}
        >
            <Container
                maxWidth="lg"
                style={{
                    padding: "0",
                    textAlign: "center"
                }}
            >
                <Box
                    style={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center'
                    }}
                >
                    {showLicensePlate ? <TextFieldLicencePlates /> : <TextFieldPlaces />}
                </Box>
            </Container>
        </Box>
    );
}
