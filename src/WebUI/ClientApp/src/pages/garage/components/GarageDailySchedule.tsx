import {
    Paper,
    Skeleton,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography,
    useMediaQuery,
    useTheme
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import { DAYSINWEEK } from '../../../constants/days';
import { useTranslation } from 'react-i18next';

interface IProps {
    openDaysOfWeek: number[] | undefined;
}

export default ({ openDaysOfWeek }: IProps) => {
    const { t } = useTranslation();

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
                                <TableCell>{t(day)}</TableCell>
                                <TableCell>
                                    {openDaysOfWeek ?
                                        openDaysOfWeek.includes(index) ? t("Open") : t("Closed")
                                        :
                                        <Skeleton />
                                    }
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Paper>
    );
};