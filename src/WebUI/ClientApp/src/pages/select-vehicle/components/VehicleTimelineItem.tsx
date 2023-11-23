import * as React from 'react';
import Timeline from '@mui/lab/Timeline';
import TimelineItem from '@mui/lab/TimelineItem';
import TimelineSeparator from '@mui/lab/TimelineSeparator';
import TimelineConnector from '@mui/lab/TimelineConnector';
import TimelineContent from '@mui/lab/TimelineContent';
import TimelineOppositeContent, { timelineOppositeContentClasses } from '@mui/lab/TimelineOppositeContent';
import TimelineDot from '@mui/lab/TimelineDot';
import FastfoodIcon from '@mui/icons-material/Fastfood';
import Person4OutlinedIcon from '@mui/icons-material/Person4Outlined';
import LaptopMacIcon from '@mui/icons-material/LaptopMac';
import HotelIcon from '@mui/icons-material/Hotel';
import RepeatIcon from '@mui/icons-material/Repeat';
import PriorityHighIcon from '@mui/icons-material/PriorityHigh';
import BuildRoundedIcon from '@mui/icons-material/BuildRounded';
import PriorityHighRoundedIcon from '@mui/icons-material/PriorityHighRounded';
import DoneOutlineRoundedIcon from '@mui/icons-material/DoneOutlineRounded';
import Typography from '@mui/material/Typography';
import useVehicleTimelineCard from '../useVehicleTimelineCard';
import { VehicleTimelineDtoItem, VehicleTimelineType } from '../../../app/web-api-client';
import { Tooltip } from '@mui/material';

// CSS for the ellipsis
const ellipsisStyle:any = {
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    maxWidth: '250px', // Set a max-width as per your design requirements
};

interface IProps {
    textColor?: string,
    timelineItem: VehicleTimelineDtoItem
}

export default ({ textColor="white", timelineItem }: IProps) => {
    const getTimelineDot = (type: VehicleTimelineType) => {
        switch (type) {
            case VehicleTimelineType.Repair:
            case VehicleTimelineType.Service:
                return <TimelineDot color="success" variant="outlined" sx={{ bgcolor: 'white' }} >
                    <BuildRoundedIcon sx={{ color: "#2E7D32" }} />
                </TimelineDot>
            case VehicleTimelineType.SucceededMOT:
                return <TimelineDot color="success" variant="outlined" sx={{ bgcolor: 'white' }} >
                    <DoneOutlineRoundedIcon sx={{ color: "#2E7D32" }} />
                </TimelineDot>
            case VehicleTimelineType.FailedMOT:
                return <TimelineDot color="warning" variant="outlined" sx={{ bgcolor: 'white' }}>
                    <PriorityHighRoundedIcon sx={{ color: "#ED6C02" }} />
                </TimelineDot>
            case VehicleTimelineType.OwnerChange:
                return <TimelineDot variant="outlined" sx={{ bgcolor: 'white', color:"darkgray" }}>
                    <Person4OutlinedIcon sx={{ color: "darkgray" }} />
                </TimelineDot>
        }

        return <TimelineDot color="success" variant="filled" sx={{ bgcolor: 'white', mx:"12px" }}></TimelineDot>
    }

    return (
        <TimelineItem sx={{ width:"min-content"}}>
            <TimelineOppositeContent
                sx={{ m: 'auto 0' }}
                align="right"
                variant="body2"
                color={textColor}
            >
                {timelineItem?.date?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
            </TimelineOppositeContent>
            <TimelineSeparator>
                <TimelineConnector />
                {getTimelineDot(timelineItem.type!)}
                <TimelineConnector />
            </TimelineSeparator>
            <TimelineContent sx={{
                py: '12px',
                px: 2,
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center' // Center vertically
            }}>
                <Typography variant="h6" component="span" color={textColor} sx={{ width: "max-content" }}>
                    {timelineItem.title}
                </Typography>
                {timelineItem?.description &&  
                    <Tooltip title={timelineItem.description || ''}>
                        <Typography color={textColor} style={ellipsisStyle}>
                            {timelineItem.description}
                        </Typography>
                    </Tooltip>
                }
            </TimelineContent>
        </TimelineItem>
    );
}