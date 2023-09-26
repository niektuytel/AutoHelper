import React from "react";
import {
    InputAdornment,
    TextField,
    IconButton
} from "@mui/material";
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";


// own imports
import { getFormatedLicense, getLicenseFromPath } from "../../../app/LicensePlateUtils";
import { alignProperty } from "@mui/material/styles/cssUtils";


interface IProps {
    license_plate: string;
}

export default ({ license_plate }: IProps) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const location = useLocation();

    // initial license plate value
    const [value, setValue] = React.useState<string>(license_plate || "");
    const [hasError, setHasError] = React.useState(false);

    const handleInput = (e: any) => {
        let license = e.target.value.toUpperCase().replace(/-/g, '');
        license = getFormatedLicense(license);
        setValue(license);

        var isValid = getLicenseFromPath(license) ? true : false;
        if (isValid) setHasError(false);
    }

    async function handleSearch(): Promise<boolean> {
        var isValid = getLicenseFromPath(value) ? true : false;
        setHasError(!isValid);

        if (!isValid || value.length === 0 || !license_plate) {
            return false;
        }


        // Combine pathname, search query, and hash fragment
        let fullURI = location.pathname + location.search + location.hash;
        const uri = fullURI.replace(license_plate, value);
        navigate(uri);
        return true;
    }


    const handleEnterPress = async (event:any) => {
        if (event.key === 'Enter' && await handleSearch()) {
            // Remove focus from the TextField
            event.target.blur();
        }
    };

    //label={value.length > 0 ? t("license") : undefined}

    return <>
        <TextField
            fullWidth
            autoComplete="new-password"
            value={value}
            onChange={handleInput}
            onKeyDown={handleEnterPress}
            variant="outlined"
            placeholder={t("e.g. 87-GRN-6")}
            error={hasError}
            InputProps={{
                endAdornment: (
                    <InputAdornment position="end">
                        {value.length > 0 &&
                            <IconButton onClick={handleSearch}>
                                <SearchIcon sx={{ color:"lightgray"}} />
                            </IconButton>
                        }
                    </InputAdornment>
                ),
                style: {
                    color: "black",
                    height: '40px',
                    //fontSize: '1.2em',
                    paddingRight: '0',
                    //backgroundColor: '#fff',
                    
                }
            }}
        />
    </>
}



