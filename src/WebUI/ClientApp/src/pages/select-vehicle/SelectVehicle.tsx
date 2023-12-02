import React from "react";
import { Container, useTheme, useMediaQuery, Fab} from "@mui/material";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useParams } from "react-router-dom";

// own imports
import GradientBox from "./components/GradientBox";
import HomeHeaderView from "./views/HomeHeaderView";
import HomeBodyView from "./views/HomeBodyView";
import VehicleHeaderView from "./views/VehicleHeaderView";
import VehicleBodyView from "./views/VehicleBodyView";
import { ROUTES } from "../../constants/routes";
import ServiceLogDrawerProvider, { useDrawer } from "./ServiceLogDrawerProvider";
import ServiceLogDrawer from "./components/ServiceLogDrawer";
import { useTranslation } from "react-i18next";

interface IProps {}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { license_plate } = useParams();
    const { t } = useTranslation();

    if (window.location.pathname === ROUTES.SELECT_VEHICLE) {
        return <>
            <GradientBox>
                <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight: "400px" }}>
                    <HomeHeaderView />
                </Container>
            </GradientBox>
            <Container maxWidth="lg" sx={{ padding: "0" }}>
                <HomeBodyView />
            </Container>
        </>
    }

    return (
        <>
            <ServiceLogDrawerProvider license_plate={license_plate || ""}>
                <GradientBox>
                    <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight:"400px" }}>
                        <VehicleHeaderView isMobile={isMobile} license_plate={license_plate || ""}/>
                    </Container>
                </GradientBox>
                <Container maxWidth="lg" sx={{ padding: "0" }}>
                    <VehicleBodyView isMobile={isMobile} license_plate={license_plate || ""}/>
                </Container>
                <ServiceLogDrawer licensePlate={license_plate || ""}/>
            </ServiceLogDrawerProvider>
        </>
    );
}
