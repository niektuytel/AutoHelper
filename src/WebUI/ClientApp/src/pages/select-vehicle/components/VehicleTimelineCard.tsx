import * as React from 'react';
import Timeline from '@mui/lab/Timeline';
import TimelineItem from '@mui/lab/TimelineItem';
import TimelineSeparator from '@mui/lab/TimelineSeparator';
import TimelineConnector from '@mui/lab/TimelineConnector';
import TimelineContent from '@mui/lab/TimelineContent';
import TimelineOppositeContent, { timelineOppositeContentClasses } from '@mui/lab/TimelineOppositeContent';
import TimelineDot from '@mui/lab/TimelineDot';
import FastfoodIcon from '@mui/icons-material/Fastfood';
import LaptopMacIcon from '@mui/icons-material/LaptopMac';
import HotelIcon from '@mui/icons-material/Hotel';
import RepeatIcon from '@mui/icons-material/Repeat';
import PriorityHighIcon from '@mui/icons-material/PriorityHigh';
import BuildRoundedIcon from '@mui/icons-material/BuildRounded';
import PriorityHighRoundedIcon from '@mui/icons-material/PriorityHighRounded';
import DoneOutlineRoundedIcon from '@mui/icons-material/DoneOutlineRounded';
import Typography from '@mui/material/Typography';
import CarCrashIcon from '@mui/icons-material/CarCrash';
import { Paper, Skeleton } from '@mui/material';
import { FormatColorText } from '@mui/icons-material';
import useVehicleTimelineCard from '../useVehicleTimelineCard';
import { VehicleTimelineDtoItem, VehicleTimelineType } from '../../../app/web-api-client';
import VehicleTimelineItemSkeleton from './VehicleTimelineItemSkeleton';
import VehicleTimelineItem from './VehicleTimelineItem';
import VehicleTimeline from './VehicleTimeline';

interface IProps {
    license_plate: string
}

export default ({ license_plate }:IProps) => {
    const { loading, vehicleTimelineCard } = useVehicleTimelineCard(license_plate);

    const getTimelineDot = (type: VehicleTimelineType) => {
        switch (type) {
            case VehicleTimelineType.Repair:
                return <TimelineDot color="success" variant="outlined" sx={{ bgcolor: 'white' }} >
                    <BuildRoundedIcon sx={{ color: "#2E7D32" }} />
                </TimelineDot>
            case VehicleTimelineType.Service:
            case VehicleTimelineType.SucceededMOT:
                return <TimelineDot color="success" variant="outlined" sx={{ bgcolor: 'white' }} >
                    <DoneOutlineRoundedIcon sx={{ color: "#2E7D32" }} />
                </TimelineDot>
            case VehicleTimelineType.FailedMOT:
                return <TimelineDot color="warning" variant="outlined" sx={{ bgcolor: 'white' }}>
                    <PriorityHighRoundedIcon sx={{ color: "#ED6C02" }} />
                </TimelineDot>
        }

        return <TimelineDot color="success" variant="filled" sx={{ bgcolor: 'white' }}></TimelineDot>
    }

    return (
        <Timeline position="right"
                sx={{
                    [`& .${timelineOppositeContentClasses.root}`]: {
                        flex: 0.2,
                        minWidth: "110px"
                    },
                }}>
                {loading ?
                    Array.from({ length: 4 }).map((_, index) => (
                        <VehicleTimelineItemSkeleton key={index} usedIndex={index} />
                    ))
                    :
                    vehicleTimelineCard?.map((timelineItem, index) => (
                        <VehicleTimelineItem key={index} timelineItem={timelineItem} />
                    ))
                }
        </Timeline>
    );
}