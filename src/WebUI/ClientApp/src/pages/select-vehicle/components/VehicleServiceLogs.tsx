import React, { useEffect } from "react";
import { Box, Card, CircularProgress, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import LicensePlateTextField from "./LicensePlateTextField";
import useVehicle from "../useVehicle";
import { CSSProperties } from "react";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    //TODO: const { loading, vehicleBriefInfo } = useVehicleServiceLogs(license_plate);
    return <>
        <Box>
            <Typography variant="h4">
                TODO: Does not found any service logs for this vehicle
            </Typography>
        </Box>
    </>
}