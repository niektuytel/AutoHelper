import React, { forwardRef, useEffect, useMemo, useState } from "react";
import {
    Button, IconButton, Dialog, DialogActions, DialogContent,
    DialogTitle, TextField, useTheme, useMediaQuery, Select,
    InputAdornment, MenuItem, FormControl, InputLabel,
    CircularProgress, ListItemText, List, ListItem, Drawer,
    Grid, Divider, Box
} from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import AddIcon from '@mui/icons-material/Add';

import {
    Controller, useForm
} from "react-hook-form";

import {
    GarageEmployeeWorkExperienceItemDto, GarageEmployeeWorkSchemaItemDto, GarageServiceItemDto,
    UpdateGarageEmployeeCommand
} from "../../../../app/web-api-client";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router";
import { useQuery, useQueryClient } from "react-query";
import { useDispatch } from "react-redux";

import { ROUTES } from "../../../../constants/routes";
import { getTitleForServiceType } from "../../defaultGarageService";
import useGarageEmployees from "../useGarageEmployees";
import useConfirmationStep from "../../../../hooks/useConfirmationStep";
import useUserRole from "../../../../hooks/useUserRole";
import { GetGarageClient } from "../../../../app/GarageClient";
import { useTranslation } from "react-i18next";
import { DndProvider, useDrag, useDrop } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import { DAYSINWEEKSHORT, DAYSINWEEK } from "../../../../constants/days";

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
    height: '40px',
    fontSize: "small"
};

interface DroppableDayProps {
    day: number;
    children?: React.ReactNode;
}


function findLastConnectedRange(time: number, day: number, selectedRanges: Array<{ time: number, day: number }>): { time: number, day: number } {
    const nextItem = selectedRanges.find(item => item.day === day && item.time === time + 30);

    if (!nextItem) {
        return { time: time, day: day };
    } else {
        return findLastConnectedRange(nextItem.time, nextItem.day, selectedRanges);
    }
}



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
    selectedRanges: Array<{ time: number, day: number }>;
}

const DraggableHalfHour = forwardRef<HTMLDivElement, DraggableHalfHourProps>((props, ref) => {
    const { hour, minute, day, showHour, isSelected, onMouseDown, onMouseEnter, selectedRanges } = props;
    const formattedHour = hour.toString().padStart(2, '0');
    const formattedMinute = minute.toString().padStart(2, '0');

    var background = isSelected ? 'lightblue' : 'transparent';
    if (!isSelected && (hour < 8 || hour > 17)) {
        background = 'lightgray';
    }

    let displayText = "";
    const previousItem = selectedRanges.find(item => item.day === day && item.time === (hour * 60 + minute - 30));
    if (isSelected && !previousItem) {
        const lastRangeItem = findLastConnectedRange((hour * 60 + minute), day, selectedRanges);
        lastRangeItem.time += 30;// set to end of range

        const lastFormattedHour = (Math.floor(lastRangeItem.time / 60)).toString().padStart(2, '0');
        const lastFormattedMinute = (lastRangeItem.time % 60).toString().padStart(2, '0');
        displayText = `${formattedHour}:${formattedMinute}-${lastFormattedHour}:${lastFormattedMinute}`;
    } else if ((isSelected && showHour) || showHour) {
        displayText = `${formattedHour}:${formattedMinute}`;
    }

    return (
        <div
            ref={ref as any}
            onMouseDown={() => onMouseDown(hour, minute, day)}
            onMouseEnter={() => onMouseEnter(hour, minute)}
            style={{ ...hourStyles, background }}
        >
            <span style={{ userSelect: "none" }}>{displayText}</span>
        </div>
    );
});

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
    const slotRefs = useMemo(() => new Map<string, React.RefObject<HTMLDivElement>>(), []);
    hours.forEach(hour => {
        [0, 30].forEach(minute => {
            const key = `${hour}:${minute}`;
            if (!slotRefs.has(key)) {
                slotRefs.set(key, React.createRef());
            }
        });
    });

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

    useEffect(() => {
        const scrollToSlot = slotRefs.get("7:0"); // Note: the minute is "0", not "00"
        if (scrollToSlot?.current) {
            scrollToSlot.current.scrollIntoView({
                behavior: "smooth"
            });
        }
    }, []);

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

    return (
        <Dialog
            open={dialogOpen}
            onClose={() => setDialogOpen(false)}
            fullScreen={isMobile}
            sx={{ display: 'flex', flexDirection: 'column' }}
        >
            <DialogTitle>{t('Set WorkSchema')}</DialogTitle>
            <DialogContent
                style={{
                    minWidth: isMobile ? undefined : '600px',
                    flex: 1, // Ensures content fills the available space
                    display: 'flex', // Flexbox for the content
                    flexDirection: 'column', // Column direction for the flex items
                    overflow: 'hidden' // To ensure overflow from children does not break the layout
                }}
            >
                <Grid container style={isMobile ? { width: '100%' } : { width: '100%', paddingRight: "16px" }}>
                    {(isMobile ? DAYSINWEEKSHORT : DAYSINWEEK).map((day) => (
                        <Grid item xs={12} md={1} key={day} style={dayStyles}>
                            <Box mb={2} textAlign="center">{day}</Box>
                        </Grid>
                    ))}
                </Grid>
                <DndProvider backend={HTML5Backend}>
                    <Grid container style={{ flex: 1, display: 'flex', width: '100%', overflowY: 'auto', border: '1px solid gray' }}>
                        {DAYSINWEEK.map((day, dayIndex) => (
                            <Grid item xs={12} md={1} key={day} style={dayStyles}>
                                <DroppableDay day={dayIndex}>
                                    {hours.map(hour => (
                                        [0, 30].map(minute => (
                                            <DraggableHalfHour
                                                ref={slotRefs.get(`${hour}:${minute}`)}
                                                key={`${hour}:${minute}`}
                                                hour={hour}
                                                minute={minute}
                                                day={dayIndex}
                                                showHour={dayIndex === 0 || dayIndex === draggingDay}
                                                isSelected={selectedRanges.find(item => item.day === dayIndex && item.time === (hour * 60 + minute)) != undefined}
                                                onMouseDown={handleMouseDown}
                                                onMouseEnter={handleMouseEnter}
                                                selectedRanges={selectedRanges}
                                            />
                                        ))
                                    ))}
                                </DroppableDay>
                            </Grid>
                        ))}
                    </Grid>
                </DndProvider>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setDialogOpen(false)}>
                    {t("Cancel")}
                </Button>
                <Button onClick={handleSetWorkSchema} variant="contained" color="primary">
                    {t("Confirm")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
