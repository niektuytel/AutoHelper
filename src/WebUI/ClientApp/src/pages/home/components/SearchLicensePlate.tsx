import React from "react";
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
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import DriveEtaIcon from '@mui/icons-material/DriveEta';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";
import useOnclickOutside from "react-cool-onclickoutside";
import { useDispatch } from "react-redux";

// own imports
import { getFormatedLicense } from "../../../utils/LicensePlateUtils";
import useSearchVehicle from "../../useSearchVehicle";
import { ROUTES } from "../../../constants/routes";
import { showOnError } from "../../../redux/slices/statusSnackbarSlice";


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useDispatch();
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

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        handleSearch();
    };

    const handleSearch = async () => {
        var response = await fetchVehicleByPlate(value);
        if (response) {
            console.log("Response received:", response);

            navigate(`${ROUTES.VEHICLE}/${value}`, { state: { from: location } });
        } else {
            console.error("Failed to get vehicle by license plate");
        }
    }

    return <>
        <form onSubmit={handleSubmit}>
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
                            <DriveEtaIcon color="action" />
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
                        marginTop: isMobile ? '15px' : '90px',
                        height: '50px',
                        fontSize: '1.2em',
                        paddingRight: '0',
                        backgroundColor: 'white'
                    }
                }}
            />
            {/*Used as iphone is using Done button to submit the form*/}
            <button type="submit" style={{ display: 'none' }}>Submit</button>
        </form >
    </>
}



