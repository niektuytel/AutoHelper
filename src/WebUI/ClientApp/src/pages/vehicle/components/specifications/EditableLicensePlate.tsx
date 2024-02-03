import React from "react";
import {
    InputAdornment,
    TextField,
    IconButton
} from "@mui/material";
import SearchIcon from '@mui/icons-material/Search';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";

// own imports
import { getFormatedLicense, getLicenseFromPath } from "../../../../utils/LicensePlateUtils";


interface IProps {
    license_plate: string;
}

export default ({ license_plate }: IProps) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const location = useLocation();
    const [value, setValue] = React.useState<string>(license_plate || "");
    const [focused, setFocused] = React.useState(false);
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
        navigate(uri, { state: { from: location } });
        return true;
    }


    const handleEnterPress = async (event:any) => {
        if (event.key === 'Enter' && await handleSearch()) {
            event.target.blur();
        }
    };

    return <>
        <TextField
            fullWidth
            autoComplete="new-password"
            value={value}
            onChange={handleInput}
            onKeyDown={handleEnterPress}
            onFocus={() => setFocused(true)}
            onBlur={() => setFocused(false)}
            variant="outlined"
            placeholder={t("e.g. 87-GRN-6")}
            error={hasError}
            InputProps={{
                endAdornment: (
                    <InputAdornment position="end">
                        <IconButton onClick={handleSearch}>
                            <SearchIcon
                                sx={{ color: "lightgray" }}
                            />
                        </IconButton>
                    </InputAdornment>
                ),
                style: {
                    color: "black",
                    height: '40px',
                    paddingRight: '0'
                }
            }}
        />
    </>
}



