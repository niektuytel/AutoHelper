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
    const { garageGUID } = useUserClaims();
    const navigate = useNavigate();
    const { t } = useTranslation();

    useEffect(() => {
        // redirect to garage overview if garage_guid is "login" and user is logged in
        if (garage_guid == "login" && garageGUID) {
            navigate(RoutesGarageOverview(garageGUID!));
        }
        else {
            // get overview of garage
        }
    }, [garageGUID]);

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
