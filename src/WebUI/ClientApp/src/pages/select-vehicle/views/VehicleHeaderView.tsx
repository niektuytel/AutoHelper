import React from 'react';
import { Box, Card, CardContent, Grid, Hidden, Typography } from "@mui/material";
import TextFieldLicensePlates from "../components/LicensePlateSearchField";
import LocationSearchField from '../components/LocationSearchField';
import { useParams } from 'react-router-dom';
import VehicleBriefInformation from '../components/VehicleBriefInformation';
import CustomizedTimeline from '../components/TimelineBox';

interface IProps {
    isMobile: boolean;
    license_plate: string;
}

export default ({ isMobile, license_plate }: IProps) => {

    return (
        <>
            <Hidden mdDown>
                <Box sx={{ alignSelf: "left", marginTop: "10px" }}>
                    <Typography variant="h4" color="white" align="left">
                        <b>Zoek Garage</b>
                    </Typography>
                    <Typography variant="body1" color="white" align="left">
                        <b>voor onderhoud</b>
                    </Typography>
                    <LocationSearchField licence_plate={license_plate} />
                </Box>
                <Grid container sx={{ minHeight: "50vh" }} >
                    <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "25px" }}>
                        <CustomizedTimeline/>
                    </Grid>
                    <Grid item xs={6} sx={{ marginTop: "25px", paddingLeft:"50px" }}>
                        <VehicleBriefInformation isMobile={isMobile} license_plate={license_plate} />
                    </Grid>
                </Grid>
            </Hidden>
            <Hidden mdUp>
                <Box sx={{ alignSelf: "center", marginTop: "20px" }}>
                    <Typography variant="h4" color="white">
                        <b>Zoek Garage</b>
                    </Typography>
                    <Typography variant="body1" color="white">
                        <b>voor onderhoud</b>
                    </Typography>
                    <LocationSearchField licence_plate={license_plate} />
                    <Box sx={{ marginTop: "25px" }}>
                        <VehicleBriefInformation isMobile={isMobile} license_plate={license_plate} />
                    </Box>
                </Box>
            </Hidden>
        </>
    );
}
