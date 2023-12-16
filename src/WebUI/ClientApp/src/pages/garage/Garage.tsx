import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, ButtonGroup, CircularProgress, Container, Divider, Grid, IconButton, Pagination, Paper, Skeleton, TextField, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';

// local
import { GarageLookupDtoItem, GarageServiceDtoItem, PaginatedListOfGarageLookupBriefDto, SelectedService } from "../../app/web-api-client";
import useGarage from "./useGarage";
import Header from "../../components/header/Header";
import GarageServiceInfoCard from "./components/GarageServiceInfoCard";
import { showOnError, showOnSuccess } from "../../redux/slices/statusSnackbarSlice";
import { useDispatch, useSelector } from "react-redux";
import GarageDailySchedule from "./components/GarageDailySchedule";
import GarageContactSection from "./components/GarageContactSection";
import GarageContactDialog from "../../components/GarageContactDialog";
import { addService } from "../../redux/slices/storedServicesSlice";
import { ROUTES } from "../../constants/routes";

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const location = useLocation();
    const navigate = useNavigate();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const dispatch = useDispatch();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [relatedServiceTypes, setRelatedServiceTypes] = useState<GarageServiceDtoItem[]>([]);
    const [selectedItem, setSelectedItem] = useState<GarageServiceDtoItem | null>(null);
    const queryParams = new URLSearchParams(location.search);
    const { identifier } = useParams();
    const licensePlate = queryParams.get('licensePlate');
    const lat = queryParams.get('lat');
    const lng = queryParams.get('lng');

    const { loading, garageLookup, fetchGarageLookupByPlate } = useGarage(identifier!, licensePlate);
    const services: SelectedService[] = useSelector((state: any) => state.storedServices);

    const hasQuestionItem = (serviceType: GarageServiceDtoItem) => {
        setRelatedServiceTypes([ serviceType ]);
        setDialogOpen(true);
    };

    // TODO: implement when using converstations
    const tryAddCartItem = (service: SelectedService) => {
        //service.relatedGarageLookupIdentifier = garageLookup?.identifier!;
        //service.relatedGarageLookupName = garageLookup?.name;

        //if (services.some(item => (
        //    item.relatedGarageLookupIdentifier === service.relatedGarageLookupIdentifier &&
        //    item.relatedServiceType === service.relatedServiceType
        //))) {
        //    dispatch(showOnError(t("Cart item already exist")));
        //    return;
        //}

        //service.conversationContactEmail = garageLookup?.conversationContactEmail;
        //service.conversationContactWhatsappNumber = garageLookup?.conversationContactWhatsappNumber;
        //service.vehicleLicensePlate = licensePlate!;
        //service.vehicleLatitude = lat!;
        //service.vehicleLongitude = lng!;

        //dispatch(addService(service));
    }

    if (!loading && garageLookup?.garageId)
    {
        // TODO: handle garage specific page
    }
    const imageUrl = `http://127.0.0.1:10000/devstoreaccount1/garage-images/${garageLookup?.image}`;
    const showConversation = garageLookup?.conversationContactEmail !== null || garageLookup?.conversationContactWhatsappNumber !== null;
    return <>
        <Header garageLookupIsLoading={loading} garageLookup={garageLookup} showStaticDrawer={false} navigateGoto={() => navigate(-1)} />
        <Container sx={{ mb: 5 }}>
            <Box pt={1} pb={2}>
                <Paper
                    style={{
                        backgroundSize: 'cover',
                        backgroundPosition: 'left center',
                        backgroundRepeat: 'no-repeat',
                        minWidth: '100%',
                        minHeight: '400px'
                    }}
                >
                    <img
                        src={imageUrl}
                        alt="Garage"
                        style={{
                            width: '100%',
                            height: '100%',
                            objectFit: 'cover'
                        }}
                    />
                </Paper>
            </Box>
            <Grid container spacing={1}>
                <Grid item xs={12} md={8} pr={isMobile ? 0 : 2}>
                    <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                        {t("Services")}
                        <Tooltip title={t("Services.Description")}>
                            <IconButton size="small">
                                <InfoOutlinedIcon fontSize="inherit" />
                            </IconButton>
                        </Tooltip>
                    </Typography>
                    {loading ?
                        <>
                            <Skeleton variant="rounded" width="100%" height="90px" sx={{ mb:2 }}  />
                            <Skeleton variant="rounded" width="100%" height="90px" sx={{ mb:2 }} />
                            <Skeleton variant="rounded" width="100%" height="90px" sx={{ mb:2 }} />
                        </>
                        :
                        garageLookup?.services?.map((item) =>
                            <GarageServiceInfoCard
                                key={`service-card-${item.id}`}
                                service={item}
                                showConversationActions={showConversation}
                                selectedItem={selectedItem}
                                setSelectedItem={setSelectedItem}
                                addCartItem={tryAddCartItem}
                                hasQuestionItem={hasQuestionItem}
                            />
                        )
                    }
                </Grid>
                <Grid item xs={12} md={4}>
                    <GarageContactSection
                        loading={loading}
                        garageLookup={garageLookup}
                    />
                </Grid>  
            </Grid>
        </Container>
    </>;

    //{garageLookup && relatedServiceTypes &&
    //    <GarageContactDialog
    //        services={[new SelectedService({
    //            relatedServiceType: relatedServiceTypes![0],
    //            relatedGarageLookupIdentifier: garageLookup?.identifier!,
    //            relatedGarageLookupName: garageLookup?.name,
    //            conversationContactEmail: garageLookup?.conversationContactEmail,
    //            conversationContactWhatsappNumber: garageLookup?.conversationContactWhatsappNumber,
    //            vehicleLicensePlate: licensePlate!,
    //            vehicleLatitude: lat!,
    //            vehicleLongitude: lng!
    //        })]}
    //        open={dialogOpen}
    //        onClose={() => setDialogOpen(false)}
    //    />
    //}
}
