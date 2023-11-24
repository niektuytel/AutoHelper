import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router';
import { Box, Paper, Typography } from "@mui/material";
import TimelineIcon from '@mui/icons-material/History';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
import ServicelogsIcon from '@mui/icons-material/Notes';
import { useTranslation } from 'react-i18next';

// own imports
import VehicleServiceLogs from '../components/VehicleServiceLogs';
import VehicleSpecifications from '../components/VehicleSpecifications';
import VehicleTimeline from '../components/VehicleTimeline';

const tabsConfig = [
    { hash: "#mot_history", label: 'Tijdlijn', icon: <TimelineIcon fontSize='medium' /> },
    { hash: "#service_logs", label: 'Maintenance logs', icon: <ServicelogsIcon fontSize='medium' /> },
    { hash: "#information", label: 'Information', icon: <DirectionsCarFilledIcon fontSize='medium' /> },
];

const findTabValueByHash = (hash: string) => {
    const tabIndex = tabsConfig.findIndex(tab => tab.hash === hash);
    return tabIndex !== -1 ? tabIndex : 1;
};

interface IProps {
    isMobile: boolean,
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { t } = useTranslation();
    const location = useLocation();
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState(findTabValueByHash(location.hash));
    const handleCardClick = (index: number) => {
        setActiveTab(index);
        navigate(tabsConfig[index].hash);
    };

    return (
        <>
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
            <Box sx={{ marginBottom: "40px", mx:1 }}>
                {activeTab === 0 ?
                    <VehicleTimeline isMobile={isMobile} license_plate={license_plate} />
                    :
                    activeTab === 1 ?
                        <VehicleServiceLogs isMobile={isMobile} license_plate={license_plate} />
                        :
                        <VehicleSpecifications isMobile={isMobile} license_plate={license_plate} />
                }
            </Box>
        </>
    );
}
