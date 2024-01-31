import React, { useEffect, useState } from "react";
import {
    Typography,
    Chip,
    Box,
    Skeleton,
    Divider,
    Paper,
    Button,
    Popover,
    Tooltip
} from '@mui/material';
import GarageIcon from '@mui/icons-material/CarRepair';
import SpeedIcon from '@mui/icons-material/Speed';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import VerifiedIcon from '@mui/icons-material/Verified';
import NewReleasesIcon from '@mui/icons-material/NewReleases';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router";

// own imports
import { GarageServiceType, VehicleServiceLogDtoItem, VehicleServiceLogStatus } from "../../../../app/web-api-client";
import { ROUTES } from "../../../../constants/routes";

const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
        '@media (max-width:600px)': {
            fontSize: 'calc(100% + 1vw)',
        },
    },
    hoverSection: {
        cursor: 'pointer',
        '&:hover svg': {
            visibility: 'visible',
        },
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
    const location = useLocation();
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);

    // Click handler to toggle the open state
    const handleToggleOpen = () => {
        setOpen(!open);
    };

    return <>
        <Box key={`serviceLog-${keyIndex}`} sx={textStyles.hoverSection} onClick={handleToggleOpen}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    {open ? (
                        <KeyboardArrowUpIcon />
                    ) : (
                        <KeyboardArrowDownIcon />
                    )}
                    <Typography variant="h6" sx={textStyles.root}>
                        <b>{logItem.title ? logItem.title : t(`serviceTypes:${GarageServiceType[logItem.type!]}.Title`)}</b>
                    </Typography>
                </Box>
                <Typography variant="subtitle1" color="text.secondary" sx={{ mr: 0.5, alignSelf:"start" }}>
                    {logItem.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                </Typography>
            </Box>
            {open && <>
                <Typography variant="body2" sx={{ ml: 1.5 }}>
                    {logItem.description ? logItem.description : t(`serviceTypes:${GarageServiceType[logItem.type!]}.Description`)}
                </Typography>
            </>}
            <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center' }}>
                <Chip
                    icon={<SpeedIcon />}
                    label={`${logItem.odometerReading!.toLocaleString()} km`}
                    variant="outlined"
                    sx={{ ml: 1, mt: 1 }}
                />
                <Chip
                    icon={<GarageIcon />}
                    label={logItem.garageLookupName}
                    variant="outlined"
                    onClick={() => navigate(`${ROUTES.GARAGE}/${logItem.garageLookupIdentifier}?licensePlate=${license_plate}`, { state: { from: location } })}
                    sx={{ ml: 1, mt: 1 }}
                />
                {logItem.attachedFile && (
                    <Chip
                        icon={<AttachFileIcon />}
                        label={"Bijlage"}
                        variant="outlined"
                        onClick={() => window.open(logItem.attachedFile, '_blank')}
                        sx={{ ml: 1, mt: 1 }}
                    />
                )}
            </Box>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1, mt: 0 }}>
                {logItem.status === VehicleServiceLogStatus.VerifiedByGarage ?
                    <Tooltip title={t("ServiceLog.Verified.Title")}>
                        <Chip
                            color="success"
                            icon={<VerifiedIcon />}
                            label={t("ServiceLog.Verified.Label")}
                            variant="outlined"
                            sx={{ mr: 1, mt: 1 }}
                        />
                    </Tooltip>
                    :
                    <Tooltip title={t("ServiceLog.UnVerified.Title")}>
                        <Chip
                            color="warning"
                            icon={<NewReleasesIcon />}
                            label={t("ServiceLog.UnVerified.Label")}
                            variant="outlined"
                            sx={{ mr: 1, mt: 1 }}
                        />
                    </Tooltip>
                }
            </Box>
            <Divider sx={{ mt: 1 }} />
        </Box>
    </>
}