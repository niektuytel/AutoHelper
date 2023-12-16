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
    openDaysOfWeek: string[] | undefined;
}

export default ({ openDaysOfWeek }: IProps) => {
    const { t } = useTranslation();

    return (
        <Paper elevation={3} sx={{ marginTop: '16px', padding: 1 }}>
            <TableContainer>
                <Table size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell>
                                <CalendarTodayIcon style={{ marginRight: '8px' }} fontSize='small' />
                                {t("Opening hours")}
                            </TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {openDaysOfWeek?.map((day, index) => {
                            const splitted = day.split(": ");
                            const dayOfWeek = splitted[0];
                            const hours = splitted[1];

                            return <TableRow key={index}>
                                <TableCell>{dayOfWeek}</TableCell>
                                <TableCell>{hours}</TableCell>
                            </TableRow>;
                        })}
                    </TableBody>
                </Table>
            </TableContainer>
        </Paper>
    );
};