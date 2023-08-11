import React from "react";
import { Box, Container, Typography } from "@mui/material";
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

    
    return (
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
                    {licence_plate ?
                        <TextFieldPlaces licence_plate={licence_plate} />
                        :
                        <TextFieldLicencePlates/>
                    }
                    {/*
                    Voordelen van autohelper, in een paar klikken:(should be 2 cards horizontal aligned with an image and an title and an body)
                    - Onderhoud geven
                    - Informatie uitlezen
                    - Geschiedenis uitlezen

                    Stel je eens voor een website waar je alles kan inzien en alles kan regelen voor je auto?
                    Tja, hier is autohelper.nl door onstaan om online je auto reparatie of onderhoud te regelen met 
                    de garage die je wil. Het ophalen en brengen van je auto is ook mogelijk door een goede planning 
                    aan te bieden bij je garage. Met ons motto 'makkelijk is al moeilijk genoeg' streven wij naar een perfecte 
                    service voor klant en garage. 

                    Als u een service via de autohelper.nl laat lopen of u garage laat het via de autohelper.nl lopen dan
                    worden de reparaties of onderhouds informatie opgelagen, en is inleesbaar door alle gebruikers

                    Met Vriedelijke groeten.
                    AutoHelper.nl

                    */}
                </Box>
            </Container>
        </Box>
    );
}
