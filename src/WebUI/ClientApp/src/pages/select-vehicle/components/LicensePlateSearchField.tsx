﻿import React from "react";
import {
    InputAdornment,
    TextField,
    IconButton,
    Button,
    Hidden,
    CircularProgress,
    useTheme,
    useMediaQuery
} from "@mui/material";
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import useOnclickOutside from "react-cool-onclickoutside";


// own imports
import { useAuth0 } from "@auth0/auth0-react";
import { VehicleClient } from "../../../app/web-api-client";
import { getFormatedLicense } from "../../../app/LicensePlateUtils";
import useSearchVehicle from "../useSearchVehicle";
import { ROUTES } from "../../../constants/routes";


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const navigate = useNavigate();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [value, setValue] = React.useState<string>("");
    const ref = useOnclickOutside(() => handleClearInput());
    const { loading, fetchVehicleByPlate } = useSearchVehicle();

    const handleInput = (e: any) => {
        let license = e.target.value.toUpperCase().replace(/-/g, '');
        license = getFormatedLicense(license);

        setValue(license);
    }

    const handleClearInput = () => {
        setValue("");
    };

    const handleSearch = async () => {
        var response = await fetchVehicleByPlate(value);
        if (response) {
            console.log("Response received:", response);

            navigate(`${ROUTES.SELECT_VEHICLE}/${value}`);
        } else {
            // TODO: trigger snackbar
            console.error("Failed to get vehicle by license plate");
        }
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
                                {loading ? <CircularProgress size={24} /> : <SearchIcon />}
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
                                {loading ? <CircularProgress size={24} color="inherit" /> : t("search_camelcase")}
                            </Button>
                        </Hidden>
                    </InputAdornment>
                ),
                style: {
                    marginTop: isMobile ? '15px' : '60px',
                    height: '50px',
                    fontSize: '1.2em',
                    paddingRight: '0',
                    backgroundColor: 'white'
                }
            }}
        />
    </>
}



