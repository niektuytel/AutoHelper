import React from "react";
import StoreIcon from '@material-ui/icons/Store';
import LoyaltyIcon from '@material-ui/icons/Loyalty';
import LocalOfferIcon from '@material-ui/icons/LocalOffer';
import HomeIcon from '@material-ui/icons/Home';
import LabelIcon from '@material-ui/icons/Label';
import ReorderIcon from '@material-ui/icons/Reorder';
import HorizontalSplitIcon from '@material-ui/icons/HorizontalSplit';
import LockOpenOutlinedIcon from '@material-ui/icons/LockOpenOutlined';
import AlternateEmailIcon from '@material-ui/icons/AlternateEmail';
import InfoIcon from '@material-ui/icons/Info';
import LabelOffIcon from '@material-ui/icons/LabelOff';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import AnnouncementIcon from '@material-ui/icons/Announcement';
import { Accordion, AccordionDetails, AccordionSummary, Divider, Drawer, Hidden, List, ListItem, ListItemIcon, ListItemText, Toolbar, Typography } from "@material-ui/core";
import { useHistory } from "react-router";
import { IsLoggedIn, loginRequest, logoutRequest } from "../../../msalConfig";
import { msalInstance } from "../../../index";
import { useTranslation } from "react-i18next";
import { HashValues } from "../../../i18n/HashValues";

interface IProps {
    onMenu: boolean;
    setOnMenu: (value:boolean) => void;
    isAdmin: boolean;
}


export default ({onMenu, setOnMenu, isAdmin}:IProps) => {
    const path = window.location.pathname;
    const history = useHistory();
    const { t } = useTranslation();
    const onClick = (url:string) => {
        history.push(url)
        setOnMenu(false);
    }

    return <Drawer open={onMenu} onClose={() => setOnMenu(!onMenu)}>
        <Toolbar
            style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'flex-end',
                width:"250px"
            }}
        >
        </Toolbar>
        <Divider />
        <List component="nav">
            <ListItem button onClick={() => onClick("/")}>
                <ListItemIcon>
                    <HomeIcon />
                </ListItemIcon>
                <ListItemText primary="Home" />
            </ListItem>
            {path === "/" &&
                <Hidden smUp>
                    <ListItem button onClick={() => onClick(`/${HashValues.contact}`)}>
                        <ListItemIcon>
                            <AlternateEmailIcon />
                        </ListItemIcon>
                        <ListItemText primary={t("contact")} />
                    </ListItem>
                    <ListItem button onClick={() => onClick(`/${HashValues.info}`)}>
                        <ListItemIcon>
                            <AnnouncementIcon />
                        </ListItemIcon>
                        <ListItemText primary={t("news")}/>
                    </ListItem>
                </Hidden>
            }
            <ListItem button onClick={() => onClick("/products")}>
                <ListItemIcon>
                    <StoreIcon />
                </ListItemIcon>
                <ListItemText primary="Products" />
            </ListItem>
            {isAdmin ?
                <>
                    <Divider/>
                    <ListItem button onClick={() => onClick("/dashboard#tags")}>
                        <ListItemIcon>
                            <LabelIcon />
                        </ListItemIcon>
                        <ListItemText primary="Tags" />
                    </ListItem>
                    <Accordion>
                        <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1a-content"
                        id="panel1a-header"
                        >
                            <Typography>Tags filter (Beta)</Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            
                            <List component="nav">
                                <ListItem button onClick={() => onClick("/dashboard#tagtargets")}>
                                    <ListItemIcon>
                                        <LoyaltyIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Tag Targets" />
                                </ListItem>
                                <ListItem button onClick={() => onClick("/dashboard#tagfilters")}>
                                    <ListItemIcon>
                                        <LocalOfferIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Tag Filters" />
                                </ListItem>
                                <ListItem button onClick={() => onClick("/dashboard#tagsituations")}>
                                    <ListItemIcon>
                                        <LabelOffIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Tag Situations" />
                                </ListItem>
                            </List>
                        </AccordionDetails>
                    </Accordion>
                    {/* <Divider/> */}
                    <ListItem button onClick={() => onClick("/dashboard#orders")}>
                        <ListItemIcon>
                            <ReorderIcon />
                        </ListItemIcon>
                        <ListItemText primary="Orders" />
                    </ListItem>
                    <ListItem button onClick={() => onClick("/dashboard#products")}>
                        <ListItemIcon>
                            <HorizontalSplitIcon />
                        </ListItemIcon>
                        <ListItemText primary="Products" />
                    </ListItem>
                    <ListItem button onClick={() => msalInstance.logoutRedirect(logoutRequest)}>
                        <ListItemIcon>
                            <LockOpenOutlinedIcon />
                        </ListItemIcon>
                        <ListItemText primary="Logout" />
                    </ListItem>
                </>
                :
                IsLoggedIn() ? 
                    <ListItem button onClick={() => msalInstance.logoutRedirect(logoutRequest)}>
                        <ListItemIcon>
                            <LockOpenOutlinedIcon />
                        </ListItemIcon>
                        <ListItemText primary="Logout" />
                    </ListItem>
                :
                    <ListItem button onClick={() => msalInstance.loginRedirect(loginRequest)}>
                        <ListItemIcon>
                            <LockOpenOutlinedIcon />
                        </ListItemIcon>
                        <ListItemText primary="Login" />
                    </ListItem>
            }
        </List>
    </Drawer>;
}

