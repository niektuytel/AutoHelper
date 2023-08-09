import React from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden } from "@material-ui/core";
import LocationOnOutlinedIcon from '@material-ui/icons/LocationOnOutlined';
import SearchIcon from '@material-ui/icons/Search';
import ClearIcon from '@material-ui/icons/Clear';
import { useTranslation } from "react-i18next";
import { useHistory } from "react-router";


// local
import Faq from "./sections/ContactSection";
import { HashValues } from "../../i18n/HashValues";
import ReactPlayer from "react-player";

import TypedLogo, { TypedIconStyle } from "../../components/logo/TypedLogo";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const history = useHistory();
    const classes = TypedIconStyle();
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    const [suggestions, setSuggestions] = React.useState<string[]>([]);
    const [inputValue, setInputValue] = React.useState<string>("");

    const handleClearInput = () => {
        setInputValue('');
    };
    /// GOOGLE MAPS API: AIzaSyDHOHHAjYZsfPQqy7qQaopZNcLWJMp6Re0

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setInputValue(event.target.value);
        // Here, you can also fetch and update the suggestions based on the input value.
    };

    const handleSearch = () => {
        // Handle the search functionality here
        // For now, let's just console.log the input value
        console.log(inputValue);
    }


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
                    <TextField
                        fullWidth
                        value={inputValue}
                        onChange={handleInputChange}
                        variant="outlined"
                        placeholder="Adres, b.v. Amstelplein 10"
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <LocationOnOutlinedIcon color="action" />
                                </InputAdornment>
                            ),
                            endAdornment: (
                                <InputAdornment position="end">
                                    <IconButton onClick={handleClearInput}>
                                        <ClearIcon color="action" />
                                    </IconButton>
                                    <Hidden xlDown implementation="css">
                                        <IconButton onClick={handleSearch}>
                                            <SearchIcon />
                                        </IconButton>
                                    </Hidden>
                                    <Hidden lgUp implementation="css">
                                        {suggestions.length > 1 && (
                                            <Button
                                                onClick={handleSearch}
                                                style={{
                                                    backgroundColor: '#1B97F0',
                                                    color: 'white',
                                                    borderRadius: '4px',
                                                    height: '100%',
                                                    borderTopLeftRadius: '0',
                                                    borderBottomLeftRadius: '0',
                                                    margin: '0',
                                                    padding: '0',
                                                }}
                                            >
                                                Search
                                            </Button>
                                        )}
                                    </Hidden>
                                </InputAdornment>
                            ),
                            style: {
                                height: '50px',
                                fontSize: '1.2em',
                                paddingRight: '0',
                            }
                        }}
                    />
                    {
                        inputValue && (
                            <Box mt={2} boxShadow={3} style={{ maxHeight: '200px', overflowY: 'auto', backgroundColor: '#fff' }}>
                                {suggestions.map((suggestion, index) => (
                                    <Box key={index} p={1} borderBottom="1px solid #ddd">
                                        {suggestion}
                                    </Box>
                                ))}
                            </Box>
                        )
                    }
                </Box>
            </Container>
        </Box>
    </>
}