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
    Theme} from "@mui/material";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// own imports
import GradientBox from "./components/GradientBox";
import LicensePlateHeadView from "./views/LicensePlateSearchView";
import AutoHelperAboutView from "./views/AutoHelperAboutView";
import PlacesHeadView from "./views/PlacesSearchView";
import VehicleInfoView from "./views/VehicleInfoView";
import { ROUTES } from "../../constants/routes";

interface IProps {}


export default ({ }: IProps) => {
    const { licence_plate } = useParams();

    return (
        <>
            <GradientBox>
                <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight:"400px" }}>
                    { window.location.pathname === ROUTES.SELECT_VEHICLE ?
                        <LicensePlateHeadView/>
                        :
                        <PlacesHeadView licence_plate={licence_plate || ""}/>
                    }
                </Container>
            </GradientBox>
            <Container maxWidth="lg">
                { window.location.pathname === ROUTES.SELECT_VEHICLE ? 
                    <AutoHelperAboutView />
                    :
                    <VehicleInfoView licence_plate={licence_plate || ""}/>
                }
            </Container>
        </>
    );
}
