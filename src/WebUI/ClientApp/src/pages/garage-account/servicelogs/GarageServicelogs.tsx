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
} from "@mui/material";
import { useTranslation } from "react-i18next";
//import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
//import AddIcon from '@mui/icons-material/Add';
//import EditIcon from '@mui/icons-material/Edit';
//import DeleteIcon from '@mui/icons-material/Delete';
//import AccessTimeIcon from '@mui/icons-material/AccessTime';
//import EuroIcon from '@mui/icons-material/Euro';
//import { useNavigate, useParams } from "react-router";
import { GarageServiceDtoItem, GarageServiceType } from "../../../app/web-api-client";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
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
import GarageServiceLogDialog from "./components/GarageServiceLogDialog";

// own imports


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const dispatch = useDispatch();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [cartItems, setCartItems] = useState<GarageServiceDtoItem[]>([]);
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

    const { loading, createService, updateService, deleteService, isError, garageServices } = useGarageServicelogs(handleFormSubmit);

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

    const tryAddCartItem = (itemToAdd: GarageServiceDtoItem) => {
        if (cartItems.some(cartItem => cartItem.id === itemToAdd.id))
        {
            dispatch(showOnError(t("Cart item already exist")));
            return;
        } 

        setCartItems([...cartItems, itemToAdd]);
    }


    return (
        <>

            <Box pt={4}>
                <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                    {t("Services")}
                    {loading ?
                        <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                        :
                        <Tooltip title={t("Services.Description")}>
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
                            <AddIcon />{t("add")}
                        </Button>
                        <Button onClick={() => handleEditClick()} disabled={!selectedItem}>
                            <EditIcon />{t("edit")}
                        </Button>
                        <Button onClick={() => handleDeleteClick()} disabled={!selectedItem}>
                            <DeleteIcon />{t("delete")}
                        </Button>
                    </ButtonGroup>

                }
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            {garageServices?.map((item) => item &&
                <GarageServiceLogCard
                    key={`service-card-${item.id}`}
                    service={item}
                    selectedItem={selectedItem}
                    setSelectedItem={setSelectedItem}
                    addCartItem={tryAddCartItem}
                />
            )}
            <GarageServiceLogDeleteDialog
                service={selectedItem}
                confirmDeleteOpen={dialogDeleteOpen}
                setConfirmDeleteOpen={setDialogDeleteOpen}
                deleteService={deleteService}
                loading={loading}
            />
            <GarageServiceLogDialog
                mode={dialogMode}
                service={selectedItem}
                dialogOpen={dialogOpen}
                setDialogOpen={setDialogOpen}
                createService={createService}
                updateService={updateService}
                loading={loading}
            />
        </>
    );
}
