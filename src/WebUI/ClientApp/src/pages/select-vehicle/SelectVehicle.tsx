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

// local
import { HashValues } from "../../i18n/HashValues";
import TextFieldLicensePlates from "./components/LicensePlateTextField";
import GradientBox from "./components/GradientBox";
import LicensePlateHeadView from "./views/LicensePlateSearchView";
import AutoHelperAboutView from "./views/AutoHelperAboutView";
import PlacesHeadView from "./views/PlacesSearchView";


import VehicleHeaderBar from "./components/VehicleHeaderBar";
import VehicleInfoView from "./views/VehicleInfoView";
import { ROUTES } from "../../constants/routes";

interface StyledProps {
    theme: Theme;
}

const DemoPaper = styled(Paper)<StyledProps>(({ theme }) => ({
    width: 120,
    height: 120,
    padding: theme.spacing(2),
    ...theme.typography.body2,
    textAlign: 'center',
}));

interface IProps {}


export default ({ }: IProps) => {
    const { licence_plate } = useParams();
    const hash = window.location.hash.length == 0 ? HashValues.select_vehicle_default : window.location.hash;

    const showOnMaintanance = (licence_plate && hash == HashValues.select_vehicle_maintanance);
    const showOnInfo = (licence_plate && hash == HashValues.select_vehicle_info);

    return (
        <>
            {!showOnInfo && 
                <GradientBox>
                    <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center" }}>
                        {showOnMaintanance ?
                            <PlacesHeadView licence_plate={licence_plate || ""}/>
                            :
                            <LicensePlateHeadView/>
                        }
                    </Container>
                </GradientBox>
            }
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
