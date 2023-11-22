import React, { useEffect, useState } from "react";
import { Box, Card, CircularProgress, Divider, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import { CSSProperties } from "react";
import { styled } from '@mui/material/styles';
import ArrowForwardIosSharpIcon from '@mui/icons-material/ArrowForwardIosSharp';
import MuiAccordion, { AccordionProps } from '@mui/material/Accordion';
import MuiAccordionSummary, {
    AccordionSummaryProps,
} from '@mui/material/AccordionSummary';
import MuiAccordionDetails from '@mui/material/AccordionDetails';
import useVehicleInformation from "../useVehicleInformation";
import useVehicleTimeline from "../useVehicleTimeline";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleTimeline } = useVehicleTimeline(license_plate);

    return <>
        <Paper variant="outlined" sx={{ borderRadius: 1, overflow: "hidden" }}>
            {loading ?
                Array.from({ length: 10 }).map((_, index) =>
                    <Skeleton sx={{ width: "100%" }} />
                )
                :
                vehicleTimeline?.map((timelineItem, index) => (
                    <Box key={`serviceLog-${index}`}>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <Typography variant="subtitle1" color="text.secondary">
                                    {timelineItem.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                                </Typography>
                                <Typography sx={{ mx: 1 }}> - </Typography>
                                <Typography variant="h6">
                                </Typography>
                            </Box>
                            {/*<Chip*/}
                            {/*    label="Unverified"*/}
                            {/*    color="warning"*/}
                            {/*    variant="outlined"*/}
                            {/*    sx={{ ml: 'auto' }} // This will push the chip to the right*/}
                            {/*/>*/}
                        </Box>
                        {timelineItem.description && (
                            <Typography variant="body2" sx={{ mx: 1 }}>
                                {timelineItem.description}
                            </Typography>
                        )}
                        {/*<Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1 }}>*/}
                        {/*    <Chip*/}
                        {/*        icon={<SpeedIcon />}*/}
                        {/*        label={`${timelineItem.odometerReading!.toLocaleString()} km`}*/}
                        {/*        variant="outlined"*/}
                        {/*        sx={{ mr: 1, my: 0.5 }}*/}
                        {/*    />*/}
                        {/*    <Chip*/}
                        {/*        icon={<GarageIcon />}*/}
                        {/*        label={timelineItem.garageLookupName}*/}
                        {/*        variant="outlined"*/}
                        {/*        onClick={() => navigate(`${ROUTES.GARAGE}/${timelineItem.garageLookupIdentifier}?licensePlate=${license_plate}`)}*/}
                        {/*        sx={{ mr: 1, my: 0.5 }}*/}
                        {/*    />*/}
                        {/*    {timelineItem.attachedFile && (*/}
                        {/*        <Chip*/}
                        {/*            icon={<AttachFileIcon />}*/}
                        {/*            label={"Bijlage"}*/}
                        {/*            variant="outlined"*/}
                        {/*            onClick={() => window.open(timelineItem.attachedFile, '_blank')}*/}
                        {/*            sx={{ mr: 1, my: 0.5 }}*/}
                        {/*        />*/}
                        {/*    )}*/}
                        {/*</Box>*/}
                        <Divider sx={{ mt: 1 }} />
                    </Box>
                ))
            }
        </Paper>
    </>
}
