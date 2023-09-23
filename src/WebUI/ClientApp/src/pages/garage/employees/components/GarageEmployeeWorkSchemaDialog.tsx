import React, { forwardRef, useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import {
    Button, Dialog, DialogActions, DialogContent,
    DialogTitle, useTheme, useMediaQuery,
    Grid, Box
} from "@mui/material";
import { useTranslation } from "react-i18next";

// own imports
import { GarageEmployeeWorkSchemaItemDto } from "../../../../app/web-api-client";
import { DAYSINWEEKSHORT, DAYSINWEEK } from "../../../../constants/days";
import { DraggableHalfHour } from "./DraggableHalfHour";

type DraggableHourProps = {
    day: string;
    dayIndex: number;
    // Add any other props that you might need
};

//const dayStyles = {
//    flex: '1',
//    textAlign: 'center',
//    padding: '10px',
//    boxSizing: 'border-box',
//};

const dayStyles = {
    flexBasis: '14.284%',
    maxWidth: '14.284%'
};

interface IProps {
    mode: 'create' | 'edit';
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    workSchema: Array<GarageEmployeeWorkSchemaItemDto> | undefined;
    setWorkSchema: (data: Array<GarageEmployeeWorkSchemaItemDto>) => void;
}

export default function ExperienceDialog({ mode, dialogOpen, setDialogOpen, workSchema, setWorkSchema }: IProps) {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const hours = useMemo(() => Array.from({ length: 24 }, (_, i) => i), []);

    const [selectedRanges, setSelectedRanges] = useState<Array<{ time: number, day: number }>>([]);
    const [selectedHours, setSelectedHours] = useState<string[]>([]);
    const [isMouseDown, setIsMouseDown] = useState(false);
    const [draggingDay, setDraggingDay] = useState<number | null>(null);

    useEffect(() => {
        if (mode === "edit" && workSchema) {
            const ranges = workSchema.map(item => {
                const startMinutes = item.startTime.getHours() * 60 + item.startTime.getMinutes();
                const endMinutes = item.endTime.getHours() * 60 + item.endTime.getMinutes();
                const intervals = [];

                for (let i = startMinutes; i < endMinutes; i += 30) {
                    intervals.push({ time: i, day: item.dayOfWeek });
                }

                return intervals;
            }).flat(); // Flatten the array since the map will return a 2D array

            setSelectedRanges(ranges);
        } else {
            setSelectedRanges([]);
        }
    }, [mode, workSchema]);


    const handleMouseDown = (hour: number, minute: number, day: number) => {
        setIsMouseDown(true);
        setDraggingDay(day);

        handleHourClick(hour, minute, day);
    };

    const handleMouseEnter = (hour: number, minute: number) => {
        if (isMouseDown) {
            const endIdentifier = `${hour * 60 + minute}-${draggingDay!}`;
            setSelectedHours([...selectedHours, endIdentifier]);

            const range = { time: (hour * 60 + minute), day: draggingDay! };
            if (!selectedRanges.find(item => item.day === range.day && item.time === range.time)) {
                setSelectedRanges([...selectedRanges, range]);
            }
        }
    };

    const handleHourClick = (hour: number, minute: number, day: number) => {
        const range = { time: (hour * 60 + minute), day: day };
        if (selectedRanges.find(item => item.day === range.day && item.time === range.time)) {
            setSelectedRanges(selectedRanges.filter(r => !(r.day === range.day && r.time === range.time)));
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
        };

        window.addEventListener('mouseup', handleMouseUp);
        return () => window.removeEventListener('mouseup', handleMouseUp);
    }, [selectedRanges]);


    const handleSetWorkSchema = () => {
        const workSchemaItems: GarageEmployeeWorkSchemaItemDto[] = [];

        selectedRanges.sort((a, b) => a.day - b.day || a.time - b.time);

        let i = 0;
        while (i < selectedRanges.length) {
            const currentRange = selectedRanges[i];

            const startTime = new Date();
            startTime.setHours(Math.floor(currentRange.time / 60));
            startTime.setMinutes(currentRange.time % 60);
            startTime.setSeconds(0);
            startTime.setMilliseconds(0);

            let endTime = new Date(startTime);

            // Check for consecutive blocks
            while (i < selectedRanges.length - 1 && selectedRanges[i + 1].day === currentRange.day && selectedRanges[i + 1].time === selectedRanges[i].time + 30) {
                i++; // move to the next block in the array since it's connected
            }

            // Set the endTime based on the last block in the sequence
            endTime.setHours(Math.floor((selectedRanges[i].time + 30) / 60));
            endTime.setMinutes((selectedRanges[i].time + 30) % 60);

            const schemaItem = new GarageEmployeeWorkSchemaItemDto({
                weekOfYear: undefined,
                dayOfWeek: currentRange.day, // Rotate based on today
                startTime: startTime,
                endTime: endTime,
            });

            workSchemaItems.push(schemaItem);
            i++; // move to the next distinct block or sequence
        }

        setWorkSchema(workSchemaItems);
    }


    const DraggableDayComponent: React.FC<DraggableHourProps> = ({ day, dayIndex }) => {
        const seventhHourRef = useRef<HTMLDivElement>(null);

        useEffect(() => {
            if (seventhHourRef.current) {
                seventhHourRef.current.scrollIntoView({ behavior: 'smooth' });
            }
        }, []);

        return (
            <div key={day} style={{ borderRight: '1px solid gray', ...dayStyles }}>
                {hours.map(hour => (
                    [0, 30].map(minute => (
                        <DraggableHalfHour
                            ref={hour === 7 && minute === 0 ? seventhHourRef : null}
                            key={`${hour}:${minute}`}
                            hour={hour}
                            minute={minute}
                            day={dayIndex}
                            showHour={dayIndex === 0 || dayIndex === draggingDay}
                            isSelected={selectedRanges.find(item => item.day === dayIndex && item.time === (hour * 60 + minute)) !== undefined}
                            onMouseDown={handleMouseDown}
                            onMouseEnter={handleMouseEnter}
                            selectedRanges={selectedRanges}
                        />
                    ))
                ))}
            </div>
        );
    }


    return (
        <Dialog
            open={dialogOpen}
            onClose={() => setDialogOpen(false)}
            fullScreen={isMobile}
            sx={{ display: 'flex', flexDirection: 'column' }}
        >
            <DialogTitle>{t('schema_edit_title')}</DialogTitle>
            <Grid container style={{ width: '100%', paddingLeft: "16px", paddingRight: "16px" }}>
                {(isMobile ? DAYSINWEEKSHORT : DAYSINWEEK).map(day => (
                    <div key={day} style={dayStyles}>
                        <Box mb={2} textAlign="center">{t(day)}</Box>
                    </div>
                ))}
            </Grid>
            <form onSubmit={handleSetWorkSchema} style={{ minWidth: isMobile ? '100%' : '600px', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
                <DialogContent dividers sx={{ padding: 0, marginLeft: "15px", marginRight: isMobile ? "15px" : "0" }}>
                    <Grid container style={{ flex: 1, display: 'flex', width: '100%', overflowY: 'auto', borderLeft: '1px solid gray' }}>
                        {DAYSINWEEK.map((day, dayIndex) => (
                            <DraggableDayComponent day={day} dayIndex={dayIndex} />
                        ))}
                    </Grid>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDialogOpen(false)}>
                        {t("Cancel")}
                    </Button>
                    <Button type="submit" variant="contained" color="primary">
                        {t("add")}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );

}
