import React from 'react';
import { Box, Grid, Hidden, Typography } from "@mui/material";
import TextFieldLicensePlates from "../components/LicensePlateSearchField";
import PlacesTextField from '../components/PlacesTextField';
import { useParams } from 'react-router-dom';
import VehicleBriefInfoCard from '../components/VehicleBriefInfoCard';

interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {

    return (
        <>
            <Hidden mdDown>
                <PlacesTextField licence_plate={licence_plate} />
                <Grid container sx={{ minHeight: "50vh" }} >
                    <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "30px" }}>
                        <Typography variant="h2" color="white">
                            <b>Bestel onderhoud</b>
                        </Typography>
                        <Typography variant="h6" color="white">
                            <b>met de garage in de buurt</b>
                        </Typography>
                        {/*<img*/}
                        {/*    src="/images/carbridge.png"*/}
                        {/*    height="350px"*/}
                        {/*    alt="Car key is not been found"*/}
                        {/*/>*/}
                    </Grid>
                    <Grid item xs={6} sx={{ marginTop: "25px", paddingLeft:"50px" }}>
                        <VehicleBriefInfoCard license_plate={licence_plate} />
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
                    <PlacesTextField licence_plate={licence_plate} />
                    <Box sx={{ marginTop: "25px" }}>
                        <VehicleBriefInfoCard license_plate={licence_plate} />
                    </Box>
                    {/*<img*/}
                    {/*    src="/images/carbridge.png"*/}
                    {/*    height="200px"*/}
                    {/*    alt="Car key is not been found"*/}
                    {/*    style={{ marginTop:"20px"}}*/}
                    {/*/>*/}
                </Box>
            </Hidden>
        </>
    );
}
