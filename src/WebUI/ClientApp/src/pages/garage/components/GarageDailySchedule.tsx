import {
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import { DAYSINWEEK } from '../../../constants/days';

interface IProps {
    openDaysOfWeek: number[];
}

const DailySchedule = ({ openDaysOfWeek }:IProps) => {
    // TODO: also show the time range for each day
    return (
        <Paper elevation={3} sx={{ marginTop: '16px', padding: 1 }}>
            <TableContainer>
                <Table size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell>
                                <CalendarTodayIcon style={{ marginRight: '8px' }} fontSize='small' />
                                Openings tijden
                            </TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {DAYSINWEEK.map((day, index) => (
                            <TableRow key={index}>
                                <TableCell>{day}</TableCell>
                                <TableCell>
                                    {openDaysOfWeek.includes(index) ? "Open" : "Closed"}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Paper>
    );
};

export default DailySchedule;
