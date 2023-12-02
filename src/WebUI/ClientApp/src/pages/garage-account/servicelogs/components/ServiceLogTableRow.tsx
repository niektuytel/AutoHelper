import { useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button, Collapse, Box, Typography, IconButton } from '@mui/material';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AcceptIcon from '@mui/icons-material/CheckCircleOutline'; // You can change this to the appropriate Accept icon
import DeclineIcon from '@mui/icons-material/Cancel'; // You can change this to the appropriate Decline icon
import { VehicleServiceLogAsGarageDtoItem, VehicleServiceLogStatus } from '../../../../app/web-api-client';

interface IProps {
    item: VehicleServiceLogAsGarageDtoItem
    handleEdit: (item: VehicleServiceLogAsGarageDtoItem) => void;
    handleDelete: (item: VehicleServiceLogAsGarageDtoItem) => void;
}

export default ({ item, handleEdit, handleDelete }: IProps) => {
    const [open, setOpen] = useState(false);

    const handleAccept = (e: any, item: VehicleServiceLogAsGarageDtoItem) => {
        e.stopPropagation();

        item.status = VehicleServiceLogStatus.VerifiedByGarage;
        handleEdit(item);
    };

    const handleDecline = (e: any, item: VehicleServiceLogAsGarageDtoItem) => {
        e.stopPropagation();

        handleDelete(item);
    };

    const isNotVerified = item.status === VehicleServiceLogStatus.NotVerified;
    const isVerified = item.status === VehicleServiceLogStatus.VerifiedByGarage;
    const backgroundColor = isVerified ? '#e8ffe8' : '#ffeade';

    return (
        <>
            <TableRow sx={{ '& > *': { borderBottom: 'unset', backgroundColor } }} onClick={() => setOpen(!open)}>
                <TableCell component="th" scope="row">
                    {item.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                </TableCell>
                <TableCell>{item.vehicleLicensePlate}</TableCell>
                <TableCell>{item.type}</TableCell>
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
