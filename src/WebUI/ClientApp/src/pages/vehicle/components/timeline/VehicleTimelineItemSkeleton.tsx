import * as React from 'react';
import Timeline from '@mui/lab/Timeline';
import TimelineItem from '@mui/lab/TimelineItem';
import TimelineSeparator from '@mui/lab/TimelineSeparator';
import TimelineConnector from '@mui/lab/TimelineConnector';
import TimelineContent from '@mui/lab/TimelineContent';
import TimelineOppositeContent from '@mui/lab/TimelineOppositeContent';
import TimelineDot from '@mui/lab/TimelineDot';
import BuildRoundedIcon from '@mui/icons-material/BuildRounded';
import PriorityHighRoundedIcon from '@mui/icons-material/PriorityHighRounded';
import DoneOutlineRoundedIcon from '@mui/icons-material/DoneOutlineRounded';
import { Paper, Skeleton } from '@mui/material';
import { VehicleTimelineType } from '../../../../app/web-api-client';

interface IProps {
    usedIndex: number
}

export default ({ usedIndex }:IProps) => {
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
        <TimelineItem sx={{ width: "min-content" }} key={`TimelineItem-${usedIndex}`}>
            <TimelineOppositeContent
                sx={{ m: 'auto 0' }}
                align="right"
                variant="body2"
                color="white"
            >
                <Skeleton sx={{ width: "75px" }} />
            </TimelineOppositeContent>
            <TimelineSeparator>
                <TimelineConnector />
                {usedIndex % 3 === 0 ?
                    getTimelineDot(VehicleTimelineType.Repair)
                : usedIndex % 3 === 1 ?
                    getTimelineDot(VehicleTimelineType.Service)
                :
                    getTimelineDot(VehicleTimelineType.FailedMOT)
                }
                <TimelineConnector />
            </TimelineSeparator>
            <TimelineContent sx={{ py: '12px', px: 2 }}>
            </TimelineContent>
        </TimelineItem>
    );
}