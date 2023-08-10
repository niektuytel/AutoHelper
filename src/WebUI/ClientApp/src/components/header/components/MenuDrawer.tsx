import React from "react";
import StoreIcon from '@mui/icons-material/Store';
import LoyaltyIcon from '@mui/icons-material/Loyalty';
import LocalOfferIcon from '@mui/icons-material/LocalOffer';
import HomeIcon from '@mui/icons-material/Home';
import LabelIcon from '@mui/icons-material/Label';
import ReorderIcon from '@mui/icons-material/Reorder';
import HorizontalSplitIcon from '@mui/icons-material/HorizontalSplit';
import LockOpenOutlinedIcon from '@mui/icons-material/LockOpenOutlined';
import AlternateEmailIcon from '@mui/icons-material/AlternateEmail';
import InfoIcon from '@mui/icons-material/Info';
import LabelOffIcon from '@mui/icons-material/LabelOff';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import AnnouncementIcon from '@mui/icons-material/Announcement';
import { Accordion, AccordionDetails, AccordionSummary, Divider, Drawer, Hidden, List, ListItem, ListItemIcon, ListItemText, Toolbar, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { IsLoggedIn, loginRequest, logoutRequest } from "../../../oidcConfig";
//import { msalInstance } from "../../../index";
import { useTranslation } from "react-i18next";
import { HashValues } from "../../../i18n/HashValues";

interface IProps {
    onMenu: boolean;
    setOnMenu: (value: boolean) => void;
    isAdmin: boolean;
}

export default ({ onMenu, setOnMenu, isAdmin }: IProps) => {
    const path = window.location.pathname;
    const navigate = useNavigate();
    const { t } = useTranslation();
    const onClick = (url: string) => {
        navigate(url)
        setOnMenu(false);
    }

    return (
        <Drawer open={onMenu} onClose={() => setOnMenu(!onMenu)}>
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'flex-end',
                    width: "250px"
                }}
            />
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
                        <ListItem button>{/* onClick={() => msalInstance.logoutRedirect(logoutRequest)}>*/}
                            <ListItemIcon>
                                <LockOpenOutlinedIcon />
                            </ListItemIcon>
                            <ListItemText primary="Logout" />
                        </ListItem>
                    </>
                    :
                    IsLoggedIn() ? 
                        <ListItem button>{/* onClick={() => msalInstance.logoutRedirect(logoutRequest)}>*/}
                            <ListItemIcon>
                                <LockOpenOutlinedIcon />
                            </ListItemIcon>
                            <ListItemText primary="Logout" />
                        </ListItem>
                    :
                        <ListItem button>{/* onClick={() => msalInstance.loginRedirect(loginRequest)}>*/}
                            <ListItemIcon>
                                <LockOpenOutlinedIcon />
                            </ListItemIcon>
                            <ListItemText primary="Login" />
                        </ListItem>
                }
            </List>
        </Drawer>
    );
}

