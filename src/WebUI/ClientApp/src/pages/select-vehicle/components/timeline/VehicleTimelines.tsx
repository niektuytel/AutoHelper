import React, { useEffect, useState } from "react";

// custom imports
import useVehicleTimeline from "../../useVehicleTimeline";
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
                <VehicleTimelineItemSkeleton key={`VehicleTimelineItemSkeleton-${index}`} usedIndex={index} />
            ))
            :
            vehicleTimeline?.map((timelineItem, index) => (
                <VehicleTimelineItem key={`VehicleTimelineItem-${index}`} timelineItem={timelineItem} textColor={'black'} />
            ))
        }
    </>
}
