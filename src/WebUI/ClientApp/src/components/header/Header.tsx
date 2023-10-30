import React, { useState, MouseEvent, useEffect, useRef, RefObject } from "react";
import CloseIcon from '@mui/icons-material/Close';
import { useSelector } from "react-redux";
import { Avatar, Button, Card, CardContent, CardHeader, Container, Divider, Grid, Hidden, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, Skeleton, Theme, Typography, useMediaQuery, useTheme } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';

import ChevronLeftRounded from '@mui/icons-material/ChevronLeftRounded';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import MenuDrawer from "./components/MenuDrawer";
import ImageLogo from "../logo/ImageLogo";
import DeleteIcon from '@mui/icons-material/Delete';

// own imports
import { StyledAppBar, StyledToolbar, StyledIconButton, StyledBadge } from "./HeaderStyle";
import LoginButton from "./components/LoginButton";
import ConstructionIcon from '@mui/icons-material/Construction';
import { BorderBottom } from "@mui/icons-material";
import { COLORS } from "../../constants/colors";
import { useParams } from "react-router";
import HeaderLicensePlateSearch from "./components/HeaderLicensePlateSearch";
import { ROUTES } from "../../constants/routes";
import { GarageLookupDto, GarageServiceItemDto } from "../../app/web-api-client";
import { getServices } from "../../redux/slices/storedServicesSlice";


export function useOutsideClick<T extends HTMLElement = HTMLElement>(
    ref: RefObject<T>,
    callback: () => void
): void {
    const handleClick = (e: any) => {
        if (ref.current && !ref.current.contains(e.target as Node)) {
            callback();
        }
    };

    useEffect(() => {
        document.addEventListener('mousedown', handleClick);
        return () => {
            document.removeEventListener('mousedown', handleClick);
        };
    }, [ref, callback]);
}

interface Props {
    services: GarageServiceItemDto[];
    onClose: () => void;
}

