import React, { useEffect, useState } from 'react';
import { Box, Card, Paper, Tab, Table, TableBody, TableCell, TableRow, Tabs, Typography } from "@mui/material";
import CarRepairIcon from '@mui/icons-material/CarRepair';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
import Divider from '@mui/material/Divider';

// own imports
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router';
import VehicleServiceLogs from '../components/VehicleServiceLogs';
import VehicleInformation from '../components/VehicleInformation';


const tabsConfig = [
    { hash: "#service_logs", label: 'Maintenance logs', icon: <CarRepairIcon fontSize='medium' /> },
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
    const [value, setValue] = useState(findTabValueByHash(location.hash));

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
        navigate(tabsConfig[newValue].hash);
    };

    return (
        <>
            <Box sx={{ marginBottom: "40px" }}>
                <Paper elevation={3} variant="outlined" sx={{ borderRadius: 1, overflow: "hidden", margin: 1 }}>
                    <Tabs value={value} onChange={handleChange} aria-label="icon label tabs example">
                        {tabsConfig.map((tab, index) => (
                            <Tab
                                key={index}
                                icon={tab.icon}
                                label={t(tab.label)}
                                sx={{ textTransform: 'none' }}
                            />
                        ))}
                    </Tabs>
                    <Divider />
                    {value === 0 ?
                        <VehicleServiceLogs isMobile={isMobile} license_plate={license_plate} />
                        :
                        <VehicleInformation isMobile={isMobile} license_plate={license_plate} />
                    }
                </Paper>
            </Box>
        </>
    );
}
