import React from "react";
import { Container, useTheme, useMediaQuery, Fab} from "@mui/material";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

// own imports
import GradientBox from "../../components/GradientBox";
import HomeSelectVehicle from "./components/HomeSelectVehicle";
import HomeAboutUs from "./components/HomeAboutUs";

interface IProps {}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { license_plate } = useParams();
    const { t } = useTranslation();

    return <>
        <GradientBox>
            <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight: "400px" }}>
                <HomeSelectVehicle />
            </Container>
        </GradientBox>
        <Container maxWidth="lg" sx={{ padding: "0" }}>
            <HomeAboutUs />
        </Container>
    </>
}
