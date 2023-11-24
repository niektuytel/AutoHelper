import React from 'react';
import { Box, Grid, Hidden, Typography } from "@mui/material";
import { useTranslation } from 'react-i18next';

// custom imports
import SearchLocation from '../components/SearchLocation';
import VehicleSpecificationsCard from '../components/VehicleSpecificationsCard';
import VehicleTimelineCard from '../components/VehicleTimelineCard';

interface IProps {
    isMobile: boolean;
    license_plate: string;
}

export default ({ isMobile, license_plate }: IProps) => {
    const { t } = useTranslation();

    return (
        <>
            <Hidden mdDown>
                <Box sx={{ alignSelf: "left", marginTop: "10px" }}>
                    <Typography variant="h4" color="white" align="left">
                        <b>{t("VehiclePage.Title")}</b>
                    </Typography>
                    <Typography variant="body1" color="white" align="left">
                        <b>{t("VehiclePage.SubTitle")}</b>
                    </Typography>
                    <SearchLocation licence_plate={license_plate} />
                </Box>
                <Grid container>
                    <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "25px" }}>
                        <VehicleTimelineCard license_plate={license_plate} />
                    </Grid>
                    <Grid item xs={6} sx={{ marginTop: "25px", paddingLeft:"50px" }}>
                        <VehicleSpecificationsCard isMobile={isMobile} license_plate={license_plate} />
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
                    <SearchLocation licence_plate={license_plate} />
                    <Box sx={{ marginTop: "25px" }}>
                        <VehicleSpecificationsCard isMobile={isMobile} license_plate={license_plate} />
                    </Box>
                </Box>
            </Hidden>
        </>
    );
}
