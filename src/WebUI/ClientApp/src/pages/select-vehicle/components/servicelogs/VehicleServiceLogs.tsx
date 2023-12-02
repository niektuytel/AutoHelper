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
import useVehicleServiceLogs from "../../useVehicleServiceLogs";
import { GarageServiceType, VehicleClient, VehicleServiceLogItem } from "../../../../app/web-api-client";
import VehicleServiceLogItemSkeleton from "./VehicleServiceLogLineSkeleton";
import VehicleServiceLog from "./VehicleServiceLogLine";

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

    // TODO: open will collabse more service log information
    const open = false;

    return <>
        <Paper variant="outlined" sx={{ borderRadius: 1, overflow: "hidden" }}>
            {loading ?
                Array.from({ length: 10 }).map((_, index) =>
                    <VehicleServiceLogItemSkeleton key={`skeleton-${index}`} keyIndex={index} />
                )
                :
                vehicleServiceLogs?.map((logItem, index) => 
                    <VehicleServiceLog key={`serviceLog-${index}`} keyIndex={index} isMobile={isMobile} license_plate={license_plate} logItem={logItem} />
                )
            }
        </Paper>
    </>
}