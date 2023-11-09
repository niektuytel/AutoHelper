import React, { useEffect, useState } from 'react';
import { Box, Button, Card, IconButton, Paper, Tab, Table, TableBody, TableCell, TableRow, Tabs, Tooltip, Typography, Drawer, List, ListItem } from "@mui/material";
import CarRepairIcon from '@mui/icons-material/CarRepair';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import CloseIcon from '@mui/icons-material/Close';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
import Divider from '@mui/material/Divider';

// own imports
import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router';
import VehicleServiceLogs from '../components/VehicleServiceLogs';
import VehicleInformation from '../components/VehicleInformation';



const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
    }
}

const tabsConfig = [
    { hash: "#service_logs", label: 'Maintenance logs', icon: <CarRepairIcon fontSize='medium' /> },
    { hash: "#mot_history", label: 'Tijdlijn', icon: <DirectionsCarFilledIcon fontSize='medium' /> },
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

    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => {
        if (event.type === 'keydown' && ((event as React.KeyboardEvent).key === 'Tab' || (event as React.KeyboardEvent).key === 'Shift')) {
            return;
        }
        setDrawerOpen(open);
    };

    const handleAddService = () => {
        setDrawerOpen(true);
    };

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
        navigate(tabsConfig[newValue].hash);
    };

    return (
        <>
            <Tabs value={value} onChange={handleChange} aria-label="icon label tabs example" sx={{ flexGrow: 1 }}>
                {tabsConfig.map((tab, index) => (
                    <Tab
                        key={index}
                        icon={tab.icon}
                        label={t(tab.label)}
                        sx={{ textTransform: 'none' }}
                    />
                ))}
            </Tabs>
            <Box sx={{ marginBottom: "40px" }}>
                <Paper variant="outlined" sx={{ borderRadius: 1, overflow: "hidden" }}>
                    <Box display="flex" justifyContent="space-between" alignItems="center" padding={1}>
                        <Typography variant={"h4"} sx={textStyles.root}>
                            {t("Service logs")}
                            <Tooltip title={t("Service logs.description")}>
                                <IconButton size="small">
                                    <InfoOutlinedIcon fontSize="inherit" />
                                </IconButton>
                            </Tooltip>
                        </Typography>
                        <Button
                            variant="outlined"
                            onClick={handleAddService}
                            startIcon={<AddCircleOutlineIcon />}
                        >
                            {t("Onderhoud Toevoegen")}
                        </Button>
                    </Box>
                    <Divider />
                    {value === 0 ?
                        <VehicleServiceLogs isMobile={isMobile} license_plate={license_plate} />
                        :
                        <VehicleInformation isMobile={isMobile} license_plate={license_plate} />
                    }
                </Paper>
            </Box>

            <Drawer anchor="right" open={drawerOpen} onClose={toggleDrawer(false)}>
                <Box
                    sx={{ width: 250 }}
                    role="presentation"
                    onKeyDown={toggleDrawer(false)}
                >
                    <Box display="flex" justifyContent="space-between" alignItems="center" p={1}>
                        <Typography variant="h6" component="div">
                            Upload Image
                        </Typography>
                        <IconButton onClick={toggleDrawer(false)}>
                            <CloseIcon />
                        </IconButton>
                    </Box>
                    <Divider />
                    <List>
                        <ListItem>
                            {/* Implement your image upload functionality here */}
                            {/* Placeholder for image upload - replace this with your component or implementation */}
                            <Button variant="contained" component="label">
                                Upload File
                                <input type="file" hidden />
                            </Button>
                        </ListItem>
                    </List>
                    <Divider />
                    <Box p={1}>
                        <Button fullWidth variant="contained" color="primary">
                            Confirm
                        </Button>
                    </Box>
                </Box>
            </Drawer>
        </>
    );
}
