import React, { useEffect } from "react";
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import useUserClaims from "../../../hooks/useUserClaims";
import { RoutesGarageOverview } from "../../../constants/routes";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const { garage_guid } = useParams();
    const { t } = useTranslation();

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
                TODO: Garage overview
            </Container>
        </Box>
    );
}
