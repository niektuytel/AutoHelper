import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, ButtonGroup, CircularProgress, Container, Divider, Grid, IconButton, Pagination, Paper, Skeleton, TextField, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import LiveHelpIcon from '@mui/icons-material/LiveHelp';
import { useLocation, useNavigate, useParams } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import { COLORS } from "../../constants/colors";
import { GarageLookupDto, GarageServiceItem, GarageServiceItemDto, PaginatedListOfGarageLookupBriefDto } from "../../app/web-api-client";
import { useQueryClient } from "react-query";
import useGarage from "./useGarage";
import Header from "../../components/header/Header";
import GarageServiceInfoCard from "./components/GarageServiceInfoCard";
import { showOnError, showOnSuccess } from "../../redux/slices/statusSnackbarSlice";
import { useDispatch } from "react-redux";

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const location = useLocation();
    const theme = useTheme();
    const dispatch = useDispatch();
    const [selectedItem, setSelectedItem] = useState<GarageServiceItemDto|null>(null);
    const [cartItems, setCartItems] = useState<GarageServiceItemDto[]>([]);
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const queryParams = new URLSearchParams(location.search);
    const { identifier } = useParams();
    const licensePlate = queryParams.get('licensePlate');
    const lat = queryParams.get('lat');
    const lng = queryParams.get('lng');

    const { loading, garageLookup, fetchGarageLookupByPlate } = useGarage(identifier!, licensePlate);

    const handleContactClick = () => {
        console.log("handleContactClick");

        //setSelectedItem(undefined);
        //setDialogMode("create");
        //setDialogOpen(true);
    }

    const tryAddCartItem = (itemToAdd: GarageServiceItemDto) => {
        if (cartItems.some(cartItem => cartItem.id === itemToAdd.id)) {
            dispatch(showOnError(t("Cart item already exist")));
            return;
        }

        setCartItems([...cartItems, itemToAdd]);
    }

    const hasQuestionItem = (serviceItem: any) => {
        dispatch(showOnSuccess(t("Conversation.Started")));
    }

    hasQuestionItem

    if (!loading && garageLookup?.garageId)
    {
        // TODO: handle garage specific page
    }

    return <>
        <Header garageLookupIsLoading={loading} garageLookup={garageLookup} showStaticDrawer={false} />
        <Container>
            <Box pt={1} pb={2}>
                <Paper
                    style={{
                        backgroundImage: `url(data:image/jpeg;base64,${garageLookup?.largeData?.firstPlacePhoto})`,
                        backgroundSize: 'cover',
                        backgroundPosition: 'left center', // This line changed
                        backgroundRepeat: 'no-repeat',
                        minWidth: '100%',
                        minHeight: '400px'
                    }}
                />
            </Box>
            <Grid container spacing={1}>
                <Grid item xs={12} md={8} pr={isMobile ? 0 : 2}>
                    <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                        {t("Services")}
                        {loading ?
                            <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                            :
                            <Tooltip title={t("Services.Description")}>
                                <IconButton size="small">
                                    <InfoOutlinedIcon fontSize="inherit" />
                                </IconButton>
                            </Tooltip>
                        }
                    </Typography>
                    {!loading && garageLookup?.knownServices && garageLookup.knownServices.map((item) =>
                        <GarageServiceInfoCard
                            key={`service-card-${item}`}
                            serviceType={item}
                            selectedItem={selectedItem}
                            setSelectedItem={setSelectedItem}
                            addCartItem={tryAddCartItem}
                            hasQuestionItem={hasQuestionItem}
                        />
                    )}
                </Grid>
                <Grid item xs={12} md={4}>
                    <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                        {t("Contact")}
                        {loading ?
                            <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                            :
                            <Tooltip title={t("Contact.Description")}>
                                <IconButton size="small">
                                    <InfoOutlinedIcon fontSize="inherit" />
                                </IconButton>
                            </Tooltip>
                        }
                    </Typography>
                    <Typography variant="subtitle1">
                        {garageLookup?.address}, {garageLookup?.city}
                    </Typography>
                    <Typography variant="subtitle1">
                        {garageLookup?.daysOfWeek}
                    </Typography>
                    <Typography variant="subtitle1">
                        {garageLookup?.phoneNumber}
                    </Typography>
                    <Typography variant="subtitle1">
                        {garageLookup?.whatsappNumber}
                    </Typography>
                    <Typography variant="subtitle1">
                        {garageLookup?.emailAddress}
                    </Typography>
                </Grid> 
            </Grid>
        </Container>
    </>;
}
