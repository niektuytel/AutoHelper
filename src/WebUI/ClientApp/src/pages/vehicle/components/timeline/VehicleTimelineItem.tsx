import * as React from 'react';
import TimelineItem from '@mui/lab/TimelineItem';
import TimelineSeparator from '@mui/lab/TimelineSeparator';
import TimelineConnector from '@mui/lab/TimelineConnector';
import TimelineContent from '@mui/lab/TimelineContent';
import TimelineOppositeContent from '@mui/lab/TimelineOppositeContent';
import TimelineDot from '@mui/lab/TimelineDot';
import Person4OutlinedIcon from '@mui/icons-material/Person4Outlined';
import BuildRoundedIcon from '@mui/icons-material/BuildRounded';
import PriorityHighRoundedIcon from '@mui/icons-material/PriorityHighRounded';
import DoneOutlineRoundedIcon from '@mui/icons-material/DoneOutlineRounded';
import Typography from '@mui/material/Typography';
import { Tooltip } from '@mui/material';

// custom imports
import { VehicleTimelineDtoItem, VehicleTimelineType } from '../../../../app/web-api-client';
import TimelineDialog from './TimelineDialog';
import { useState } from 'react';

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

export default ({ textColor = "white", timelineItem }: IProps) => {
    const [showDialog, setShowDialog] = useState(false);

    const handleOpenDialog = () => {
        setShowDialog(true);
    };

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

    return <>
        <TimelineItem sx={{ width: "min-content" }} key={`TimelineItem-${timelineItem.date?.toDateString()}`}>
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
            <TimelineContent
                sx={{
                    py: '12px',
                    px: 2,
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    cursor: timelineItem?.description ? 'pointer' : 'auto',
                }}
                onClick={() => timelineItem?.description && handleOpenDialog()}
            >
                <Typography variant="h6" component="span" color={textColor} sx={{ width: "max-content" }}>
                    {timelineItem.title}
                </Typography>
                {timelineItem?.description &&
                    <Typography color={textColor} style={ellipsisStyle}>
                        {timelineItem.description}
                    </Typography>
                }
            </TimelineContent>
        </TimelineItem>
        {timelineItem.extraData && showDialog && <TimelineDialog open={showDialog} onClose={() => setShowDialog(false)} timeline={timelineItem} />}
    </>;
}