import React, { useEffect } from "react";
import { Box, Button, Container, Dialog, DialogContent, DialogContentText, DialogTitle, Divider, Drawer, FormControl, Hidden, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Paper, TextField, Toolbar, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { BankingInfoItem, BusinessOwnerItem, ContactItem, GarageClient, GarageSettings, IGarageSettings, LocationItem } from "../../../app/web-api-client";
import GetGarageClient from "../../../app/garageClient";
import { useAuth0 } from "@auth0/auth0-react";
import LocationSection from "./components/LocationSection";
import BusinessOwnerSection from "./components/BusinessOwnerSection";
import BankingInfoSection from "./components/BankingInfoSection";
import InboxIcon from '@mui/icons-material/MoveToInbox';
import MailIcon from '@mui/icons-material/Mail';
import RoleBasedList from "../../../components/header/components/RoleBasedList";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const { garage_guid } = useParams();
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const notFound = queryParams.get('garage_notfound');
    const [isLoading, setIsLoading] = React.useState<boolean>(false);
    const initialGarageSettings = new GarageSettings({
        name: "",
        location: new LocationItem(),
        businessOwner: new BusinessOwnerItem(),
        bankingDetails: new BankingInfoItem(),
        contacts: []
    });

    const [garageSettings, setGarageSettings] = React.useState<GarageSettings>(initialGarageSettings);
    const [openDialog, setOpenDialog] = React.useState<boolean>(Boolean(notFound));
    const { getAccessTokenSilently } = useAuth0();

    const handleCloseDialog = () => {
        setOpenDialog(false);
    };

    const handleLoading = async () => {
        if (notFound) return;
        setIsLoading(true);


        var accessToken = await getAccessTokenSilently();
        const garageClient = GetGarageClient(accessToken);
        garageClient.settings(garage_guid!)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);
                    setGarageSettings(response);
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                console.error("Error occurred:", error);

                // TODO: trigger snackbar
            })
            .finally(() => {
                setIsLoading(false);
            });
    }

    useEffect(() => {
        if (garageSettings === undefined) {
            handleLoading();
        }
    }, []);

    return (
        <>
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
                    <Paper variant="outlined" sx={{ top:"50px", padding: "25px" }} >
                        {isLoading ?
                            <Typography>Loading...</Typography>
                            :
                            <FormControl fullWidth >
                                <BusinessOwnerSection
                                    businessOwner={garageSettings.businessOwner}
                                    onChange={(updatedBusinessOwner) => {
                                        const newGarageSettings = garageSettings;
                                        newGarageSettings.businessOwner = updatedBusinessOwner;

                                        setGarageSettings(newGarageSettings);
                                    }}
                                />
                                <LocationSection
                                    location={garageSettings.location}
                                    onChange={(updatedLocation) => {
                                        const newGarageSettings = garageSettings;
                                        newGarageSettings.location = updatedLocation;

                                        setGarageSettings(newGarageSettings);
                                    }}
                                />
                                <BankingInfoSection
                                    bankingInfo={garageSettings.bankingDetails}
                                    onChange={(updatedBankingInfo) => {
                                        const newGarageSettings = garageSettings;
                                        newGarageSettings.bankingDetails = updatedBankingInfo;

                                        setGarageSettings(newGarageSettings);
                                    }}
                                />
                            </FormControl>
                        }
                    </Paper>
                </Container>
                <Dialog open={openDialog} onClose={handleCloseDialog}>
                    <DialogTitle>Welcome</DialogTitle>
                    <DialogContent>
                        <DialogContentText>
                            Welcome, you have no garage been defined yet. Let's create! :)
                        </DialogContentText>
                    </DialogContent>
                </Dialog>
            </Box>
        </>
    );
}
