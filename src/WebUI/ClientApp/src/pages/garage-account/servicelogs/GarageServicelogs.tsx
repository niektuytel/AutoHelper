﻿import React, { useState } from "react";
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
import { GarageServiceDtoItem, GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogDtoItem } from "../../../app/web-api-client";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { Table, TableBody, TableContainer, TableHead, TableRow, Paper } from '@mui/material';

//import GarageServiceDialog from "./components/GarageServiceDialog";
//import { getTitleForServiceType } from "../defaultGarageService";
//import { COLORS } from "../../../constants/colors";
//import GarageServiceCardOther from "./components/GarageServiceCardOther";
//import GarageServiceCard from "./components/GarageServiceCard";
//import GarageServiceDeleteDialog from "./components/GarageServiceDeleteDialog";
//import GarageServicesCollectionCard from "./components/GarageServicesCollectionCard";
import { useDispatch } from "react-redux";
import { showOnError } from "../../../redux/slices/statusSnackbarSlice";
import useGarageServicelogs from "./useGarageServicelogs";
import GarageServiceLogCard from "./components/GarageServiceLogCard";
import GarageServiceLogDeleteDialog from "./components/GarageServiceLogDeleteDialog";
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

    const { loading, updateServiceLog, deleteServiceLog, isError, garageServiceLogs } = useGarageServicelogs(handleFormSubmit);
    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = (open: boolean) => {
        setDrawerOpen(open);
    };

    const handleAddService = () => {
        setDrawerOpen(true);
    };

    const handleService = (data: any) => {
        console.log("Handle service", data);

        if (dialogMode == "create") {
            //createServiceLog(data, file);
        }
        else if (dialogMode == "edit") {
            //updateServiceLog(data, file);
        }

        setDrawerOpen(false);
        setSelectedItem(undefined);
    };

    // Sample data
    const handleAddClick = () => {
        setSelectedItem(undefined);
        setDialogMode("create");
        setDrawerOpen(true);
    }

    const handleEditClick = (item: VehicleServiceLogAsGarageDtoItem) => {
        setSelectedItem(item);
        //if (!selectedItem) return;

        //updateServiceLog(item);
        //setDialogMode("edit");
        //setDrawerOpen(true);
    }

    const handleDeleteClick = (item: VehicleServiceLogAsGarageDtoItem) => {
        setSelectedItem(item);
        //if (!selectedItem) return;

        deleteServiceLog(item);
        //setDialogDeleteOpen(true);
    }

    const handleApprove = (item: VehicleServiceLogAsGarageDtoItem) => {

    }

    const tryAddCartItem = (itemToAdd: VehicleServiceLogAsGarageDtoItem) => {
        //if (cartItems.some(cartItem => cartItem.id === itemToAdd.id))
        //{
        //    dispatch(showOnError(t("Cart item already exist")));
        //    return;
        //} 

        //setCartItems([...cartItems, itemToAdd]);
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
                        {/*<IconButton disabled={!selectedItem} onClick={() => handleEditClick()}>*/}
                        {/*    <EditIcon />*/}
                        {/*</IconButton>*/}
                        {/*<IconButton disabled={!selectedItem} onClick={() => handleDeleteClick()}>*/}
                        {/*    <DeleteIcon />*/}
                        {/*</IconButton>*/}
                    </div>
                    :
                    <ButtonGroup aria-label="Buttons used for create, edit and delete">
                        <Button onClick={() => handleAddClick()}>
                            <AddIcon />{t("add")}
                        </Button>
                        {/*<Button onClick={() => handleEditClick()} disabled={!selectedItem}>*/}
                        {/*    <EditIcon />{t("edit")}*/}
                        {/*</Button>*/}
                        {/*<Button onClick={() => handleDeleteClick()} disabled={!selectedItem}>*/}
                        {/*    <DeleteIcon />{t("delete")}*/}
                        {/*</Button>*/}
                    </ButtonGroup>

                }
            </Box>
            <TableContainer component={Paper}>
                <Table aria-label="garage service logs table">
                    <TableHead>
                        <TableRow>
                            <TableCell>Date</TableCell>
                            <TableCell>License Plate</TableCell>
                            <TableCell>Type</TableCell>
                            <TableCell></TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {garageServiceLogs?.map((item) => (
                            <ServiceLogTableRow key={`service-log-row-${item.id}`} item={item} handleEdit={handleEditClick} handleDelete={handleDeleteClick} />
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <GarageServiceLogDeleteDialog
                service={selectedItem}
                confirmDeleteOpen={dialogDeleteOpen}
                setConfirmDeleteOpen={setDialogDeleteOpen}
                deleteService={deleteServiceLog}
                loading={loading}
            />

            <ServiceLogDrawer drawerOpen={drawerOpen} toggleDrawer={toggleDrawer} handleService={handleService} />
        </>
    );
}
