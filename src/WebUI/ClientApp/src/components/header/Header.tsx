import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Container, Grid, Hidden, IconButton, Theme, useMediaQuery, useTheme } from "@mui/material";
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

interface IProps {
    showStaticDrawer: boolean;
}

const Header = ({ showStaticDrawer }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [onMenu, setOnMenu] = useState(false);
    const [cartOpen, setCartOpen] = useState(false);
    //const { items }:CartState = useSelector((state:any) => state.cart);
    const items: ICookieProduct[] = [];

    // set cart total quantity
    var badgeContent = 0;
    items.forEach(item => badgeContent += item.quantity);


    // Step 1: Create a ref for the header and a state to store its height.
    const headerRef = React.useRef<HTMLDivElement | null>(null);
    const [headerHeight, setHeaderHeight] = useState(75);  // default to 75px

    // Step 2: Use an effect to set the header's height to the state.
    React.useEffect(() => {
        if (headerRef.current) {
            setHeaderHeight(headerRef.current.offsetHeight);
        }
    }, [headerRef.current]);

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
                        <Grid item xs={3} md={4} sx={isMobile ? { paddingLeft: "24px", display: 'flex', alignItems: 'center' } : { display: 'flex', alignItems: 'center' }}>
                            <ImageLogo small />
                        </Grid>
                        <Grid item xs={6} md={4} sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                            <HeaderLicensePlateSearch/>
                        </Grid>
                        <Grid item xs={3} md={4} sx={isMobile ? { paddingRight: "24px", textAlign: "right" } : { textAlign: "right" }}>
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
