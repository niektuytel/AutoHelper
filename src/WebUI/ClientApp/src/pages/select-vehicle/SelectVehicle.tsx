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
    Tabs,
    Tab
} from "@mui/material";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import TextFieldPlaces from "./components/TextFieldPlaces";
import TextFieldLicensePlates from "./components/LicensePlateTextField";
import GradientBox from "./components/GradientBox";
import LicensePlateHeadView from "./views/LicensePlateHeadView";
import LicensePlateBodyView from "./views/LicensePlateBodyView";
import PlacesHeadView from "./views/PlacesHeadView";


import CarRepair from '@mui/icons-material/CarRepair';
import Dashboard from '@mui/icons-material/Dashboard';
import Archive from '@mui/icons-material/Archive';

function HeaderBar() {
    const defaultTabValue = window.location.hash || HashValues.select_vehicle_maintanance;
    const [tabValue, setTabValue] = React.useState(defaultTabValue);

    const handleChange = (event: React.SyntheticEvent, newValue: string) => {
        setTabValue(newValue);
        navigateTo(newValue);
    };

    const navigateTo = (hash: string) => {
        if (window.location.hash !== hash) {
            window.location.hash = hash;
        }
    };

    const toCamelCase = (text: string) => {
        return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
    };

    return (
        <Container maxWidth="lg">
            <Tabs
                value={tabValue}
                onChange={handleChange}
                aria-label="Navigation Tabs"
                centered
                sx={{
                    '.MuiTab-root': {  // Targets the individual tabs
                        height: '30px',
                        minHeight: '30px',  // Overrides default minHeight
                    },
                    height: '30px',  // Sets the height of the Tabs container
                }}
            >

                <Tab
                    label={toCamelCase("Onderhoud")}
                    value={HashValues.select_vehicle_maintanance}
                    sx={{ color: tabValue === HashValues.select_vehicle_maintanance ? "#1C94F3" : "inherit" }}
                    icon={<CarRepair />}
                    iconPosition="start"
                />
                <Tab
                    label={toCamelCase("Overview")}
                    value={HashValues.select_vehicle_overview}
                    sx={{ color: tabValue === HashValues.select_vehicle_overview ? "#1C94F3" : "inherit" }}
                    icon={<Dashboard />}
                    iconPosition="start"
                />
                <Tab
                    label={toCamelCase("History")}
                    disabled
                    icon={<Archive />}
                    iconPosition="start"
                />
            </Tabs>
        </Container>
    );
}
interface IProps {
}

export default ({ }: IProps) => {
    const { licence_plate } = useParams();
    const hash = window.location.hash.length == 0 ? HashValues.select_vehicle_default : window.location.hash;
    console.log(hash, licence_plate)

    const showOnPlacesSearch = (licence_plate && hash == HashValues.select_vehicle_maintanance);
    const showOnOverviewSearch = (licence_plate && hash == HashValues.select_vehicle_overview);

    return (
        <>
            {(showOnPlacesSearch || showOnOverviewSearch) && <HeaderBar/>}
            <GradientBox>
                <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center" }}>
                    {showOnPlacesSearch ?
                        <PlacesHeadView licence_plate={licence_plate}/>
                        : showOnOverviewSearch ?
                        <PlacesHeadView licence_plate={licence_plate}/>
                        :
                        <LicensePlateHeadView/>
                    }
                </Container>
            </GradientBox>
            <Container maxWidth="lg">
                {showOnPlacesSearch ?
                    <LicensePlateBodyView/>
                    : showOnOverviewSearch ?
                    <LicensePlateBodyView />
                    :
                    <LicensePlateBodyView/>
                }
            </Container>
        </>
    );
}