const SelectedServicesCard: React.FC<Props> = ({ services, onClose }) => {
    const cardRef = useRef<HTMLDivElement>(null);
    useOutsideClick(cardRef, onClose);


    const handleServiceClick = (serviceId: string) => {
        // Replace '/garage/' with the actual path to your garage page
        // and append the service ID or any other identifier you need
        //history.push(`/garage/${serviceId}`);
    };

    const handleServiceRemove = (event: React.MouseEvent<HTMLButtonElement>, service: GarageServiceItemDto) => {
        event.stopPropagation(); // Stop the click event from bubbling up
        // Replace '/garage/' with the actual path to your garage page
        // and append the service ID or any other identifier you need
        //history.push(`/garage/${serviceId}`);
    };

    return (
        <Card ref={cardRef} sx={{ maxWidth: 345, position: 'absolute', top: '100%', right: 0, zIndex: 2, m:1 }}>
            <CardHeader
                title="Selected Services"
                action={
                    <IconButton onClick={onClose}>
                        <CloseIcon />
                    </IconButton>
                }
                titleTypographyProps={{ align: 'left' }}
                sx={{ paddingBottom: 0 }}
            />
            <CardContent>
                <List dense>
                    {services.map((service, index) => (
                        <React.Fragment key={service.id}>
                            {index > 0 && <Divider component="li" />}
                            <ListItem
                                secondaryAction={
                                    <IconButton edge="end" aria-label="delete">
                                        <DeleteIcon onClick={(e:any) => handleServiceRemove(e, service)} />
                                    </IconButton>
                                }
                                sx={{
                                    '&:hover': {
                                        backgroundColor: 'rgba(0, 0, 0, 0.04)', // Or any other color
                                        cursor: 'pointer',
                                    },
                                }}
                            >
                                <ListItemText
                                    primary={
                                        <Typography variant="subtitle1" noWrap>
                                            {"service.name"}
                                        </Typography>
                                    }
                                    secondary={
                                        <Typography variant="body2" noWrap sx={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                            {service.description}
                                        </Typography>
                                    }
                                />
                            </ListItem>
                        </React.Fragment>
                    ))}
                </List>
                <Button variant="outlined" fullWidth sx={{ marginTop: 2 }}>
                    Vraag stellen
                </Button>
                <Button variant="contained" fullWidth sx={{ marginTop: 2 }}>
                    Offerte aanvragen
                </Button>
            </CardContent>
        </Card>
    );
};

interface IProps {
    showStaticDrawer: boolean;
    garageLookupIsLoading?: boolean | undefined;
    garageLookup?: GarageLookupDto | undefined;
}

const Header = ({ garageLookupIsLoading, garageLookup, showStaticDrawer }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [onMenu, setOnMenu] = useState(false);
    const [isCardVisible, setIsCardVisible] = useState(false);
    const services: GarageServiceItemDto[] = useSelector((state: any) => state.storedServices);

    // 1. State to track the currently focused button
    const [focusedButton, setFocusedButton] = useState<string | null>("#services");

    // 2. Handler function to update the URL and set the focused button
    const handleButtonClick = (hash: string) => {
        window.location.hash = hash;
        setFocusedButton(hash);
    };

    // Step 1: Create a ref for the header and a state to store its height.
    const headerRef = React.useRef<HTMLDivElement | null>(null);
    const [headerHeight, setHeaderHeight] = useState(75);  // default to 75px

    // Step 2: Use an effect to set the header's height to the state.
    React.useEffect(() => {
        if (headerRef.current) {
            setHeaderHeight(headerRef.current.offsetHeight);
        }
    }, [headerRef.current]);

    const isEditableLicensePlate = location.pathname.startsWith(ROUTES.SELECT_GARAGE);
    const has3Sections = isEditableLicensePlate || (garageLookupIsLoading || garageLookup);
    return (
        <>
            <div style={{ margin: `${headerHeight}px 0` }} />
            <StyledAppBar
                ref={headerRef}
                sx={(showStaticDrawer && !onMenu) ? { zIndex: (theme) => theme.zIndex.drawer + 1 } : {}}
                style={{
                    boxShadow: "none",
                    borderBottom: `1px solid ${COLORS.BORDER_GRAY}`,
                    zIndex: 1
                }}
            >
                <StyledToolbar>
                    <Grid container>
                        <Grid item xs={has3Sections ? 4 : 6} sx={isMobile ? { paddingLeft: "24px", display: 'flex', alignItems: 'center' } : { display: 'flex', alignItems: 'center' }}>
                            <ImageLogo small />
                        </Grid>
                        { has3Sections &&
                            <Grid item xs={4} md={4} sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                {isEditableLicensePlate && <HeaderLicensePlateSearch />}
                                {garageLookupIsLoading && <Skeleton variant='rounded' />}
                                {garageLookup?.name &&
                                    <Typography 
                                        variant={"h5"} 
                                        sx={{
                                            color: "black",
                                            fontFamily: "Dubai light",
                                            marginTop: "5px",
                                            overflow: "hidden",
                                            textOverflow: "ellipsis",
                                            whiteSpace: "nowrap",
                                            maxWidth: "100%"  // or set to any value that fits well in your layout
                                        }}
                                    >
                                        <b>{garageLookup.name}</b>
                                    </Typography>
                                }
                            </Grid>
                        }
                        <Grid item xs={has3Sections ? 4 : 6} sx={isMobile ? { pr:1, textAlign: "right" } : { textAlign: "right" }}>
                            {services.length! > 0 && 
                                <StyledIconButton onClick={() => setIsCardVisible(true)}>
                                    <StyledBadge badgeContent={services!.length!} color="error">
                                        <ConstructionIcon />
                                    </StyledBadge>
                                </StyledIconButton>
                            }
                            <StyledIconButton onClick={() => setOnMenu(!onMenu)}>
                                <MenuIcon />
                            </StyledIconButton>
                            {isCardVisible && <SelectedServicesCard services={services} onClose={() => setIsCardVisible(false)} />}
                        </Grid>
                    </Grid>
                </StyledToolbar>
            </StyledAppBar>
            <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} />
        </>
    );
}

export default Header;
