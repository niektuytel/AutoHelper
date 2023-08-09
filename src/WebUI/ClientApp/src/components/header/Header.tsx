import React, { useState } from "react";
import { useSelector } from "react-redux";
import { AppBar, Container, Grid, Hidden, IconButton, Toolbar } from "@material-ui/core";
import Badge from '@material-ui/core/Badge';
import MenuIcon from '@material-ui/icons/Menu';
import ShoppingCartOutlinedIcon from '@material-ui/icons/ShoppingCartOutlined';
import MenuDrawer from "./components/MenuDrawer";
import CartDrawer from "../cart/CartDrawer";
import HeaderStyle from "./HeaderStyle";
import CartState from "../../store/cart/CartState";
import ImageLogo from "../logo/ImageLogo";
 
interface IProps {
    isAdmin: boolean;
}

const Header = ({isAdmin}:IProps) => {
    const path = window.location.pathname;
    const classes = HeaderStyle();
    const [onMenu, setOnMenu] = useState(false);
    const [cartOpen, setCartOpen] = useState(false);
    const { items }:CartState = useSelector((state:any) => state.cart);

    
    // set cart total quantity
    var badgeContent = 0;
    items.forEach(item => badgeContent += item.quantity)
    return <>
        <div className={classes.headerHeight}/>
        <AppBar className={classes.appbar} style={{ 
            boxShadow: "0px 2px 2px -1px rgb(0 0 0 / 20%), 0px 1px 2px 0px rgb(0 0 0 / 14%), 0px 1px 1px 0px rgb(0 0 0 / 12%)"
        }}>
            <Toolbar className={classes.toolbar}>
                    <Grid container>
                        <Grid item xs={6} className={classes.centerVertically}>
                            <ImageLogo small className={classes.marginLeft16} />
                        </Grid>
                        <Grid item xs={6} className={classes.iconGrid}>
                            <Hidden xsDown>
                                {badgeContent > 0 && !location.pathname.startsWith("/cart") && 
                                    <IconButton className={classes.icon} onClick={() => setCartOpen(true)}>
                                        <Badge 
                                            classes={{ badge: classes.badge }} 
                                            badgeContent={badgeContent} 
                                            color="error" 
                                            overlap="circular"
                                        >
                                            <ShoppingCartOutlinedIcon />
                                        </Badge>
                                    </IconButton>
                                }
                            </Hidden>
                            <IconButton className={classes.icon} onClick={() => setOnMenu(!onMenu)}>
                                <MenuIcon />
                            </IconButton>
                        </Grid>
                    </Grid>
            </Toolbar>
        </AppBar>
        <MenuDrawer onMenu={onMenu} setOnMenu={setOnMenu} isAdmin={isAdmin}/>
        <CartDrawer cartOpen={cartOpen} setCartOpen={setCartOpen}/>
    </>
}


export default Header;

