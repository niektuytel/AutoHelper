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
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router";

// own imports
import useVehicleServiceLogs from "../useVehicleServiceLogs";
import { GarageServiceType } from "../../../app/web-api-client";
import { ROUTES } from "../../../constants/routes";

const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
    }
}

interface IProps {
    isMobile: boolean;
    license_plate: string;
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleServiceLogs, isError } = useVehicleServiceLogs(license_plate);
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();

    const getServiceTypeLabel = (type: GarageServiceType): string => {
        return t(`serviceTypes:${GarageServiceType[type]}.Title`);
    };

    return <>
        <Paper variant="outlined" sx={{ borderRadius: 1, overflow: "hidden" }}>
            {loading ?
                Array.from({ length: 10 }).map((_, index) =>
                    <Box key={`serviceLog-${index}`}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <Typography variant="subtitle1" color="text.secondary">
                                    <Skeleton sx={{ width: "100px" }} />
                                </Typography>
                            </Box>
                            <Chip
                                label="Unverified"
                                color="warning"
                                variant="outlined"
                                sx={{ ml: 'auto' }}
                            />
                        </Box>
                        <Typography variant="body2" sx={{ mx: 1 }}>
                            <Skeleton sx={{ width: "100%" }} />
                        </Typography>
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1 }}>
                            <Skeleton sx={{ width: "100%" }} />
                        </Box>
                        <Divider sx={{ mt: 1 }} />
                    </Box>
                )
                :
                vehicleServiceLogs?.map((logItem, index) => (
                    <Box key={`serviceLog-${index}`}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <Typography variant="subtitle1" color="text.secondary">
                                    {logItem.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                                </Typography>
                                <Typography sx={{ mx: 1 }}> - </Typography>
                                <Typography variant="h6" sx={textStyles.root}>
                                    <b>{getServiceTypeLabel(logItem.type!)}</b>
                                </Typography>
                            </Box>
                            <Chip
                                label="Unverified"
                                color="warning"
                                variant="outlined"
                                sx={{ ml: 'auto' }}
                            />
                        </Box>
                        {logItem.description && (
                            <Typography variant="body2" sx={{ mx: 1 }}>
                                {logItem.description}
                            </Typography>
                        )}
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
                ))
            }
        </Paper>
    </>
}