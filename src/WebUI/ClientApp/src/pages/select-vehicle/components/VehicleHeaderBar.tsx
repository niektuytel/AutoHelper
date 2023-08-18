import React from "react";
import {
    Container, Tabs,
    Tab,
    Box
} from "@mui/material";
import { HashValues } from "../../../i18n/HashValues";
import CarRepair from '@mui/icons-material/CarRepair';
import Dashboard from '@mui/icons-material/Dashboard';
import Archive from '@mui/icons-material/Archive';

interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {
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

    return (
        <Box sx={{ boxShadow: '0px 4px 6px -1px rgba(0,0,0,0.1)' }}>
            <Container maxWidth="lg">
                <Tabs
                    sx={{
                        padding: 0,
                        marginLeft: "10px",
                        '.MuiTab-root': {
                            height: '50px',
                            minHeight: '50px',
                            '&.Mui-selected': {
                                color: '#1C94F3',
                            },
                        },
                        height: '50px', // Sets the height of the Tabs container
                    }}
                    value={tabValue}
                    onChange={handleChange}
                    aria-label="Navigation Tabs"
                    variant="scrollable"
                    TabIndicatorProps={{ style: { background: "#1C94F3", color: "#1C94F3" } }}
                >
                    <Tab
                        label={`Onderhoud`}
                        value={HashValues.select_vehicle_maintanance}
                        sx={{ color: "inherit", textTransform: "none" }}
                        icon={<CarRepair />}
                        iconPosition="start" />
                    <Tab
                        label={`Informatie (${licence_plate})`}
                        value={HashValues.select_vehicle_info}
                        sx={{ color: "inherit", textTransform: "none" }}
                        icon={<Dashboard />}
                        iconPosition="start" />
                    
                </Tabs>
            </Container>
        </Box>
    );
}
