﻿import React from "react";
import { Container, useTheme, useMediaQuery} from "@mui/material";
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
import ServiceLogDrawerProvider from "./ServiceLogDrawerProvider";
import ServiceLogForm from "./components/ServiceLogForm";

interface IProps {}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { license_plate } = useParams();

    return (
        <>
            <ServiceLogDrawerProvider license_plate={license_plate || ""}>
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
                <ServiceLogForm licensePlate={license_plate || ""}/>
            </ServiceLogDrawerProvider>
        </>
    );
}
