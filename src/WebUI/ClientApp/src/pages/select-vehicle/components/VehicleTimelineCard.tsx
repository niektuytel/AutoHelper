import * as React from 'react';
import Timeline from '@mui/lab/Timeline';
import { timelineOppositeContentClasses } from '@mui/lab/TimelineOppositeContent';

// custom imports
import useVehicleTimelineCard from '../useVehicleTimelineCard';
import VehicleTimelineItemSkeleton from './VehicleTimelineItemSkeleton';
import VehicleTimelineItem from './VehicleTimelineItem';
import VehicleTimelineAddItem from './VehicleTimelineAddItem';

interface IProps {
    license_plate: string
}

export default ({ license_plate }:IProps) => {
    const { loading, vehicleTimelineCard } = useVehicleTimelineCard(license_plate);

    return (
        <>
            <Timeline position="right" sx={{
                [`& .${timelineOppositeContentClasses.root}`]: {
                    flex: 0.2,
                    minWidth: "110px"
                },
            }}>
                <VehicleTimelineAddItem/>
                {loading ?
                    Array.from({ length: 4 }).map((_, index) => (
                        <VehicleTimelineItemSkeleton key={`VehicleTimelineItemSkeleton-${index}`} usedIndex={index} />
                    ))
                    :
                    vehicleTimelineCard?.map((timelineItem, index) => (
                        <VehicleTimelineItem key={`VehicleTimelineItem-${index}`} timelineItem={timelineItem} />
                    ))
                }
            </Timeline>
        </>
    );
}