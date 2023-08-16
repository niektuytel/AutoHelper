import React from 'react';
import { Box, Grid, Hidden, Typography } from "@mui/material";
import TextFieldLicensePlates from "../components/LicensePlateTextField";
import TextFieldPlaces from '../components/TextFieldPlaces';
import { useParams } from 'react-router-dom';

interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {

    return (
        <>
            <TextFieldPlaces licence_plate={licence_plate} />
            <Hidden mdDown>
                <Grid container sx={{ minHeight: "50vh" }} >
                    <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "30px" }}>
                        <Typography variant="h2" color="white">
                            <b>Bestel onderhoud</b>
                        </Typography>
                        <Typography variant="h6" color="white">
                            <b>met de garage in de buurt</b>
                        </Typography>
                    </Grid>
                    <Grid item xs={6} sx={{ alignSelf: "center" }}>
                        <img
                            src="/images/mauntain_with_car_key.png"
                            height="200px"
                            alt="Car key is not been found"
                        />
                    </Grid>
                </Grid>
            </Hidden>
            <Hidden mdUp>
                <Box sx={{ alignSelf: "center", marginTop: "10px" }}>
                    <Typography variant="h4" color="white">
                        <b>Bestel onderhoud</b>
                    </Typography>
                    <Typography variant="body1" color="white">
                        <b>met de garage in de buurt</b>
                    </Typography>
                    <img
                        src="/images/mauntain_with_car_key.png"
                        height="200px"
                        alt="Car key is not been found"
                    />
                </Box>
            </Hidden>
        </>
    );
}
