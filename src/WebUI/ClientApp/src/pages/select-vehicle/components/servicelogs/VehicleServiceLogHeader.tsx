import React, { useContext, useEffect, useState } from "react";
import {
    Typography,
    Chip,
    Box,
    Skeleton,
    Divider,
    Paper,
    Button,
    Popover,
    Tooltip
} from '@mui/material';
import GarageIcon from '@mui/icons-material/CarRepair';
import SpeedIcon from '@mui/icons-material/Speed';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import VerifiedIcon from '@mui/icons-material/Verified';
import NewReleasesIcon from '@mui/icons-material/NewReleases';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { useTranslation } from "react-i18next";
import AddIcon from '@mui/icons-material/Add';
import { useLocation, useNavigate } from "react-router";

// own imports
import { GarageServiceType, VehicleServiceLogDtoItem, VehicleServiceLogStatus } from "../../../../app/web-api-client";
import { ROUTES } from "../../../../constants/routes";
import { ServiceLogDrawerContext } from "../../../../context/ServiceLogDrawerContext";

interface IProps {
    isMobile: boolean;
}

export default ({ isMobile }: IProps) => {
    const { t } = useTranslation(["translations"]);
    const context = useContext(ServiceLogDrawerContext);

    if (!context) {
        throw new Error("DrawerComponent must be used within a DrawerProvider");
    }

    const { toggleDrawer } = context;

    return <>
        <Box sx={{ p: 1, backgroundColor:"#f7f7f7" }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Typography variant="h5" component="div" sx={{ fontWeight: 'bold', fontFamily: 'Dubai, sans-serif' }}>
                    {t("VehicleServiceLogHeader.Title")}
                </Typography>
                <Button
                    variant="outlined"
                    onClick={() => toggleDrawer(true)}
                    sx={{
                        minWidth: 'auto', // Removes the minimum width
                        borderRadius: '50%', // Makes the button circular
                        border: 'none',    // Removes the border
                        '&:hover': {
                            backgroundColor: 'transparent', // Maintains transparency on hover
                            border: 'none',
                        }
                    }}
                >
                    <AddIcon sx={{ fontSize: 'large' }} />
                </Button>
            </Box>
        </Box>
        <Divider sx={{ my: 0 }} />
    </>
}