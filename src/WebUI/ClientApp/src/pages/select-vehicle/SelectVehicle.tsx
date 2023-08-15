import React from "react";
import {
    Box,
    Hidden,
    Container,
    Typography,
    Grid,
    Card,
    CardMedia,
    CardContent
} from "@mui/material";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import TextFieldPlaces from "./components/TextFieldPlaces";
import TextFieldLicencePlates from "./components/TextFieldLicencePlates";

interface IProps {
}

export default ({ }: IProps) => {
    const { licence_plate } = useParams();
    const hash = window.location.hash.length == 0 ? HashValues.select_vehicle_default : window.location.hash;
    console.log(hash, licence_plate)


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

    if (licence_plate) {
        <>
            <div>
                {/*TODO: menu bar on the top with the fullwidth and an shadow on the bottom with button: [onderhoud, overview, history]*/}
                {/*de 3 knoppen worden bestuurd door de hash. select_vehicle_maintanance is voor de onderhoud, overview is de select_vehicle_overview  en history nog even uitcommenten omdat die later komt*/}
            </div>
            <Box
                style={{
                    position: "relative",
                    marginLeft: "10px",
                    marginRight: "10px"
                }}
            >
                <Container
                    maxWidth="lg"
                    style={{
                        padding: "0",
                        textAlign: "center"
                    }}
                >
                    <Box
                        style={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center'
                        }}
                    >
                        {hash == HashValues.select_vehicle_maintanance && 
                            <TextFieldPlaces licence_plate={licence_plate} />
                        }
                    </Box>
                </Container>
            </Box>
        </>
    }

    return (
        <>
            <Box
                sx={{
                    position: "relative",
                    background: 'linear-gradient(80deg, #1C94F3 50%, transparent 50%)',
                    height: "100%",
                    padding: "10px"
                }}
            >
                <Hidden xsDown>
                </Hidden>
                <Hidden xsUp>
                </Hidden>
                <Container
                    maxWidth="lg"
                    sx={{
                        padding: "0",
                        textAlign: "center",
                    }}
                >
                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                            position: 'relative'
                        }}
                    >
                        <TextFieldLicencePlates />
                        <Grid container sx={{ minHeight: "50vh" }} >
                            <Grid item xs={6} sx={{ textAlign: 'left', marginTop: "30px" }}>
                                <Typography variant="h2" color="white">
                                    <b>Bekijk voertuig</b>
                                </Typography>
                                <Typography variant="h6" color="white">
                                    <b>voor je onderhoud en informatie</b>
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
                    </Box>
                </Container>
            </Box>
            <Container maxWidth="lg">
                <Typography variant="h5" gutterBottom>
                    Voordelen van autohelper, in een paar klikken:
                </Typography>
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
                <Box sx={{margin:"20px"}}>
                    <Typography gutterBottom>
                        Welkom bij AutoHelper.nl, hét online platform voor al uw autozaken. Op onze website kunt u eenvoudig alle benodigde informatie over uw voertuig inzien en beheren. Wij zorgen voor een gestroomlijnde ervaring bij zowel autoreparaties als regulier onderhoud. U selecteert de gewenste garage en bepaalt zelf of u de auto wilt ophalen of afzetten bij de garage. Ongeacht uw keuze, houden wij u nauwgezet op de hoogte van de status van uw auto.
                    </Typography>
                    <br/>
                    <Typography gutterBottom>
                        Bij gebruik van onze diensten, of wanneer uw garage onze platform gebruikt voor het uitvoeren van diensten aan uw auto, wordt alle relevante informatie over de reparaties of onderhoud centraal opgeslagen. Dit zorgt voor een transparant overzicht, toegankelijk voor alle betrokken partijen.
                    </Typography>
                    <br />
                    <Typography gutterBottom>
                        Ons streven is om het beheer van autozaken zo eenvoudig mogelijk te maken. Zoals ons motto luidt: 'Makkelijk is al moeilijk genoeg'. We zetten ons in voor optimale tevredenheid van zowel klant als garage.
                    </Typography>
                </Box>

            </Container>
        </>
    );

    //{/*
    //Voordelen van autohelper, in een paar klikken:(should be 2 cards horizontal aligned with an image and an title and an body)
    //- Onderhoud geven
    //- Informatie uitlezen
    //- Geschiedenis uitlezen

    //Stel je eens voor een website waar je alles kan inzien en alles kan regelen voor je auto?
    //Tja, hier is autohelper.nl door onstaan om online je auto reparatie of onderhoud te regelen met 
    //de garage die je wil. Het ophalen en brengen van je auto is ook mogelijk door een goede planning 
    //aan te bieden bij je garage. Met ons motto 'makkelijk is al moeilijk genoeg' streven wij naar een perfecte 
    //service voor klant en garage. 

    //Als u een service via de autohelper.nl laat lopen of u garage laat het via de autohelper.nl lopen dan
    //worden de reparaties of onderhouds informatie opgelagen, en is inleesbaar door alle gebruikers

    //Met Vriedelijke groeten.
    //AutoHelper.nl

    //*/}
}
