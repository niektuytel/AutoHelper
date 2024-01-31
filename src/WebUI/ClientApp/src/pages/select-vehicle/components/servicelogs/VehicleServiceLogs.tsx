import React, { useContext, useEffect, useState } from "react";
import { Box, Button, Paper } from '@mui/material';

// own imports
import useVehicleServiceLogs from "../../useVehicleServiceLogs";
import VehicleServiceLogItemSkeleton from "./VehicleServiceLogLineSkeleton";
import VehicleServiceLog from "./VehicleServiceLogLine";
import { ServiceLogDrawerContext } from "../../../../context/ServiceLogDrawerContext";
import VehicleServiceLogsIsEmpty from "./VehicleServiceLogsIsEmpty";
import VehicleServiceLogHeader from "./VehicleServiceLogHeader";

interface IProps {
    isMobile: boolean;
    license_plate: string;
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleServiceLogs } = useVehicleServiceLogs(license_plate);

    return <>
        <Paper elevation={2} sx={{ borderRadius: 1, overflow: "hidden" }}>
            {loading ?
                Array.from({ length: 10 }).map((_, index) =>
                    <VehicleServiceLogItemSkeleton key={`skeleton-${index}`} keyIndex={index} />
                )
                :
                vehicleServiceLogs?.length === 0 ?
                    <VehicleServiceLogsIsEmpty />
                : <>
                    <VehicleServiceLogHeader isMobile={isMobile}/>
                    {vehicleServiceLogs?.map((logItem, index) =>
                        <VehicleServiceLog key={`serviceLog-${index}`} keyIndex={index} isMobile={isMobile} license_plate={license_plate} logItem={logItem} />
                    )}
                </>
            }
        </Paper>
    </>
}