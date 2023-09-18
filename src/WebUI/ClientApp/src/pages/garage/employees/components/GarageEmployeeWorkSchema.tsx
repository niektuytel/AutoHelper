import React, { useEffect, useMemo, useState } from "react";
import {
    useTheme,
    useMediaQuery,
    Grid,
    Box
} from "@mui/material";
import { useTranslation } from "react-i18next";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";

// own imports

const dayStyles = {
    flexBasis: '14.284%',
    maxWidth: '14.284%'
};

const droppableStyles = {
    borderRight: '1px solid gray'
};

const hourStyles = {
    borderBottom: '1px solid gray',
    height: '20px',
    lineHeight: '20px',
};

interface DroppableDayProps {
    day: number;
    children?: React.ReactNode;
}

const daysOfWeek = [
    "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday"
];

const createRange = (value1: number, value2: number, day: number) => {
    return {
        start: Math.min(value1, value2),
        end: Math.max(value1, value2),
        day
    };
};

const timeInRange = (time: number, day: number, ranges: Array<{ start: number, end: number, day: number }>) => {
    return ranges.some(range => range.day === day && time >= range.start && time <= range.end);
};


const DroppableDay: React.FC<DroppableDayProps> = ({ day, children }) => {
    const [{ isOver }, ref] = useDrop({
        accept: "HOUR",
        drop: (item: { hour: number }) => {
            console.log(`Dropped hour ${item.hour} on day ${day}`);
        },
        collect: (monitor) => ({
            isOver: !!monitor.isOver()
        })
    });

    const opacity = useMemo(() => (isOver ? 0.5 : 1), [isOver]);

    return <div ref={ref} style={{ ...droppableStyles, opacity }}>{children}</div>;
};

interface DraggableHalfHourProps {
    hour: number;
    minute: number;
    day: number;
    showHour: boolean;
    isSelected: boolean;
    onMouseDown: (hour: number, minute: number, day: number) => void;
    onMouseEnter: (hour: number, minute: number) => void;
    selectedRanges: Array<{ start: number, end: number, day: number }>;
}

const DraggableHalfHour: React.FC<DraggableHalfHourProps> = (props) => {
    const { hour, minute, day, showHour, isSelected, onMouseDown, onMouseEnter, selectedRanges } = props;
    const formattedHour = hour.toString().padStart(2, '0');
    const formattedMinute = minute.toString().padStart(2, '0');

    var background = isSelected ? 'lightblue' : 'transparent';
    if (!isSelected && (hour < 8 || hour > 17)) {
        background = 'lightgray';
    }

    const currentIdentifier = hour * 60 + minute;
    const matchingRange = selectedRanges.find(range =>
        day === range.day && range.start === currentIdentifier
    );

    let displayText = `${formattedHour}:${formattedMinute}`;
    if (matchingRange) {
        const startHour = (Math.floor(matchingRange.start / 60)).toString().padStart(2, '0');
        const startMinutes = (matchingRange.start % 60).toString().padStart(2, '0');
        const endHour = (Math.floor(matchingRange.end / 60)).toString().padStart(2, '0');
        const endMinutes = (matchingRange.end % 60).toString().padStart(2, '0');
        displayText = `${startHour}:${startMinutes}-${endHour}:${endMinutes}`;
    }

    return (
        <div
            onMouseDown={() => onMouseDown(hour, minute, day)}
            onMouseEnter={() => onMouseEnter(hour, minute)}
            style={{ ...hourStyles, background }}
        >
            {(showHour || matchingRange) && <span style={{ userSelect: "none" }}>{displayText}</span>}
        </div>
    );
};

interface IProps {
}

