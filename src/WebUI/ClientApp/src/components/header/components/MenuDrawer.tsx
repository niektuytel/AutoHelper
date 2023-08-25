import React, { useState, MouseEvent, useEffect } from 'react';
import StoreIcon from '@mui/icons-material/Store';
import LoyaltyIcon from '@mui/icons-material/Loyalty';
import LocalOfferIcon from '@mui/icons-material/LocalOffer';
import HomeIcon from '@mui/icons-material/Home';
import LabelIcon from '@mui/icons-material/Label';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import ReorderIcon from '@mui/icons-material/Reorder';
import DashboardIcon from '@mui/icons-material/Dashboard';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import BuildIcon from '@mui/icons-material/Build';
import GroupIcon from '@mui/icons-material/Group';
import SettingsIcon from '@mui/icons-material/Settings';

import { Accordion, CircularProgress,  AccordionDetails, AccordionSummary, Divider, Drawer, Hidden, List, ListItem, ListItemIcon, ListItemText, Toolbar, Typography, Button, Box, Menu, MenuItem, Collapse, ListItemButton } from "@mui/material";
import { useNavigate } from "react-router-dom";
//import { msalInstance } from "../../../index";

import { useTranslation } from "react-i18next";
import { useAuth0 } from "@auth0/auth0-react";
import jwt_decode from 'jwt-decode'; 


import { HashValues } from "../../../i18n/HashValues";


interface IProps {
    onMenu: boolean;
    setOnMenu: (value: boolean) => void;
    isAdmin: boolean;
}

export default ({ onMenu, setOnMenu, isAdmin }: IProps) => {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [userRoles, setUserRoles] = useState<string[] | null>(null);
    const [open, setOpen] = React.useState(true);
    const { loginWithRedirect, logout, isAuthenticated, isLoading, error, getAccessTokenSilently, getIdTokenClaims } = useAuth0();
    const path = window.location.pathname;
    const navigate = useNavigate();
    const { t } = useTranslation();

    const rolesClaim = `${window.location.origin}/roles`

    useEffect(() => {
        const fetchRole = async () => {
            try {
                const idTokenClaims: any = await getIdTokenClaims();
                console.log(idTokenClaims);

                const roles = idTokenClaims[rolesClaim];
                setUserRoles(roles);
            } catch (e) {
                console.error("Error fetching role:", e);
            }
        };

        fetchRole();
    }, [getIdTokenClaims]);


    const onClick = (url: string) => {
        navigate(url)
        setOnMenu(false);
    }

    
    const handleClick2 = () => {
        setOpen(!open);
    };

    const handleClick = (event: MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogin = (role: string) => {
        if (role === 'garage') {
            loginWithRedirect({
                appState: {
                    returnTo: "/garage/overview"
                },
                authorizationParams: {
                    signUpAsGarage: true
                }
            });
        } else {
            loginWithRedirect({
                appState: {
                    returnTo: "/select-vehicle"
                }
            });
        }

        handleClose();
    };



    if (error) {
        console.error("Auth0 Error:", error);
    }
    
    return (
        <Drawer open={onMenu} onClose={() => setOnMenu(!onMenu)}>
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'flex-end',
                    width: "100%",
                    padding: "12px!important",  // Add padding around the toolbar
                }}
            >
                {isLoading ? (
                    <Box display="flex" justifyContent="center" width="100%">
                        <CircularProgress color="secondary" />  {/* Loading bar */}
                    </Box>
                ) : isAuthenticated ? (
                    <Button
                        variant="contained"
                        fullWidth  // Make button full width
                        style={{ backgroundColor: 'black' }} // Set color to black
                        onClick={() => {
                            setUserRoles(null);
                            logout({ logoutParams: { returnTo: window.location.origin } })
                        }}
                    >
                        Logout
                    </Button>
                    ) : (<>
                    <Button
                        variant="contained"
                        fullWidth 
                        style={{ backgroundColor: '#1C94F3' }}
                        onClick={handleClick}
                    >
                        Login
                    </Button>
                    <Menu
                        anchorEl={anchorEl}
                        open={Boolean(anchorEl)}
                        onClose={handleClose}
                    >
                        <MenuItem onClick={() => handleLogin('user')}>Login as User</MenuItem>
                        <MenuItem onClick={() => handleLogin('garage')}>Login as Garage</MenuItem>
                    </Menu>
                </>)}
            </Toolbar>
            <Divider />
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemButton onClick={handleClick2}>
                    <ListItemIcon>
                        <AccountBoxIcon />
                    </ListItemIcon>
                    <ListItemText primary="Account" />
                    {open ? <ExpandLess /> : <ExpandMore />}
                </ListItemButton>
                <Collapse in={open} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                        <ListItem button onClick={() => onClick("/user/overview")}>
                            <ListItemIcon>
                                <HomeIcon />
                            </ListItemIcon>
                            <ListItemText primary="Overview" />
                        </ListItem>
                    </List>
                </Collapse>
                {userRoles?.includes('garage') && (
                    <>
                        <ListItem button onClick={() => onClick("/garage/overview")}>
                            <ListItemIcon>
                                <DashboardIcon />
                            </ListItemIcon>
                            <ListItemText primary="Overview" />
                        </ListItem>
                        <ListItem button onClick={() => onClick("/garage/agenda")}>
                            <ListItemIcon>
                                <CalendarTodayIcon />
                            </ListItemIcon>
                            <ListItemText primary="Agenda" />
                        </ListItem>
                        <ListItem button onClick={() => onClick("/garage/services")}>
                            <ListItemIcon>
                                <BuildIcon />
                            </ListItemIcon>
                            <ListItemText primary="Services" />
                        </ListItem>
                        <ListItemButton onClick={handleClick2}>
                            <ListItemIcon>
                                <GroupIcon />
                            </ListItemIcon>
                            <ListItemText primary="All Colleagues" />
                            {open ? <ExpandLess /> : <ExpandMore />}
                        </ListItemButton>
                        <Collapse in={open} timeout="auto" unmountOnExit>
                            <List component="div" disablePadding>
                                <ListItem button onClick={() => onClick("/garage/colleague1")}>
                                    <ListItemIcon>
                                        {/* Icon for Colleague 1 */}
                                    </ListItemIcon>
                                    <ListItemText primary="Colleague 1" />
                                </ListItem>
                            </List>
                        </Collapse>
                        <ListItem button onClick={() => onClick("/garage/settings")}>
                            <ListItemIcon>
                                <SettingsIcon />
                            </ListItemIcon>
                            <ListItemText primary="Settings" />
                        </ListItem>
                    </>
                )}
            </List>
        </Drawer>
    );
}

