import React, { useState, MouseEvent, useEffect, useRef, RefObject } from "react";
import CloseIcon from '@mui/icons-material/Close';
import { useDispatch, useSelector } from "react-redux";
import { Avatar, Box, Button, Card, CardContent, CardHeader, Container, Divider, Grid, Hidden, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, Skeleton, Theme, Typography, useMediaQuery, useTheme } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';

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
import HeaderLicensePlateSearch from "./components/HeaderLicensePlateSearch";
import { ROUTES } from "../../constants/routes";
import { GarageLookupDtoItem, SelectedService } from "../../app/web-api-client";
import { getServices, removeService } from "../../redux/slices/storedServicesSlice";
import SelectedServicesCard from "./components/SelectedServicesCard";


interface IProps {
    showStaticDrawer: boolean;
    garageLookupIsLoading?: boolean | undefined;
    garageLookup?: GarageLookupDtoItem | undefined;
    navigateGoto?: () => void | undefined;
}

const Header = ({ garageLookupIsLoading, garageLookup, showStaticDrawer, navigateGoto }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [onMenu, setOnMenu] = useState(false);
    const [isCardVisible, setIsCardVisible] = useState(false);
    const services: SelectedService[] = useSelector((state: any) => state.storedServices);
    const headerRef = React.useRef<HTMLDivElement | null>(null);
    const [headerHeight, setHeaderHeight] = useState(75);

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
                    boxShadow: `none`,
                    borderBottom: `1px solid ${COLORS.BORDER_GRAY}`,
                    zIndex: 1
                }}
            >
                <StyledToolbar>
                    <Grid container>
                        <Grid item xs={has3Sections ? 4 : 6} sx={isMobile ? { paddingLeft: "24px", display: 'flex', alignItems: 'center' } : { display: 'flex', alignItems: 'center' }}>
                            <ImageLogo small navigateGoto={navigateGoto} />
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
                                        <CarRepairIcon />
                                    </StyledBadge>
                                </StyledIconButton>
                            }
                            {showStaticDrawer ?
                                <Box sx={{ float:"right", height:"100%" }}>
                                    <LoginButton />
                                </Box>
                                :
                                <StyledIconButton onClick={() => setOnMenu(!onMenu)}>
                                    <MenuIcon />
                                </StyledIconButton>
                            }
                            <SelectedServicesCard isCardVisible={isCardVisible} services={services} onClose={() => setIsCardVisible(false)} />
                        </Grid>
                    </Grid>
                </StyledToolbar>
            </StyledAppBar>
            <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} />
        </>
    );
}

export default Header;
