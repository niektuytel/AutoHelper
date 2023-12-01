import * as React from 'react';
import TimelineItem from '@mui/lab/TimelineItem';
import TimelineSeparator from '@mui/lab/TimelineSeparator';
import TimelineConnector from '@mui/lab/TimelineConnector';
import TimelineContent from '@mui/lab/TimelineContent';
import TimelineOppositeContent from '@mui/lab/TimelineOppositeContent';
import TimelineDot from '@mui/lab/TimelineDot';
import Person4OutlinedIcon from '@mui/icons-material/Person4Outlined';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import Typography from '@mui/material/Typography';
import { Tooltip } from '@mui/material';

// custom imports
import { VehicleTimelineDtoItem, VehicleTimelineType } from '../../../app/web-api-client';
import { useDrawer } from '../ServiceLogDrawerProvider';

// CSS for the ellipsis
const ellipsisStyle:any = {
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    maxWidth: '250px', // Set a max-width as per your design requirements
};

interface IProps {
    textColor?: string
}

export default ({ textColor = "white" }: IProps) => {
    const { toggleDrawer } = useDrawer();

    const openServiceLogDrawer = () => {
        toggleDrawer(true);
    };

    return (
        <TimelineItem sx={{ width: "min-content" }}>
            <TimelineOppositeContent
                sx={{ m: 'auto 0' }}
                align="right"
                variant="body2"
                color={textColor}
            >
                {/*{timelineItem?.date?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}*/}
            </TimelineOppositeContent>
            <TimelineSeparator>
                <TimelineConnector />
                    <TimelineDot color="success" variant="outlined" sx={{ bgcolor: 'white' }} >
                        <AddRoundedIcon sx={{ color: "#2E7D32" }} />
                    </TimelineDot>
                <TimelineConnector />
            </TimelineSeparator>
            <TimelineContent sx={{
                py: '12px',
                px: 2,
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center'
            }}>
            </TimelineContent>
        </TimelineItem>
    );
}