import React from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List } from "@mui/material";
import { Paper, Typography, Grid, ButtonBase } from '@mui/material';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import usePlacesAutocomplete, {
    getGeocode,
    getLatLng,
    HookArgs,
    Suggestion,
} from "use-places-autocomplete";
import useOnclickOutside from "react-cool-onclickoutside";


// own imports
import { HashValues } from "../../../i18n/HashValues";
import { VehicleClient } from "../../../app/web-api-client";




interface IProps {
}

export default ({ }: IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.select_vehicle_default : window.location.hash;
    const navigate = useNavigate();
    const publicUrl = process.env.PUBLIC_URL;
    const vehicleClient = new VehicleClient(`${publicUrl}`);
    const { t } = useTranslation();

    const [value, setValue] = React.useState<string>("");
    const ref = useOnclickOutside(() => handleClearInput());

    const handleInput = (e: any) => {
        var licence = e.target.value.toUpperCase(); // Zet alles om naar hoofdletters.

        // Alleen toegestane karakters behouden (nummers en specifieke letters)
        licence = licence.replace(/[^GHJKLNPRSTXZ0-9-]/g, '');

        // Verboden lettercombinaties verwijderen
        const forbiddenCombinations = ["GVD", "KKK", "KVT", "LPF", "NSB", "PKK", "PSV", "TBS", "SS", "SD", "PVV", "SGP", "VVD"];
        forbiddenCombinations.forEach(combo => {
            if (licence.includes(combo)) {
                licence = licence.replace(combo, '');
            }
        });

        const formatLicence = (str: string, format: string) => {
            let result = "";
            let strIndex = 0;
            for (let i = 0; i < format.length; i++) {
                if ((format[i] === 'X' && /[GHJKLNPRSTXZ]/.test(str[strIndex])) || (format[i] === '9' && /[0-9]/.test(str[strIndex]))) {
                    result += str[strIndex++];
                } else {
                    result += format[i];
                }
            }
            return result;
        }

        if (licence.length === 6 || licence.length === 8) {
            licence = licence.replace(/-/g, '');

            const formats = ['XXX-X-XX', 'XX-XXX-X', 'X-XX-XXX', 'XXX-XX-X', 'X-XXX-XX', 'XX-X-XXX', 'XX-99-99', '99-99-XX', '99-XX-99', 'XX-99-XX', 'XX-XX-99', '99-XX-XX', '99-XXX-9', '9-XXX-99', 'XX-999-X', 'X-999-XX', 'XXX-99-X', 'X-99-XXX', '9-XX-999', '999-XX-9'];

            for (const format of formats) {
                if (format.replace(/-/g, '').length === licence.length) {
                    licence = formatLicence(licence, format);
                    break;
                }
            }
        }

        // Verwijder L en T voor kentekenseries na 11
        const series11AndAfter = ['XXX-99-X', 'X-99-XXX', '9-XX-999', '999-XX-9'];
        if (series11AndAfter.some(format => licence.includes(format))) {
            licence = licence.replace(/[LT]/g, '');
        }

        setValue(licence);
    }

    const handleClearInput = () => {
        setValue("");
    };

    const handleSearch = () => {
        vehicleClient.searchVehicle(value)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);

                    navigate(`/select-vehicle/${response.licencePlate}`);
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                // TODO: trigger snackbar
                console.error("Error occurred:", error);
            });
    }

    //TODO: search or trigger: handleSearch when press enter button.
    return <>
        <TextField
            fullWidth
            autoComplete="new-password" // Use this line instead of autoComplete="off", because it is not working
            autoFocus={true}
            value={value}
            onChange={handleInput}
            variant="outlined"
            placeholder={t("search_licenceplate_placeholder")}
            InputProps={{
                startAdornment: (
                    <InputAdornment position="start">
                        <LocationOnOutlinedIcon color="action" />
                    </InputAdornment>
                ),
                endAdornment: (
                    <InputAdornment position="end">
                        {value.length > 0 &&
                            <IconButton onClick={handleClearInput}>
                                <ClearIcon color="action" />
                            </IconButton>
                        }
                        <Hidden mdUp>
                            <IconButton onClick={handleSearch} style={{ marginRight: "10px" }}>
                                <SearchIcon />
                            </IconButton>
                        </Hidden>
                        <Hidden mdDown>
                            <Button
                                onClick={handleSearch}
                                sx={{
                                    backgroundColor: '#1B97F0',
                                    color: 'white',
                                    borderRadius: '4px',
                                    height: '50px',
                                    borderTopLeftRadius: '0',
                                    borderBottomLeftRadius: '0',
                                    margin: '0',
                                    padding: '0',
                                    '&:hover': {
                                        backgroundColor: '#1888d9'
                                    }
                                }}
                            >
                                {t("search_camelcase")}
                            </Button>
                        </Hidden>
                    </InputAdornment>
                ),
                style: {
                    marginTop: '60px',
                    height: '50px',
                    fontSize: '1.2em',
                    paddingRight: '0',
                    backgroundColor: 'white'
                }
            }}
        />
    </>
}



