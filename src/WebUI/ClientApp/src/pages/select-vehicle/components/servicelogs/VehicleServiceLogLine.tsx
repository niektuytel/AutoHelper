import React, { useEffect, useState } from "react";
import {
    Typography,
    Chip,
    Box,
    Skeleton,
    Divider,
    Paper,
    Button
} from '@mui/material';
import GarageIcon from '@mui/icons-material/CarRepair';
import SpeedIcon from '@mui/icons-material/Speed';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router";

// own imports
import { GarageServiceType, VehicleServiceLogDtoItem } from "../../../../app/web-api-client";
import { ROUTES } from "../../../../constants/routes";

const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
    },
    hoverSection: {
        cursor: 'pointer',
        '&:hover svg': { // This targets the SVG icons (arrow up/down) inside the hoverable section
            visibility: 'visible',
        }
    },
    arrowIcon: {
        visibility: 'hidden', // Initially hide the arrow icon
    }
}

interface IProps {
    isMobile: boolean;
    keyIndex: number;
    license_plate: string;
    logItem: VehicleServiceLogDtoItem,
}

export default ({ isMobile, keyIndex, license_plate, logItem }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);


    const getServiceTypeLabel = (type: GarageServiceType): string => {
        return t(`serviceTypes:${GarageServiceType[type]}.Title`);
    };

    // Click handler to toggle the open state
    const handleToggleOpen = () => {
        setOpen(!open);
    };

    return <>
        <Box key={`serviceLog-${keyIndex}`} sx={textStyles.hoverSection} onClick={handleToggleOpen}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <Typography variant="subtitle1" color="text.secondary">
                        {logItem.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                    </Typography>
                    <Typography sx={{ mx: 1 }}> - </Typography>
                    <Typography variant="h6" sx={textStyles.root}>
                        <b>{getServiceTypeLabel(logItem.type!)}</b>
                    </Typography>
                    {open ?
                        <KeyboardArrowUpIcon sx={!isMobile ? textStyles.arrowIcon : null} />
                        :
                        <KeyboardArrowDownIcon sx={!isMobile ? textStyles.arrowIcon : null} />
                    }
                </Box>
                {logItem.status == ?
                    <Chip
                        label="Verified"
                        color="success"
                        variant="outlined"
                        sx={{ ml: 'auto' }}
                    />
                    :
                    <Chip
                        label="Unverified"
                        color="warning"
                        variant="outlined"
                        sx={{ ml: 'auto' }}
                    />
                }
            </Box>
            {open && <>
                <Typography variant="body2" sx={{ mx: 1 }}>
                    {logItem.description}
                </Typography>
            </>}
            <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1 }}>
                <Chip
                    icon={<SpeedIcon />}
                    label={`${logItem.odometerReading!.toLocaleString()} km`}
                    variant="outlined"
                    sx={{ mr: 1, my: 0.5 }}
                />
                <Chip
                    icon={<GarageIcon />}
                    label={logItem.garageLookupName}
                    variant="outlined"
                    onClick={() => navigate(`${ROUTES.GARAGE}/${logItem.garageLookupIdentifier}?licensePlate=${license_plate}`)}
                    sx={{ mr: 1, my: 0.5 }}
                />
                {logItem.attachedFile && (
                    <Chip
                        icon={<AttachFileIcon />}
                        label={"Bijlage"}
                        variant="outlined"
                        onClick={() => window.open(logItem.attachedFile, '_blank')}
                        sx={{ mr: 1, my: 0.5 }}
                    />
                )}
            </Box>
            <Divider sx={{ mt: 1 }} />
        </Box>
    </>
}