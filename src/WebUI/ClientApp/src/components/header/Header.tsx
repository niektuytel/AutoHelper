import React, { useState, MouseEvent, useEffect, useRef, RefObject } from "react";
import CloseIcon from '@mui/icons-material/Close';
import { useDispatch, useSelector } from "react-redux";
import { Avatar, Box, Breadcrumbs, Button, Card, CardContent, CardHeader, Container, Divider, Grid, Hidden, IconButton, Link, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, Skeleton, Theme, Typography, useMediaQuery, useTheme } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import DirectionsCarIcon from '@mui/icons-material/DirectionsCar';

import ChevronLeftRounded from '@mui/icons-material/ChevronLeftRounded';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import MenuDrawer from "./components/MenuDrawer";
import CarRepairIcon from '@mui/icons-material/CarRepair';
import ImageLogo from "../logo/ImageLogo";
import DeleteIcon from '@mui/icons-material/Delete';

// own imports
import { StyledAppBar, StyledToolbar, StyledIconButton, StyledBadge } from "./HeaderStyle";
import LoginButton from "./components/LoginButton";
import ConstructionIcon from '@mui/icons-material/Construction';
import { BorderBottom } from "@mui/icons-material";
import { COLORS } from "../../constants/colors";
import { useNavigate, useParams } from "react-router";
import { ROUTES } from "../../constants/routes";
import { GarageLookupDtoItem, VehicleService } from "../../app/web-api-client";
import { getServices, removeService } from "../../redux/slices/storedServicesSlice";
import SelectedServicesCard from "./components/SelectedServicesCard";
import { useTranslation } from "react-i18next";
import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";


interface IProps {
    showStaticDrawer: boolean;
    garageLookupIsLoading?: boolean | undefined;
    garageLookup?: GarageLookupDtoItem | undefined;
    navigateGoto?: () => void | undefined;
}

const Header = ({ garageLookupIsLoading, garageLookup, showStaticDrawer, navigateGoto }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { license_plate } = useParams<{ license_plate: string }>();
    const [onMenu, setOnMenu] = useState(false);
    const [isCardVisible, setIsCardVisible] = useState(false);
    const services: VehicleService[] = useSelector((state: any) => state.storedServices);
    const headerRef = React.useRef<HTMLDivElement | null>(null);
    const [headerHeight, setHeaderHeight] = useState(75);
    const navigate = useNavigate();
    const { t } = useTranslation();


    React.useEffect(() => {
        if (headerRef.current) {
            setHeaderHeight(headerRef.current.offsetHeight);
        }
    }, [headerRef.current]);

    const has3Sections = (garageLookupIsLoading || garageLookup);
    return (
        <>
            <div style={{ margin: `${headerHeight}px 0` }} />
            <StyledAppBar
                ref={headerRef}
                sx={(showStaticDrawer && !onMenu) ? { zIndex: (theme) => theme.zIndex.drawer + 1 } : {}}
                style={{
                    boxShadow: `none`,
                    borderBottom: `1px solid ${COLORS.BORDER_GRAY}`
                }}
            >
                <StyledToolbar>
                    <Grid container>
                        <Grid item xs={has3Sections ? 4 : 6} sx={isMobile ? { paddingLeft: "24px", display: 'flex', alignItems: 'center' } : { display: 'flex', alignItems: 'center' }}>
                            <ImageLogo small navigateGoto={navigateGoto} />
                            {license_plate && location.pathname != `/vehicle/${license_plate}` &&
                                <>
                                    <Hidden mdUp>
                                        <IconButton
                                            onClick={() => navigate(`/vehicle/${license_plate}`)}
                                            sx={{ color: 'white', backgroundColor: COLORS.BLUE, marginLeft: 2, '&:hover': { backgroundColor: COLORS.HOVERED_BLUE } }}
                                        >
                                            <DirectionsCarIcon />
                                        </IconButton>
                                    </Hidden>
                                    <Hidden mdDown>
                                        <Button
                                            startIcon={<DirectionsCarIcon />}
                                            onClick={() => navigate(`/vehicle/${license_plate}`)}
                                            sx={{ color: 'white', backgroundColor: COLORS.BLUE, marginLeft: 2, '&:hover': { backgroundColor: COLORS.HOVERED_BLUE } }}
                                        >
                                            {license_plate}
                                        </Button>
                                    </Hidden>
                                </>
                            }
                        </Grid>
                        { has3Sections &&
                            <Grid item xs={4} md={4} sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
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
                                        <CarRepairIcon />
                                    </StyledBadge>
                                </StyledIconButton>
                            }
                            {showStaticDrawer ?
                                <Box sx={{ float:"right", height:"100%" }}>
                                    <LoginButton />
                                </Box>
                                :
                                <>
                                    <AuthenticatedTemplate>
                                        <StyledIconButton onClick={() => setOnMenu(!onMenu)}>
                                            <MenuIcon />
                                        </StyledIconButton>
                                    </AuthenticatedTemplate>
                                    <UnauthenticatedTemplate>
                                        <Hidden mdUp>
                                            <StyledIconButton onClick={() => setOnMenu(!onMenu)}>
                                                <MenuIcon />
                                            </StyledIconButton>
                                        </Hidden>
                                        <Hidden mdDown>
                                            <LoginButton />
                                        </Hidden>
                                    </UnauthenticatedTemplate>
                                </>
                            }
                            <SelectedServicesCard isCardVisible={isCardVisible} services={services} onClose={() => setIsCardVisible(false)} />
                        </Grid>
                    </Grid>
                </StyledToolbar>
            </StyledAppBar>
            <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} showStaticDrawer={showStaticDrawer} />
        </>
    );
}

export default Header;
