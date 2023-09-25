import React from "react";
import {
    InputAdornment,
    TextField,
    IconButton,
    Button,
    Hidden,
    CircularProgress
} from "@mui/material";
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import useOnclickOutside from "react-cool-onclickoutside";


// own imports
import { HashValues } from "../../../i18n/HashValues";
import { useAuth0 } from "@auth0/auth0-react";
import { VehicleClient } from "../../../app/web-api-client";
import { getFormatedLicense } from "../../../app/LicensePlateUtils";


interface IProps {
}

export default ({ }: IProps) => {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const navigate = useNavigate();
    const { t } = useTranslation();
    const { getAccessTokenSilently } = useAuth0();
    const [value, setValue] = React.useState<string>("");
    const [isSearching, setIsSearching] = React.useState<boolean>(false);
    const ref = useOnclickOutside(() => handleClearInput());

    const handleInput = (e: any) => {
        let license = e.target.value.toUpperCase().replace(/-/g, '');
        license = getFormatedLicense(license);

        setValue(license);
    }

    const handleClearInput = () => {
        setValue("");
    };

    const handleSearch = async () => {
        setIsSearching(true);

        vehicleClient.searchVehicle(value)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);

                    navigate(`/select-vehicle/${value}`);
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                // TODO: trigger snackbar
                console.error("Error occurred:", error);
            })
            .finally(() => {
                setIsSearching(false);
            });
    }

    const handleEnterPress = async (event:any) => {
        if (event.key === 'Enter') {
            handleSearch();
        }
    };

    return <>
        <TextField
            fullWidth
            autoComplete="new-password" // Use this line instead of autoComplete="off", because it is not working
            autoFocus={true}
            value={value}
            onChange={handleInput}
            onKeyDown={handleEnterPress}
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
                                {isSearching ? <CircularProgress size={24} /> : <SearchIcon />}
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
                                {isSearching ? <CircularProgress size={24} color="inherit" /> : t("search_camelcase")}
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



