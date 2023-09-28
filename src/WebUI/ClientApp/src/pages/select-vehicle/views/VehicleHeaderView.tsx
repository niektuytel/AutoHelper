import React from 'react';
import { Box, Grid, Hidden, Typography } from "@mui/material";
import TextFieldLicensePlates from "../components/LicensePlateSearchField";
import LocationSearchField from '../components/LocationSearchField';
import { useParams } from 'react-router-dom';
import VehicleBriefInformation from '../components/VehicleBriefInformation';

interface IProps {
    isMobile: boolean;
    license_plate: string;
}

export default ({ isMobile, license_plate }: IProps) => {

    return (
        <>
            <Hidden mdDown>
                <LocationSearchField licence_plate={license_plate} />
                <Grid container sx={{ minHeight: "50vh" }} >
                    <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "30px" }}>
                        <Typography variant="h2" color="white">
                            <b>Bestel onderhoud</b>
                        </Typography>
                        <Typography variant="h6" color="white">
                            <b>met de garage in de buurt</b>
                        </Typography>
                    </Grid>
                    <Grid item xs={6} sx={{ marginTop: "25px", paddingLeft:"50px" }}>
                        <VehicleBriefInformation isMobile={isMobile} license_plate={license_plate} />
                    </Grid>
                </Grid>
            </Hidden>
            <Hidden mdUp>
                <Box sx={{ alignSelf: "center", marginTop: "20px" }}>
                    <Typography variant="h4" color="white">
                        <b>Bestel onderhoud</b>
                    </Typography>
                    <Typography variant="body1" color="white">
                        <b>met de garage in de buurt</b>
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
