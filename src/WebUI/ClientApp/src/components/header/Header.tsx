import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Container, Grid, Hidden } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import MenuDrawer from "./components/MenuDrawer";
import CartDrawer from "../cart/CartDrawer";
import { colorOnIndex } from "../../i18n/ColorValues";
//import CartState from "../../store/cart/CartState";
import ImageLogo from "../logo/ImageLogo";

// own imports
import { StyledAppBar, StyledToolbar, StyledIconButton, StyledBadge } from "./HeaderStyle";
import { ICookieProduct } from "../../interfaces/ICookieProduct";

const styles = {
    margin_5: {
        margin: "5px"
    },
    chip: {
        margin: "2px",
    },
    headerHeight: {
        margin: "75px"
    },
    appbar: {
        background: "white"
    },
    toolbar: {
        minHeight: "50px",
        marginLeft: "calc(100vw - 100%)",
        marginRight: "0"
    },
    container: {
        padding: 0
    },
    logoBox: {
        width: "fit-content",
        display: "inline-table",
        marginRight: "5px"
    },
    iconGrid: {
        textAlign: "right",
        color: "black"
    },
    icon: {
        color: "black"
    },
    badge: {
        backgroundColor: colorOnIndex(0),
        color: "white"
    }
}

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

    return (
        <>
            <div style={{ margin: "75px" }} />
            <StyledAppBar style={{
                boxShadow: "0px 2px 2px -1px rgb(0 0 0 / 20%), 0px 1px 2px 0px rgb(0 0 0 / 14%), 0px 1px 1px 0px rgb(0 0 0 / 12%)"
            }}>
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
