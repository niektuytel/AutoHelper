import { forwardRef } from "react";


const hourStyles = {
    borderBottom: '1px solid gray',
    height: '40px',
    fontSize: "small"
};

function findLastConnectedRange(time: number, day: number, selectedRanges: Array<{ time: number, day: number }>): { time: number, day: number } {
    const nextItem = selectedRanges.find(item => item.day === day && item.time === time + 30);

    if (!nextItem) {
        return { time: time, day: day };
    } else {
        return findLastConnectedRange(nextItem.time, nextItem.day, selectedRanges);
    }
}

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

export const DraggableHalfHour = forwardRef<HTMLDivElement, DraggableHalfHourProps>((props, ref) => {
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