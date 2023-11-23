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
import VehicleTimelineItemSkeleton from "./VehicleTimelineItemSkeleton";
import VehicleTimelineItem from "./VehicleTimelineItem";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleTimeline } = useVehicleTimeline(license_plate);

    return <>
        {loading ?
            Array.from({ length: 15 }).map((_, index) => (
                <VehicleTimelineItemSkeleton key={index} usedIndex={index} />
            ))
            :
            vehicleTimeline?.map((timelineItem, index) => (
                <VehicleTimelineItem key={index} timelineItem={timelineItem} textColor={'black'} />
            ))
        }
    </>
}
