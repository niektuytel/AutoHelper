import React from 'react';
import { Box, Card, CardContent, Grid, Hidden, Typography } from "@mui/material";
import Slider from 'react-slick';
import { useTranslation } from 'react-i18next';

export default () => {
    const { t } = useTranslation();
    // TODO: add translations

    const settings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 3,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 960, // when window width is <= 960px
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            }
        ]
    };

    const items = [
        { image: "/images/license_plate.svg", title: "Vertel ons welke auto je zoekt", description: "Wij laten je een goede inkijk nemen in de informatie over je auto" },
        { image: "/images/pin.svg", title: "Vertel ons waar je je bevindt", description: "Wij laten je zien bij welke garages in de buurt je kan onderhouden." },
        { image: "/images/location.svg", title: "Afspraak met afhalen of afzetten", description: "We houden je op de hoogte van de voortgang van je onderhoud." }
    ];

    return (
        <>
            <Box sx={{ padding: "5vh" }}>
                <Typography variant="h6" color="black" style={{ textAlign: 'center' }}>
                    Zo werkt het
                </Typography>
                <Typography variant="h4" color="#1C94F3" style={{ textAlign: 'center' }}>
                    <b>Makkelijker kan haast niet.</b>
                </Typography>
            </Box>
            <Slider {...settings}>
                {items.map((item, index) => (
                    <Card key={index} elevation={0}>
                        <img
                            src={item.image}
                            style={{ height: "75px", width: "-webkit-fill-available" }}
                            alt={item.title}
                        />
                        <CardContent>
                            <Typography variant="h6" component="div" style={{ textAlign: 'center' }}>
                                {item.title}
                            </Typography>
                            <Typography variant="body2" color="text.secondary" style={{ textAlign: 'center' }}>
                                {item.description}
                            </Typography>
                        </CardContent>
                    </Card>
                ))}
            </Slider>
            <Box sx={{ margin: "20px", paddingBottom:"40px" }}>
                <Typography gutterBottom>
                    Welkom bij AutoHelper.nl, hét online platform voor al uw autozaken. Op onze website kunt u eenvoudig alle benodigde informatie over uw voertuig inzien en beheren. Wij zorgen voor een gestroomlijnde ervaring bij zowel autoreparaties als regulier onderhoud. U selecteert de gewenste garage en bepaalt zelf of u de auto wilt ophalen of afzetten bij de garage. Ongeacht uw keuze, houden wij u nauwgezet op de hoogte van de status van uw auto.
                </Typography>
                <br />
                <Typography gutterBottom>
                    Bij gebruik van onze diensten, of wanneer uw garage onze platform gebruikt voor het uitvoeren van diensten aan uw auto, wordt alle relevante informatie over de reparaties of onderhoud centraal opgeslagen. Dit zorgt voor een transparant overzicht, toegankelijk voor alle betrokken partijen.
                </Typography>
                <br />
                <Typography gutterBottom>
                    Ons streven is om het beheer van autozaken zo eenvoudig mogelijk te maken. Zoals ons motto luidt: 'Makkelijk is al moeilijk genoeg'. We zetten ons in voor optimale tevredenheid van zowel klant als garage.
                </Typography>
            </Box>
        </>
    );
}
