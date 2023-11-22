import React, { useEffect, useState } from 'react';
import { Box, Button, Card, IconButton, Paper, Tab, Table, TableBody, TableCell, TableRow, Tabs, Tooltip, Typography, Drawer, List, ListItem } from "@mui/material";
import CarRepairIcon from '@mui/icons-material/CarRepair';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import TimelineIcon from '@mui/icons-material/History';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import CloseIcon from '@mui/icons-material/Close';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
import Divider from '@mui/material/Divider';

// own imports
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router';
import VehicleServiceLogs from '../components/VehicleServiceLogs';
import VehicleSpecifications from '../components/VehicleSpecifications';
import ServiceLogForm from '../components/ServiceLogForm';
import { blue } from '@mui/material/colors';
import VehicleTimeline from '../components/VehicleTimeline';
import ServicelogsIcon from '@mui/icons-material/Notes';



const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
    }
}

const tabsConfig = [
    { hash: "#service_logs", label: 'Maintenance logs', icon: <ServicelogsIcon fontSize='medium' /> },
    { hash: "#mot_history", label: 'Tijdlijn', icon: <TimelineIcon fontSize='medium' /> },
    { hash: "#information", label: 'Information', icon: <DirectionsCarFilledIcon fontSize='medium' /> },
];

const findTabValueByHash = (hash: string) => {
    const tabIndex = tabsConfig.findIndex(tab => tab.hash === hash);
    return tabIndex !== -1 ? tabIndex : 0;
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

    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => {
        console.log("toggleDrawer");
        setDrawerOpen(open);
    };

    const handleCardClick = (index: number) => {
        setActiveTab(index);
        navigate(tabsConfig[index].hash);
    };

    return (
        <>
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2, mb: 1 }}>
                {tabsConfig.map((tab, index) => (
                    <Card
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
                                backgroundColor: 'primary.light',
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
                    </Card>
                ))}
            </Box>
            <Box sx={{ marginBottom: "40px", mx:1 }}>
                {activeTab === 0 ?
                    <VehicleServiceLogs isMobile={isMobile} license_plate={license_plate} setDrawerOpen={setDrawerOpen} />
                    :
                    activeTab === 1 ?
                        <VehicleTimeline isMobile={isMobile} license_plate={license_plate} />
                        :
                        <VehicleSpecifications isMobile={isMobile} license_plate={license_plate} />
                }
            </Box>
            <ServiceLogForm licensePlate={license_plate} drawerOpen={drawerOpen} toggleDrawer={toggleDrawer} />
        </>
    );
}
