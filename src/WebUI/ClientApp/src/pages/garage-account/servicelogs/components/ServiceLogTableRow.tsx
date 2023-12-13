import { useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button, Collapse, Box, Typography, IconButton, Chip, Tooltip } from '@mui/material';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import EditIcon from '@mui/icons-material/Edit';
import VerifiedIcon from '@mui/icons-material/Verified';
import DeleteIcon from '@mui/icons-material/Delete';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import AcceptIcon from '@mui/icons-material/CheckCircleOutline'; // You can change this to the appropriate Accept icon
import DeclineIcon from '@mui/icons-material/Cancel'; // You can change this to the appropriate Decline icon
import { GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogStatus } from '../../../../app/web-api-client';
import { useTranslation } from 'react-i18next';
import { getFormatedLicense } from '../../../../app/LicensePlateUtils';
import { ROUTES } from '../../../../constants/routes';

interface IProps {
    item: VehicleServiceLogAsGarageDtoItem
    handleApprove: (item: VehicleServiceLogAsGarageDtoItem, file: File | null) => void;
    handleEdit: (item: VehicleServiceLogAsGarageDtoItem, file: File|null) => void;
    handleDelete: (item: VehicleServiceLogAsGarageDtoItem) => void;
}

export default ({ item, handleApprove, handleEdit, handleDelete }: IProps) => {
    const {t} = useTranslation([ "translations", "serviceTypes" ]);
    const [open, setOpen] = useState(false);
    const [file, setFile] = useState<File | null>(null);

    const handleAccept = (e: any, item: VehicleServiceLogAsGarageDtoItem) => {
        e.stopPropagation();

        console.log(item)
        handleApprove(item, file);
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
            <TableRow sx={{
                '& > *': { borderBottom: 'unset', backgroundColor },
                '&:hover': {
                    cursor: 'pointer'
                }
            }} onClick={() => setOpen(!open)}>
                <TableCell component="th" scope="row" sx={{ padding: '8px' }}>
                    {item.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: '2-digit' })}
                </TableCell>
                <TableCell sx={{ padding: '8px' }}>
                    <Chip
                        label={getFormatedLicense(item.vehicleLicensePlate!)}
                        variant="outlined"
                        onClick={gotoVehiclePage}
                    />
                </TableCell>
                <TableCell sx={{ padding: '8px' }}>{item.title}</TableCell>
                <TableCell>
                    {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
                </TableCell>
            </TableRow>
            <TableRow sx={{ '& > *': { backgroundColor } }} >
                <TableCell sx={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Box sx={{ maxWidth: { xs: 320, sm: 480, md:"100%" }, mt:1, mb:2 }}>
                            <Typography variant="subtitle2">
                                {t("AddMaintenanceLog.Date.Label")}: {item.date?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: '2-digit' })} {item.expectedNextDate && `t/m ${item.expectedNextDate?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: '2-digit' })}`}
                            </Typography>
                            <Typography variant="subtitle2">
                                {t("AddMaintenanceLog.OdometerReading.Label")}: {item.odometerReading} {item.expectedNextOdometerReading && `t/m ${item.expectedNextOdometerReading}`}
                            </Typography>
                            <Typography variant="subtitle1" gutterBottom component="div">
                                {item.description}
                            </Typography>
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                {isNotVerified ? (
                                    <>
                                        <Button variant="contained" color="success" onClick={(e) => handleAccept(e, item)} sx={{ mr: 1 }}>
                                            <AcceptIcon />
                                        </Button>
                                        <Button variant="contained" color="error" onClick={(e) => handleDecline(e, item)}>
                                            <DeclineIcon />
                                        </Button>
                                    </>
                                ) : (
                                    <Tooltip title={t("ServiceLog.Verified.Title")}>
                                        <Chip
                                            color="success"
                                            icon={<VerifiedIcon />}
                                            label={t("ServiceLog.Verified.Label")}
                                            variant="outlined"
                                        />
                                    </Tooltip>
                                )}
                                {item.attachedFile && (
                                    <Chip
                                        icon={<AttachFileIcon />}
                                        label={"Bijlage"}
                                        variant="outlined"
                                        onClick={() => window.open(item.attachedFile, '_blank')}
                                        sx={{ mr: 1 }}
                                    />
                                )}
                                <IconButton aria-label="edit" onClick={(e) => handleEdit(item, file)} sx={{ marginLeft: 'auto' }}>
                                    <EditIcon />
                                </IconButton>
                            </Box>
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>
        </>
    );
};
