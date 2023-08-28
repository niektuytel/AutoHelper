import React, { useEffect } from "react";
import { Box, Button, Container, Dialog, DialogContent, DialogContentText, DialogTitle, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { BankingInfoItem, BusinessOwnerItem, ContactItem, GarageClient, GarageSettings, IGarageSettings, LocationItem } from "../../../app/web-api-client";
import GetGarageClient from "../../../app/garageClient";
import { useAuth0 } from "@auth0/auth0-react";

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
                    <>
                        {garageSettings ? (
                            <form>
                                <TextField
                                    label="Name"
                                    variant="outlined"
                                    fullWidth
                                    value={garageSettings.name}
                                    onChange={(e) => {
                                        const updatedSettings = new GarageSettings(garageSettings);
                                        updatedSettings.name = e.target.value;
                                        setGarageSettings(updatedSettings);
                                    }}
                                    style={{ marginBottom: '20px' }}
                                />
                                <TextField
                                    label="Business Owner Name"
                                    variant="outlined"
                                    fullWidth
                                    value={garageSettings!.businessOwner!.fullName}
                                    onChange = {(e) => {
                                        var settings = garageSettings;
                                        settings.businessOwner!.fullName = e.target.value;

                                        setGarageSettings(settings);
                                    }}
                                    style={{ marginBottom: '20px' }}
                                />
                                <TextField
                                    label="Account Number"
                                    variant="outlined"
                                    fullWidth
                                    value={garageSettings.bankingDetails!.accountNumber}
                                    onChange={(e) => {
                                        var settings = garageSettings;
                                        settings.bankingDetails!.accountNumber = e.target.value;

                                        setGarageSettings(settings);
                                    }}
                                    style={{ marginBottom: '20px' }}
                                />
                                <TextField
                                    label="Bank Name"
                                    variant="outlined"
                                    fullWidth
                                    value={garageSettings.bankingDetails!.bankName}
                                    onChange={(e) => {
                                        var settings = garageSettings;
                                        settings.bankingDetails!.bankName = e.target.value;

                                        setGarageSettings(settings);
                                    }}
                                    style={{ marginBottom: '20px' }}
                                />
                                { garageSettings.contacts!.map((contact, index) => (
                                    <div key={index}>
                                        <TextField
                                            label={`Contact Name ${index + 1}`}
                                            variant="outlined"
                                            fullWidth
                                            value={contact.fullName}
                                            onChange={(e) => {
                                                const updatedContacts = [...garageSettings.contacts!];
                                                updatedContacts[index].fullName = e.target.value;

                                                var settings = garageSettings;
                                                settings.contacts = updatedContacts;
                                                setGarageSettings(settings);
                                            }}
                                            style={{ marginBottom: '20px' }}
                                        />
                                        <TextField
                                            label={`Contact Number ${index + 1}`}
                                            variant="outlined"
                                            fullWidth
                                            value={contact.phoneNumber}
                                            onChange={(e) => {
                                                const updatedContacts = [...garageSettings.contacts!];
                                                updatedContacts[index].phoneNumber = e.target.value;

                                                const updatedSettingsData = { ...garageSettings, contacts: updatedContacts };
                                                const newGarageSettings = new GarageSettings(updatedSettingsData);

                                                setGarageSettings(newGarageSettings);
                                            }}
                                            style={{ marginBottom: '20px' }}
                                        />
                                    </div>
                                ))}

                                <Button variant="contained" color="primary">
                                    Submit
                                </Button>
                            </form>
                        ) : (
                            <Typography>No data available.</Typography>
                        )}
                    </>
                }
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
    );
}
