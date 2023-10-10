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
}

export default ({ }: IProps) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const location = useLocation();

    // initial license plate value
    const [licence_plate, setLicencePlate] = React.useState<string>(getLicenseFromPath(location.pathname) || "");
    const [isVisable, setIsVisable] = React.useState<boolean>(licence_plate ? true : false);
    const [value, setValue] = React.useState<string>(licence_plate || "");
    const [hasError, setHasError] = React.useState(false);
    const [focused, setFocused] = React.useState(false);


    // Update the license plate when pathname changes
    React.useEffect(() => {
        var license = getLicenseFromPath(location.pathname);
        setIsVisable(license ? true : false);

        if (license) {
            setLicencePlate(license);
            setValue(license);
        }
    }, [location.pathname]);

    const handleInput = (e: any) => {
        let license = e.target.value.toUpperCase().replace(/-/g, '');
        license = getFormatedLicense(license);
        setValue(license);

        var isValid = getLicenseFromPath(license) ? true : false;
        if (isValid) setHasError(false);
    }

    const handleSearch = async () => {
        var isValid = getLicenseFromPath(value) ? true : false;
        setHasError(!isValid);

        if (!isValid || value.length === 0 || !licence_plate) {
            return;
        }


        // Combine pathname, search query, and hash fragment
        let fullURI = location.pathname + location.search + location.hash;
        const uri = fullURI.replace(licence_plate, value);
        navigate(uri);
    }


    const handleEnterPress = async (event:any) => {
        if (event.key === 'Enter') {
            handleSearch();

            // Remove focus from the TextField
            event.target.blur();
        }
    };

    if (!licence_plate || !isVisable) {
        return<></>;
    }

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
                    color: focused ? "black" : "lightgray",
                    height: '40px',
                    paddingRight: '0'
                }
            }}
        />
    </>
}



