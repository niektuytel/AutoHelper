import React from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden } from "@mui/material";
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";


// local
import { HashValues } from "../../i18n/HashValues";

import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import TextFieldPlaces from "./components/TextFieldPlaces";

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    return <>
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
                    <TextFieldPlaces />
                </Box>

            </Container>
        </Box>
    </>
}