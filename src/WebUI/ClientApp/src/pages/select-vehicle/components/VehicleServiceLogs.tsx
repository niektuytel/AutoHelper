import React, { useEffect } from "react";
import {
    Card,
    CardContent,
    Typography,
    Chip,
    IconButton,
    CardActions,
    Box,
    Skeleton,
    Grid,
    Divider,
    Paper,
    Tooltip, 
    Button
} from '@mui/material';
import CarRepairIcon from '@mui/icons-material/Build';
import GarageIcon from '@mui/icons-material/CarRepair';
import SpeedIcon from '@mui/icons-material/Speed';
import FileIcon from '@mui/icons-material/Description';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { CSSProperties } from "react";
import useVehicleServiceLogs from "../useVehicleServiceLogs";
import { GarageServiceType, VehicleServiceLogItemDto } from "../../../app/web-api-client";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import CloseIcon from '@mui/icons-material/Close';
import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';

// own imports
import VehicleServiceLogs from '../components/VehicleServiceLogs';
import VehicleSpecifications from '../components/VehicleSpecifications';
import ServiceLogForm from '../components/ServiceLogForm';

const textStyles = {
    root: {
        color: "black",
        fontFamily: "'Nunito', sans-serif",
    }
}

interface IProps {
    isMobile: boolean;
    license_plate: string;
    setDrawerOpen: (open: boolean) => void;
}

export default ({ isMobile, license_plate, setDrawerOpen }: IProps) => {
    const { loading, vehicleServiceLogs } = useVehicleServiceLogs(license_plate);
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();


    const handleAddService = () => {
        setDrawerOpen(true);
    };


    // Function to return a string representation of the GarageServiceType
    const getServiceTypeLabel = (type: GarageServiceType): string => {
        return t(`serviceTypes:${GarageServiceType[type]}.Title`);
    };

    return <>
        <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ mb:1 }}>
            <Button
                variant="outlined"
                onClick={handleAddService}
                startIcon={<AddCircleOutlineIcon />}
                fullWidth
            >
                {t("AddMaintenanceLog.Title")}
            </Button>
        </Box>
        <Paper variant="outlined" sx={{ borderRadius: 1, overflow: "hidden" }}>
            {loading ?
                Array.from({ length: 10 }).map((_, index) =>
                    <Skeleton sx={{ width: "100%" }} />
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
                                sx={{ ml: 'auto' }} // This will push the chip to the right
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