import React, { useState } from "react";
import { Container, useTheme, useMediaQuery, Fab, Paper} from "@mui/material";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useParams } from "react-router-dom";
import { Box, Grid, Hidden, Typography } from "@mui/material";
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router';
import TimelineIcon from '@mui/icons-material/History';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
import ServicelogsIcon from '@mui/icons-material/Notes';

// own imports
import SearchLocation from './components/SearchLocation';
import GradientBox from "../../components/GradientBox";
import VehicleSpecificationsCard from "./components/specifications/VehicleSpecificationsCard";
import VehicleTimelinesCard from "./components/timeline/VehicleTimelinesCard";
import VehicleTimelines from "./components/timeline/VehicleTimelines";
import VehicleServiceLogs from "./components/servicelogs/VehicleServiceLogs";
import VehicleSpecifications from "./components/specifications/VehicleSpecifications";
import ServiceLogDrawer from "./components/servicelogs/ServiceLogDrawer";

const tabsConfig = [
    { hash: "#mot_history", label: 'Tijdlijn', icon: <TimelineIcon fontSize='medium' /> },
    { hash: "#service_logs", label: 'Maintenance logs', icon: <ServicelogsIcon fontSize='medium' /> },
    { hash: "#information", label: 'Information', icon: <DirectionsCarFilledIcon fontSize='medium' /> },
];

const findTabValueByHash = (hash: string) => {
    const tabIndex = tabsConfig.findIndex(tab => tab.hash === hash);
    return tabIndex !== -1 ? tabIndex : 1;
};

interface IProps {}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { license_plate } = useParams();
    const { t } = useTranslation();
    const location = useLocation();
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState(findTabValueByHash(location.hash));
    const handleCardClick = (index: number) => {
        setActiveTab(index);
        navigate(tabsConfig[index].hash, { state: { from: location } });
    };

    
    return <>
        <GradientBox>
            <Container maxWidth="lg" sx={{ padding: "0", textAlign: "center", minHeight: "400px" }}>
                <Hidden mdDown>
                    <Box sx={{ alignSelf: "left", marginTop: "10px" }}>
                        <Typography variant="h4" color="white" align="left">
                            <b>{t("VehiclePage.Title")}</b>
                        </Typography>
                        <Typography variant="body1" color="white" align="left">
                            <b>{t("VehiclePage.SubTitle")}</b>
                        </Typography>
                        <SearchLocation licence_plate={license_plate || ""} />
                    </Box>
                    <Grid container>
                        <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "25px" }}>
                            <VehicleTimelinesCard license_plate={license_plate || ""} />
                        </Grid>
                        <Grid item xs={6} sx={{ marginTop: "25px", paddingLeft: "50px" }}>
                            <VehicleSpecificationsCard isMobile={isMobile} license_plate={license_plate || ""} />
                        </Grid>
                    </Grid>
                </Hidden>
                <Hidden mdUp>
                    <Box sx={{ alignSelf: "center", marginTop: "20px" }}>
                        <Typography variant="h4" color="white">
                            <b>{t("VehiclePage.Title")}</b>
                        </Typography>
                        <Typography variant="body1" color="white">
                            <b>{t("VehiclePage.SubTitle")}</b>
                        </Typography>
                        <SearchLocation licence_plate={license_plate || ""} />
                        <Box sx={{ marginTop: "25px" }}>
                            <VehicleSpecificationsCard isMobile={isMobile} license_plate={license_plate || ""} />
                        </Box>
                    </Box>
                </Hidden>
            </Container>
        </GradientBox>
        <Container maxWidth="lg" sx={{ padding: "0" }}>
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2, mb: 1 }}>
                {tabsConfig.map((tab, index) => (
                    <Paper
                        variant="outlined"
                        key={index}
                        sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: 100,
                            width: '100%',
                            margin: 1,
                            backgroundColor: activeTab === index ? 'primary.main' : 'background.paper',
                            color: activeTab === index ? 'common.white' : 'text.primary',
                            '&:hover': {
                                backgroundColor: 'primary.main',
                                color: 'common.white',
                                cursor: 'pointer'
                            },
                            textTransform: 'none',
                        }}
                        onClick={() => handleCardClick(index)}
                    >
                        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                            {tab.icon}
                            <Typography variant="subtitle1" sx={{ textAlign: "center" }}>
                                {t(tab.label)}
                            </Typography>
                        </Box>
                    </Paper>
                ))}
            </Box>
            <Box sx={{ marginBottom: "40px", mx: 1 }}>
                {activeTab === 0 ?
                    <VehicleTimelines isMobile={isMobile} license_plate={license_plate || ""} />
                    :
                    activeTab === 1 ?
                        <VehicleServiceLogs isMobile={isMobile} license_plate={license_plate || ""} />
                        :
                        <VehicleSpecifications isMobile={isMobile} license_plate={license_plate || ""} />
                }
            </Box>
        </Container>
        <ServiceLogDrawer licensePlate={license_plate || ""}/>
    </>;
}
