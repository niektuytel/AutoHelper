import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Container, Grid, Hidden, IconButton } from "@mui/material";
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

interface IProps {
    isAdmin: boolean;
}

const Header = ({ isAdmin }: IProps) => {
    const path = window.location.pathname;
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
                style={{
                    boxShadow: "0px 2px 2px -1px rgb(0 0 0 / 20%), 0px 1px 2px 0px rgb(0 0 0 / 14%), 0px 1px 1px 0px rgb(0 0 0 / 12%)"
                }}
            >
                <StyledToolbar>
                    <Grid container>
                        <Grid item xs={6} sx={{ display: 'flex', alignItems: 'center' }}>
                            <ImageLogo small />
                        </Grid>
                        <Grid item xs={6} sx={{ textAlign: "right" }}>
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
            <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} isAdmin={isAdmin} />
            <CartDrawer cartOpen={cartOpen} setCartOpen={setCartOpen} />
        </>
    );
}

export default Header;
