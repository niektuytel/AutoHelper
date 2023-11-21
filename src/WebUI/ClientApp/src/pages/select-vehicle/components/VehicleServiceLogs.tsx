import React, { useEffect } from "react";
import {
    Card,
    CardContent,
    Typography,
    Chip,
    IconButton,
    CardActions,
    Box,
    Skeleton
} from '@mui/material';
import CarRepairIcon from '@mui/icons-material/Build';
import FileIcon from '@mui/icons-material/Description';
import { CSSProperties } from "react";
import useVehicleServiceLogs from "../useVehicleServiceLogs";
import { GarageServiceType, VehicleServiceLogItemDto } from "../../../app/web-api-client";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleServiceLogs } = useVehicleServiceLogs(license_plate);

    // Function to return a string representation of the GarageServiceType
    const getServiceTypeLabel = (type: GarageServiceType): string => {
        // Replace with actual logic to convert GarageServiceType to string
        return 'Service Type';
    };

    return <>
        {loading ?
            Array.from({ length: 10 }).map((_, index) => 
                <Skeleton sx={{ width: "100%" }} />
            )
            :
            vehicleServiceLogs?.map((logItem, index) => 

                <Card sx={{ minWidth: 275, marginBottom: 2 }}>
                    <CardContent>
                        <Typography variant="h5" component="div">
                            {logItem.garageLookupName}
                        </Typography>
                        <Typography sx={{ mb: 1.5 }} color="text.secondary">
                            {logItem.date!.toLocaleDateString()}
                        </Typography>
                        <Chip
                            icon={<CarRepairIcon />}
                            label={getServiceTypeLabel(logItem.type!)}
                            variant="outlined"
                        />
                        <Typography variant="body2">
                            Odometer Reading: {logItem.odometerReading!.toLocaleString()} km
                        </Typography>
                        {logItem.description && (
                            <Typography variant="body2" sx={{ mt: 1 }}>
                                {logItem.description}
                            </Typography>
                        )}
                    </CardContent>
                    {logItem.attachedFile && (
                        <CardActions disableSpacing>
                            <IconButton aria-label="attached file" onClick={() => window.open(logItem.attachedFile, '_blank')}>
                                <FileIcon />
                            </IconButton>
                        </CardActions>
                    )}
                    <Box sx={{ p: 2, pt: 0 }}>
                        <Typography variant="caption">ID: {logItem.garageLookupIdentifier}</Typography>
                    </Box>
                </Card>
            )
        }
    </>
}