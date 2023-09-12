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
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import EuroIcon from '@mui/icons-material/Euro';
import useGarageServices from "./useGarageServices";
import { useNavigate, useParams } from "react-router";
import { GarageServiceItemDto , GarageServiceType } from "../../../app/web-api-client";
import GarageServiceDialog from "./components/GarageServiceDialog";
import { getTitleForServiceType } from "./defaultGarageService";
import { COLORS } from "../../../constants/colors";
import GarageServiceCardOther from "./components/GarageServiceCardOther";
import GarageServiceCard from "./components/GarageServiceCard";
import GarageServiceDeleteDialog from "./components/GarageServiceDeleteDialog";
import GarageServicesCartItemsBar from "./components/GarageServicesCartItemsBar";

// own imports


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const navigate = useNavigate();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [cartItems, setCartItems] = useState<GarageServiceItemDto[]>([]);
    const [dialogOpen, setDialogOpen] = useState<boolean>(false);
    const [dialogDeleteOpen, setDialogDeleteOpen] = useState(false);


    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [currentService, setCurrentService] = useState<GarageServiceItemDto  | undefined>(undefined);


    const handleFormSubmit = (data: any) => {
        if (dialogMode == "create" || dialogMode == "edit") {
            setDialogOpen(false);
        }

        if (selectedItem) {
            setDialogDeleteOpen(false);
        }

        setSelectedItem(undefined);
    };

    const { loading, createService, updateService, deleteService, isError, garageServices } = useGarageServices(handleFormSubmit);

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
                            <AddIcon />{t("Add")}
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
            <GarageServiceCardOther addCartItem={(item) => setCartItems([...cartItems, item])} />
            {garageServices?.map((item) => item &&
                <GarageServiceCard
                    key={`service-card-${item.id}`}
                    service={item}
                    selectedItem={selectedItem}
                    setSelectedItem={setSelectedItem}
                    addCartItem={(item) => setCartItems([...cartItems, item])}
                />
            )}
            <GarageServiceDeleteDialog
                service={selectedItem}
                confirmDeleteOpen={dialogDeleteOpen}
                setConfirmDeleteOpen={setDialogDeleteOpen}
                deleteService={deleteService}
                loading={loading}
            />
            <GarageServiceDialog
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