export default ({  }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const hours = useMemo(() => Array.from({ length: 24 }, (_, i) => i), []);

    const [selectedRanges, setSelectedRanges] = useState<Array<{ start: number, end: number, day: number }>>([]);
    const [selectedHours, setSelectedHours] = useState<string[]>([]);
    const [isMouseDown, setIsMouseDown] = useState(false);
    const [draggingDay, setDraggingDay] = useState<number | null>(null);

    const handleMouseDown = (hour: number, minute: number, day: number) => {
        setIsMouseDown(true);
        setDraggingDay(day);

        handleHourClick(hour, minute, day);
    };

    const handleMouseEnter = (hour: number, minute: number) => {
        if (isMouseDown) {
            const endIdentifier = `${hour * 60 + minute}-${draggingDay!}`;
            setSelectedHours([...selectedHours, endIdentifier]);
            setSelectedRanges(prevRanges => {
                const lastRange = prevRanges[prevRanges.length - 1];
                if (lastRange && lastRange.day === draggingDay) {

                    const range = createRange(lastRange.start, (hour * 60 + minute), draggingDay);
                    return [...prevRanges.slice(0, -1), range];
                }
                return prevRanges;
            });
        }
    };


    const handleHourClick = (hour: number, minute: number, day: number) => {
        const inMinutes = hour * 60 + minute;
        const nextInMinutes = inMinutes + 30;

        const range = createRange(inMinutes, nextInMinutes, day);
        if (timeInRange(inMinutes, day, selectedRanges)) {
            setSelectedRanges(selectedRanges.filter(r => !(r.day === range.day && r.start === range.start && r.end === range.end)));
        } else {
            setSelectedRanges([...selectedRanges, range]);
        }

        const identifier = `${hour * 60 + minute}-${day}`;
        if (selectedHours.includes(identifier)) {
            setSelectedHours(selectedHours.filter(h => h !== identifier));
        } else {
            setSelectedHours([...selectedHours, identifier]);
        }
    };

    useEffect(() => {
        const handleMouseUp = () => {
            setIsMouseDown(false);
            setDraggingDay(null);

            //// First, sort the ranges by day, start, and end.
            //const sortedRanges = [...selectedRanges].sort((a, b) => {
            //    return a.day - b.day;
            //});

            //// Now, iterate through the sorted ranges to merge them
            //const mergedRanges = [];
            //let currentRange = sortedRanges[0];

            //for (let i = 1; i < sortedRanges.length; i++) {
            //    if (currentRange.end === sortedRanges[i].start && currentRange.day === sortedRanges[i].day) {
            //        // If the end of current range matches the start of next range and they are on the same day, merge them.
            //        currentRange = { start: currentRange.start, end: sortedRanges[i].end, day: currentRange.day };
            //    } else {
            //        // Otherwise, add the current range to the result and set the next range as current.
            //        mergedRanges.push(currentRange);
            //        currentRange = sortedRanges[i];
            //    }
            //}
            //mergedRanges.push(currentRange);

            //setSelectedRanges(mergedRanges);
        };

        window.addEventListener('mouseup', handleMouseUp);
        return () => window.removeEventListener('mouseup', handleMouseUp);
    }, [selectedRanges]);

    return (
        <>
        <DndProvider backend={HTML5Backend}>
            <Grid container style={{ width: '100%', paddingRight: "16px" }}>
                {daysOfWeek.map((day) => (
                    <Grid item xs={12} md={1} key={day} style={dayStyles}>
                        <Box mb={2} textAlign="center">{day}</Box>
                    </Grid>
                ))}
            </Grid>
            <Grid container style={{ width: '100%', maxHeight: '400px', overflowY: 'scroll', border: '1px solid gray' }}>
                {daysOfWeek.map((day, dayIndex) => (
                    <Grid item xs={12} md={1} key={day} style={dayStyles}>
                        <DroppableDay day={dayIndex}>
                            {hours.map(hour => (
                                [0, 30].map(minute => (
                                    <DraggableHalfHour
                                        key={`${hour}:${minute}`}
                                        hour={hour}
                                        minute={minute}
                                        day={dayIndex}
                                        isSelected={selectedHours.includes(`${hour * 60 + minute}-${dayIndex}`)}
                                        showHour={dayIndex === 0 || dayIndex === draggingDay}
                                        onMouseDown={handleMouseDown}
                                        onMouseEnter={handleMouseEnter}
                                        selectedRanges={selectedRanges}  // Pass this prop
                                    />

                                ))
                            ))}
                        </DroppableDay>
                    </Grid>
                ))}
            </Grid>
        </DndProvider>
        </>
    );
}
