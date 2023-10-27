import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, ButtonGroup, CircularProgress, Container, Divider, Grid, IconButton, Pagination, Paper, Skeleton, TextField, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import LiveHelpIcon from '@mui/icons-material/LiveHelp';
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import PhoneIcon from '@mui/icons-material/Phone';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import WhatsAppIcon from '@mui/icons-material/WhatsApp';

// local
import ImageLogo from "../../components/logo/ImageLogo";
import { COLORS } from "../../constants/colors";
import { GarageLookupDto, GarageServiceItem, GarageServiceItemDto, GarageServiceType, PaginatedListOfGarageLookupBriefDto } from "../../app/web-api-client";
import { useQueryClient } from "react-query";
import useGarage from "./useGarage";
import Header from "../../components/header/Header";
import GarageServiceInfoCard from "./components/GarageServiceInfoCard";
import { showOnError, showOnSuccess } from "../../redux/slices/statusSnackbarSlice";
import { useDispatch } from "react-redux";
import GarageDailySchedule from "./components/GarageDailySchedule";
import GarageContactSection from "./components/GarageContactSection";
import GarageQuestionDialog from "./components/GarageQuestionDialog";

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const location = useLocation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const dispatch = useDispatch();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [relatedServiceTypes, setRelatedServiceTypes] = useState<GarageServiceType[]>([]);
    const [selectedItem, setSelectedItem] = useState<GarageServiceItemDto | null>(null);
    const [cartItems, setCartItems] = useState<GarageServiceItemDto[]>([]);
    const queryParams = new URLSearchParams(location.search);
    const { identifier } = useParams();
    const licensePlate = queryParams.get('licensePlate');
    const lat = queryParams.get('lat');
    const lng = queryParams.get('lng');

    const { loading, garageLookup, fetchGarageLookupByPlate } = useGarage(identifier!, licensePlate);
    //const { startConversatrion } = useConversation();

    const tryAddCartItem = (itemToAdd: GarageServiceItemDto) => {
        if (cartItems.some(cartItem => cartItem.id === itemToAdd.id)) {
            dispatch(showOnError(t("Cart item already exist")));
            return;
        }

        setCartItems([...cartItems, itemToAdd]);
    }

    const hasQuestionItem = (serviceType: GarageServiceType) => {
        setRelatedServiceTypes([ serviceType ]);
        setDialogOpen(true);
    };

    if (!loading && garageLookup?.garageId)
    {
        // TODO: handle garage specific page
    }


    return <>
        <Header garageLookupIsLoading={loading} garageLookup={garageLookup} showStaticDrawer={false} />
        <Container sx={{ mb: 5 }}>
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
                        garageLookup?.knownServices?.map((item) =>
                            <GarageServiceInfoCard
                                key={`service-card-${item}`}
                                serviceType={item}
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
        {garageLookup && relatedServiceTypes &&
            <GarageQuestionDialog
                garageLookupId={garageLookup?.id!}
                garageWhatsAppNumberOrEmail={garageLookup?.whatsappNumber! || garageLookup?.emailAddress!}
                relatedServiceTypes={relatedServiceTypes!}
                licensePlate={licensePlate!}
                longitude={lng!}
                latitude={lat!}
                open={dialogOpen}
                onClose={() => setDialogOpen(false)}
            />
        }
    </>;
}
