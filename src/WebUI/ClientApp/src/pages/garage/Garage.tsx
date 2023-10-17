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
import GarageDailySchedule from "./components/GarageDailySchedule";

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const location = useLocation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const dispatch = useDispatch();
    const [hoveredContact, setHoveredContact] = useState<string | null>(null);
    const [selectedItem, setSelectedItem] = useState<GarageServiceItemDto|null>(null);
    const [cartItems, setCartItems] = useState<GarageServiceItemDto[]>([]);
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

    const copyToClipboard = (data: string) => {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(data);
            // You can display a toast message or some feedback here
        }
    };

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
                    <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                        {t("Contact")}
                        <Tooltip title={t("Contact.Description")}>
                            <IconButton size="small">
                                <InfoOutlinedIcon fontSize="inherit" />
                            </IconButton>
                        </Tooltip>
                    </Typography>
                    <Typography
                        variant="subtitle1"
                        display="flex"
                        alignItems="center"
                        onMouseEnter={() => setHoveredContact('address')}
                        onMouseLeave={() => setHoveredContact(null)}
                        
                    >
                        <LocationOnIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
                        {loading ?
                            <Skeleton width="50%" />
                            :
                            <>
                                {garageLookup?.address}, {garageLookup?.city}
                                {hoveredContact === 'address' && !isMobile &&
                                    <IconButton onClick={() => copyToClipboard(`${garageLookup?.address}, ${garageLookup?.city}`)}>
                                        <ContentCopyIcon fontSize="small" />
                                    </IconButton>
                                }
                            </>
                        }
                    </Typography>
                    <Typography
                        variant="subtitle1"
                        display="flex"
                        alignItems="center"
                        onMouseEnter={() => setHoveredContact('email')}
                        onMouseLeave={() => setHoveredContact(null)}
                    >
                        <MailOutlineIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
                        {loading ?
                            <Skeleton width="50%" />
                            :
                            <>
                                <Link to={`mailto:${garageLookup?.emailAddress}`}>
                                    {garageLookup?.emailAddress}
                                </Link>
                                {hoveredContact === 'email' &&
                                    <IconButton onClick={() => copyToClipboard(garageLookup?.emailAddress || '')}>
                                        <ContentCopyIcon fontSize="small" />
                                    </IconButton>
                                }
                            </>
                        }
                    </Typography>
                    <Typography
                        variant="subtitle1"
                        display="flex"
                        alignItems="center"
                        onMouseEnter={() => setHoveredContact('phone')}
                        onMouseLeave={() => setHoveredContact(null)}
                    >
                        <PhoneIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
                        {loading ?
                            <Skeleton width="50%" />
                            :
                            <>
                                <Link to={`tel:${garageLookup?.phoneNumber}`}>
                                    {garageLookup?.phoneNumber}
                                </Link>
                                {hoveredContact === 'phone' &&
                                    <IconButton onClick={() => copyToClipboard(garageLookup?.phoneNumber || '')}>
                                        <ContentCopyIcon fontSize="small" />
                                    </IconButton>
                                }
                            </>
                        }
                    </Typography>
                    { garageLookup?.whatsappNumber &&
                        <Typography
                            variant="subtitle1"
                            display="flex"
                            alignItems="center"
                            onMouseEnter={() => setHoveredContact('whatsapp')}
                            onMouseLeave={() => setHoveredContact(null)}
                        >
                            <WhatsAppIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
                            <Link to={`https://wa.me/${garageLookup?.whatsappNumber}`}>
                                {garageLookup?.whatsappNumber}
                            </Link>
                            {hoveredContact === 'whatsapp' &&
                                <IconButton onClick={() => copyToClipboard(garageLookup?.whatsappNumber || '')}>
                                    <ContentCopyIcon fontSize="small" />
                                </IconButton>
                            }
                        </Typography>
                    }
                    <GarageDailySchedule openDaysOfWeek={garageLookup?.daysOfWeek} />
                </Grid>  
            </Grid>
        </Container>
    </>;
}
