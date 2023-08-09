import React from "react";
import { BottomNavigation, Box, Breadcrumbs, Button, ButtonGroup, Chip, Container, Grid, Hidden, Link, Typography } from "@material-ui/core";
import NextWeekOutlinedIcon from '@material-ui/icons/NextWeekOutlined';
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router";

// local
import { GotoProductsIconStyle, GotoProductsStyle } from "./HomeStyle";
import NavSection from "./components/NavSection";
import About from "./sections/about/About";
import Information from "./sections/InformationSection";
import Faq from "./sections/ContactSection";
import { HashValues } from "../../i18n/HashValues";
import ReactPlayer from "react-player";
import TypedLogo, { TypedIconStyle } from "../../components/logo/TypedLogo";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const history = useHistory();
    const classes = TypedIconStyle();
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    return <>
        <div style={{width:"100%", position:"absolute"}}>
            <img 
                src="https://images.pexels.com/photos/3806249/pexels-photo-3806249.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" 
                style={{
                    height:"100%",    
                    margin: "auto",
                    position: "absolute",
                    left: "0",
                    right: "0"
                }}
            />
        </div>
        <Box 
            style={{
                // height:"90vh",
                position:"relative",
                marginLeft:"10px",
                marginRight:"10px"
            }}
        >
            <Box
                className="noselect"
                style={{
                    zIndex: 1,
                    cursor: "pointer",
                    width: "100%",
                    height: "65vh"
                }}
                onClick={() => history.push("/products")}
            >
                <Container 
                    maxWidth="lg" 
                    style={{
                        padding:"0", 
                        textAlign:"center"
                    }}
                >
                    <Box
                        style={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center'
                        }}
                    >
                        <Typography 
                            // variant={"subtitle2"} 
                            className={classes.root}
                            style={GotoProductsStyle}
                        >
                            {t('see_products')}
                        </Typography>
                    </Box>
                </Container>
            </Box>
        </Box>
    </>
}