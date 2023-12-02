import React, { useEffect } from "react";
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../../../constants/routes";
import { GarageClient, GarageOverview } from "../../../app/web-api-client";
import { ROLES } from "../../../constants/roles";
import SimpleLineChart from "./components/ChartsLineBoard";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [isLoading, setIsLoading] = React.useState<boolean>(false);
    const [garageOverview, setGarageOverview] = React.useState<GarageOverview | undefined>(undefined);
    const { t } = useTranslation();
    const navigate = useNavigate();

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
                {isLoading ?
                    <Typography>Loading...</Typography>
                    :
                    <div><SimpleLineChart/></div>
                }
            </Container>
        </Box>
    );
}
