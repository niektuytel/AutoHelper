import React from "react";
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";

interface IProps {
}

export default ({ }: IProps) => {
    // splittedPath looks like "[lat,lng, licence_plate, ...]"
    const splittedPath = window.location.pathname.split('/').filter(x => x.length > 0).slice(1);
    
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
                TODO: show garages
            </Container>
        </Box>
    );
}
