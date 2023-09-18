﻿import React, { useState } from "react";
import { Box, Button, ButtonGroup, CircularProgress, Container, Divider, IconButton, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { GarageServiceItemDto } from "../../../app/web-api-client";
import useGarageEmployees from "./useGarageEmployees";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import GarageEmployeeDrawer from "./components/GarageEmployeeDrawer";
import GarageEmployeeDeleteDialog from "./components/GarageEmployeeDeleteDialog";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));


    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [cartItems, setCartItems] = useState<GarageServiceItemDto[]>([]);
    const [dialogOpen, setDialogOpen] = useState<boolean>(false);
    const [dialogDeleteOpen, setDialogDeleteOpen] = useState(false);
    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");

    const handleFormSubmit = (data: any) => {
        if (dialogMode == "create" || dialogMode == "edit") {
            setDialogOpen(false);
        }

        if (selectedItem) {
            setDialogDeleteOpen(false);
        }

        setSelectedItem(undefined);
    };

    const { loading, createEmployee, updateEmployee, deleteEmployee, isError, garageEmployees } = useGarageEmployees(handleFormSubmit);

    // Sample data
    const handleAddClick = () => {
        setSelectedItem(undefined);
        setDialogMode("create");
        setDialogOpen(true);
    }

    const handleEditClick = () => {
        if (!selectedItem) return;

        setSelectedItem(selectedItem);
        setDialogMode("edit");
        setDialogOpen(true);
    }

    const handleDeleteClick = () => {
        if (!selectedItem) return;

        setDialogDeleteOpen(true);
    }

    return (
        <>
            <Box pt={4}>
                <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                    {t("Employees")}
                    {loading ?
                        <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                        :
                        <Tooltip title={t("Employees.Description")}>
                            <IconButton size="small">
                                <InfoOutlinedIcon fontSize="inherit" />
                            </IconButton>
                        </Tooltip>
                    }
                </Typography>
            </Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                {isMobile ?
                    <div>
                        <IconButton onClick={() => handleAddClick()}>
                            <AddIcon />
                        </IconButton>
                        <IconButton disabled={!selectedItem} onClick={() => handleEditClick()}>
                            <EditIcon />
                        </IconButton>
                        <IconButton disabled={!selectedItem} onClick={() => handleDeleteClick()}>
                            <DeleteIcon />
                        </IconButton>
                    </div>
                    :
                    <ButtonGroup aria-label="Buttons used for create, edit and delete">
                        <Button onClick={() => handleAddClick()}>
                            <AddIcon />{t("Add")}
                        </Button>
                        <Button onClick={() => handleEditClick()} disabled={!selectedItem}>
                            <EditIcon />{t("Afmelden")}
                        </Button>
                        <Button onClick={() => handleEditClick()} disabled={!selectedItem}>
                            <EditIcon />{t("Edit")}
                        </Button>
                        <Button onClick={() => handleDeleteClick()} disabled={!selectedItem}>
                            <DeleteIcon />{t("Delete")}
                        </Button>
                    </ButtonGroup>

                }
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <GarageEmployeeDeleteDialog
                service={selectedItem}
                confirmDeleteOpen={dialogDeleteOpen}
                setConfirmDeleteOpen={setDialogDeleteOpen}
                deleteService={deleteEmployee}
                loading={loading}
            />
            <GarageEmployeeDrawer
                mode={dialogMode}
                service={selectedItem}
                dialogOpen={dialogOpen}
                setDialogOpen={setDialogOpen}
                createEmployee={createEmployee}
                updateEmployee={updateEmployee}
                loading={loading}
            />
        </>
    );
}
