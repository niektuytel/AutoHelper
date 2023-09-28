import React from "react";
import {
    Box,
    Hidden,
    Container,
    Typography,
    Grid,
    Card,
    CardMedia,
    CardContent,
    AppBar,
    Toolbar,
    Button,
    styled,
    Paper,
    Theme,
    useTheme,
    useMediaQuery} from "@mui/material";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// own imports
import GradientBox from "./components/GradientBox";
import HomeHeaderView from "./views/HomeHeaderView";
import HomeBodyView from "./views/HomeBodyView";
import VehicleHeaderView from "./views/VehicleHeaderView";
import VehicleBodyView from "./views/VehicleBodyView";
import { ROUTES } from "../../constants/routes";

interface IProps {}


export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { license_plate } = useParams();

    return (
        <>
            <GradientBox>
                <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight:"400px" }}>
                    { window.location.pathname === ROUTES.SELECT_VEHICLE ?
                        <HomeHeaderView/>
                        :
                        <VehicleHeaderView isMobile={isMobile} license_plate={license_plate || ""}/>
                    }
                </Container>
            </GradientBox>
            <Container maxWidth="lg" sx={{ padding: "0" }}>
                { window.location.pathname === ROUTES.SELECT_VEHICLE ? 
                    <HomeBodyView/>
                    :
                    <VehicleBodyView isMobile={isMobile} license_plate={license_plate || ""}/>
                }
            </Container>
        </>
    );
}
