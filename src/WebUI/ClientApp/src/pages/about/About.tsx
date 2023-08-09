import React from "react";
import { BottomNavigation, Box, Breadcrumbs, Button, ButtonGroup, Chip, Container, Grid, Hidden, Link, Typography } from "@material-ui/core";
import NextWeekOutlinedIcon from '@material-ui/icons/NextWeekOutlined';
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router";

// local
import { GotoProductsIconStyle, GotoProductsStyle } from "./AboutStyle";
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
                src="https://www.diagnosisdiet.com/assets/images/c/fruit-og-d176ef00.jpg" 
                style={{
                    width:"1200px",    
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
            <Container 
                maxWidth="lg" 
                style={{
                    padding:"0",
                    textAlign:"center"
                }}
            >
                <Box 
                    boxShadow={3}
                    borderRadius={6}
                    style={{
                        backgroundColor:"white",
                    }}
                >
                    {hash == HashValues.contact && <Faq isAdmin={isAdmin}/>}
                    {hash.startsWith(HashValues.about) && <About hash={hash}/>}
                    {hash == HashValues.info && <Information isAdmin={isAdmin}/>}
                </Box>
            </Container>
        </Box>
    </>
}