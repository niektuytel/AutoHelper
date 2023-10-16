import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Button, Container, Grid, Hidden, IconButton, Skeleton, Theme, Typography, useMediaQuery, useTheme } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';

import ChevronLeftRounded from '@mui/icons-material/ChevronLeftRounded';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import MenuDrawer from "./components/MenuDrawer";
import CartDrawer from "../cart/CartDrawer";
import { colorOnIndex } from "../../i18n/ColorValues";
//import CartState from "../../store/cart/CartState";
import ImageLogo from "../logo/ImageLogo";

// own imports
import { StyledAppBar, StyledToolbar, StyledIconButton, StyledBadge } from "./HeaderStyle";
import { ICookieProduct } from "../../interfaces/ICookieProduct";
import LoginButton from "./components/LoginButton";
import { BorderBottom } from "@mui/icons-material";
import { COLORS } from "../../constants/colors";
import { useParams } from "react-router";
import HeaderLicensePlateSearch from "./components/HeaderLicensePlateSearch";
import { ROUTES } from "../../constants/routes";
import { GarageLookupDto } from "../../app/web-api-client";

interface IProps {
    showStaticDrawer: boolean;
    garageLookupIsLoading?: boolean | undefined;
    garageLookup?: GarageLookupDto | undefined;
}

const Header = ({ garageLookupIsLoading, garageLookup, showStaticDrawer }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [onMenu, setOnMenu] = useState(false);
    const [cartOpen, setCartOpen] = useState(false);
    //const { items }:CartState = useSelector((state:any) => state.cart);
    const items: ICookieProduct[] = [];

    // set cart total quantity
    var badgeContent = 0;
    items.forEach(item => badgeContent += item.quantity);

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
                        <Grid item xs={has3Sections ? 4 : 6} sx={isMobile ? { paddingRight: "24px", textAlign: "right" } : { textAlign: "right" }}>
                            <Hidden xsDown>
                                {badgeContent > 0 && !location.pathname.startsWith("/cart") &&
                                    <StyledIconButton onClick={() => setCartOpen(true)}>
                                        <StyledBadge badgeContent={badgeContent} color="error">
                                            <ShoppingCartOutlinedIcon />
                                        </StyledBadge>
                                    </StyledIconButton>
                                }
                            </Hidden>
                            <StyledIconButton onClick={() => setOnMenu(!onMenu)}>
                                <MenuIcon />
                            </StyledIconButton>
                        </Grid>
                    </Grid>
                </StyledToolbar>
            </StyledAppBar>
            <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} />
            <CartDrawer cartOpen={cartOpen} setCartOpen={setCartOpen} />
        </>
    );
}

export default Header;
