import React, { useEffect } from "react";
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import useUserClaims from "../../../hooks/useUserClaims";
import { RoutesGarageSettings } from "../../../constants/routes";
import { GarageClient, GarageOverview } from "../../../app/web-api-client";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [isLoading, setIsLoading] = React.useState<boolean>(false);
    const [garageOverview, setGarageOverview] = React.useState<GarageOverview | undefined>(undefined);
    const { garage_guid } = useParams();
    const { t } = useTranslation();
    const navigate = useNavigate();

    const handleLoading = () => {
        setIsLoading(true);

        garageClient.getOverview(garage_guid!)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);
                    setGarageOverview(response);
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                console.error("Error occurred:", error);

                if (error.status === 404) {
                    console.log('Garage not found:', error);
                    navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
                }
                else {
                    // TODO: trigger snackbar
                }
            })
            .finally(() => {
                setIsLoading(false);
            });
    }

    useEffect(() => {
        if (garageOverview === undefined) {
            handleLoading();
        }
    }, []);

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
                    <div>TODO: Garage overview</div>
                }
            </Container>
        </Box>
    );
}
