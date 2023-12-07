import { useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button, Collapse, Box, Typography, IconButton, Chip } from '@mui/material';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AcceptIcon from '@mui/icons-material/CheckCircleOutline'; // You can change this to the appropriate Accept icon
import DeclineIcon from '@mui/icons-material/Cancel'; // You can change this to the appropriate Decline icon
import { GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogStatus } from '../../../../app/web-api-client';
import { useTranslation } from 'react-i18next';
import { getFormatedLicense } from '../../../../app/LicensePlateUtils';
import { ROUTES } from '../../../../constants/routes';

interface IProps {
    item: VehicleServiceLogAsGarageDtoItem
    handleEdit: (item: VehicleServiceLogAsGarageDtoItem, file: File|null) => void;
    handleDelete: (item: VehicleServiceLogAsGarageDtoItem) => void;
}

export default ({ item, handleEdit, handleDelete }: IProps) => {
    const {t} = useTranslation(["serviceTypes"]);
    const [open, setOpen] = useState(false);
    const [file, setFile] = useState<File | null>(null);

    const handleAccept = (e: any, item: VehicleServiceLogAsGarageDtoItem) => {
        e.stopPropagation();

        item.status = VehicleServiceLogStatus.VerifiedByGarage;
        handleEdit(item, file);
    };

    const handleDecline = (e: any, item: VehicleServiceLogAsGarageDtoItem) => {
        e.stopPropagation();

        handleDelete(item);
    };

    const gotoVehiclePage = (e: any) => {
        e.stopPropagation();

        // Then the page has an fresh cache, that the services are up to date
        window.location.href = `${ROUTES.SELECT_VEHICLE}/${item.vehicleLicensePlate}`;
    }

    const isNotVerified = item.status === VehicleServiceLogStatus.NotVerified;
    const isVerified = item.status === VehicleServiceLogStatus.VerifiedByGarage;
    const backgroundColor = isVerified ? '#e8ffe8' : '#ffeade';

    return (
        <>
            <TableRow sx={{ '& > *': { borderBottom: 'unset', backgroundColor } }} onClick={() => setOpen(!open)}>
                <TableCell component="th" scope="row">
                    {item.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                </TableCell>
                <TableCell>
                    <Chip
                        label={getFormatedLicense(item.vehicleLicensePlate!)}
                        variant="outlined"
                        onClick={gotoVehiclePage}
                    />
                </TableCell>
                <TableCell>{item.title}</TableCell>
                <TableCell>
                    {isNotVerified && (
                        <>
                            <Button variant="contained" color="success" onClick={(e) => handleAccept(e, item)}>
                                <AcceptIcon />
                            </Button>
                            <Button variant="contained" color="error" onClick={(e) => handleDecline(e, item)}>
                                <DeclineIcon />
                            </Button>
                        </>
                    )}
                    {isVerified &&
                        <>
                        Geverifieerd
                        </>
                    }
                </TableCell>
                <TableCell>
                    {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
                </TableCell>
            </TableRow>
            <TableRow sx={{ '& > *': { backgroundColor } }} >
                <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Table size="small">
                            <TableHead>
                                <TableRow>
                                    <TableCell></TableCell>
                                    <TableCell></TableCell>
                                    <TableCell>Volgend onderhoud</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                <TableRow>
                                    <TableCell>Date</TableCell>
                                    <TableCell>{item.date?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}</TableCell>
                                    <TableCell>{item.expectedNextDate?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}</TableCell>
                                </TableRow>
                                <TableRow>
                                    <TableCell>OdometerReading</TableCell>
                                    <TableCell>{item.odometerReading}</TableCell>
                                    <TableCell>{item.expectedNextOdometerReading}</TableCell>
                                </TableRow>
                                <TableRow>
                                    <TableCell>Attached File</TableCell>
                                    <TableCell>{item.status}</TableCell>
                                    <TableCell></TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </Collapse>
                </TableCell>
            </TableRow>
        </>
    );
};
