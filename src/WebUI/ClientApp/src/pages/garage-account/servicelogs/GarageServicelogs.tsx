import React, { useState } from "react";
import {
    Box,
    Button,
    Divider,
    IconButton,
    Tooltip,
    Typography,
    TextField,
    Card,
    CardHeader,
    CardContent,
    CardActions,
    CircularProgress,
    useTheme,
    useMediaQuery,
    Drawer,
    ButtonGroup,
    TableCell,
} from "@mui/material";
import { useTranslation } from "react-i18next";
//import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
//import AddIcon from '@mui/icons-material/Add';
//import EditIcon from '@mui/icons-material/Edit';
//import DeleteIcon from '@mui/icons-material/Delete';
//import AccessTimeIcon from '@mui/icons-material/AccessTime';
//import EuroIcon from '@mui/icons-material/Euro';
//import { useNavigate, useParams } from "react-router";
import { GarageServiceDtoItem, GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogDtoItem, VehicleServiceLogStatus } from "../../../app/web-api-client";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { Table, TableBody, TableContainer, TableHead, TableRow, Paper } from '@mui/material';

import { useDispatch } from "react-redux";
import useGarageServicelogs from "./useGarageServicelogs";
import ServiceLogDrawer from "./components/ServiceLogDrawer";
import ServiceLogTableRow from "./components/ServiceLogTableRow";

// own imports


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const dispatch = useDispatch();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedItem, setSelectedItem] = useState<any>(null);
    //const [cartItems, setCartItems] = useState<GarageServiceDtoItem[]>([]);
    //const [dialogOpen, setDrawerOpen] = useState<boolean>(false);
    const [dialogDeleteOpen, setDialogDeleteOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");

    const handleFormSubmit = (data: any) => {
        if (dialogMode == "create" || dialogMode == "edit") {
            setDrawerOpen(false);
        }

        if (selectedItem) {
            setDialogDeleteOpen(false);
        }

        setSelectedItem(undefined);
    };

    const { loading, createServiceLog, updateServiceLog, deleteServiceLog, isError, garageServiceLogs } = useGarageServicelogs(handleFormSubmit);
    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => {
        setDrawerOpen(open);
    };

    const handleService = (data: any, file: File | null) => {
        console.log("Handle service", data);

        if (dialogMode == "create") {
            createServiceLog(data, file);
        }
        else if (dialogMode == "edit") {
            updateServiceLog(data, file);
        }

        setDrawerOpen(false);
        setSelectedItem(undefined);
    };

    const handleUpdateServiceLog = (item: any, file: File | null) => {
        setSelectedItem(item);
        setDialogMode("edit");
        setDrawerOpen(true);
    }

    const handleApprove = (item: any, file: File | null) => {
        item.garageServiceId = 
        item.status = VehicleServiceLogStatus.VerifiedByGarage;
        updateServiceLog(item, file);
    }

    // Sample data
    const handleAddClick = () => {
        setSelectedItem(undefined);
        setDialogMode("create");
        setDrawerOpen(true);
    }

    return (
        <>
            <Box pt={4}>
                <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                    {t("GarageAccount.ServiceLogs.Title")}
                    {loading ?
                        <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                        :
                        <Tooltip title={t("GarageAccount.ServiceLogs.Description")}>
                            <IconButton size="small">
                                <InfoOutlinedIcon fontSize="inherit" />
                            </IconButton>
                        </Tooltip>
                    }
                </Typography>
            </Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" sx={{ marginBottom: 2 }} >
                {isMobile ?
                    <div>
                        <IconButton onClick={() => handleAddClick()}>
                            <AddIcon />
                        </IconButton>
                    </div>
                    :
                    <ButtonGroup aria-label="Buttons used for create, edit and delete">
                        <Button onClick={() => handleAddClick()}>
                            <AddIcon />{t("add")}
                        </Button>
                    </ButtonGroup>

                }
            </Box>
            <TableContainer component={Paper}>
                <Table aria-label="garage service logs table">
                    <TableHead>
                        <TableRow>
                            <TableCell sx={{ width: '1%', whiteSpace: 'nowrap', padding: '8px' }}>Date</TableCell>
                            <TableCell sx={{ width: '1%', whiteSpace: 'nowrap', padding: '8px' }}>License Plate</TableCell>
                            <TableCell sx={{ flexGrow: 1, minWidth: 0, padding: '8px' }}>Type</TableCell>
                            <TableCell sx={{ width: '1%', whiteSpace: 'nowrap', padding: '8px' }}></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {garageServiceLogs?.map((item) => (
                            <ServiceLogTableRow key={`service-log-row-${item.id}`} item={item} handleEdit={handleUpdateServiceLog} handleApprove={handleApprove} handleDelete={deleteServiceLog} />
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <ServiceLogDrawer
                drawerOpen={drawerOpen}
                toggleDrawer={toggleDrawer}
                handleService={handleService}
            />
        </>
    );
}
