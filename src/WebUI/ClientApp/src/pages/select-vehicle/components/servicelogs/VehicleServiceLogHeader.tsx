import React, { useContext, useEffect, useState } from "react";
import {
    Typography,
    Box,
    Divider,
    Tooltip,
    IconButton
} from '@mui/material';
import { useTranslation } from "react-i18next";
import AddIcon from '@mui/icons-material/Add';

// own imports
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
        <Box sx={{ p: "3px", backgroundColor:"#f7f7f7" }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Typography variant="h5" component="div" sx={{ fontWeight: 'bold', fontFamily: 'Dubai, sans-serif' }}>
                    {t("")}
                </Typography>

                <Tooltip title={t('VehicleServiceLogHeader.AddButton')}>
                    <IconButton color="primary" onClick={() => toggleDrawer(true)}>
                        <AddIcon />
                    </IconButton>
                </Tooltip>
            </Box>
        </Box>
        <Divider sx={{ my: 0 }} />
    </>
}