import React, { useState } from "react";
import {
    Box,
    Button,
    Container,
    Divider,
    IconButton,
    Tooltip,
    Typography,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
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
    Toolbar,
    Select,
    InputAdornment,
    MenuItem
} from "@mui/material";
import { useTranslation } from "react-i18next";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CloseIcon from '@mui/icons-material/Close';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';
import useGarageServices from "./useGarageServices";
import { useNavigate, useParams } from "react-router";
import { Controller, useForm } from "react-hook-form";
import { CreateGarageServiceCommand, GarageServiceItem, UpdateGarageServiceCommand } from "../../../app/web-api-client";
import GarageServiceDialog from "./components/GarageServiceDialog";
import { ROLES } from "../../../constants/roles";
import { ROUTES } from "../../../constants/routes";

// own imports


// Sample data
const defaultAvailableServices: CreateGarageServiceCommand[] = [
    new CreateGarageServiceCommand({ title: "Service 1", description: "This is service 1 description.", duration: 25, price: 100.01 }),
    new CreateGarageServiceCommand({ title: "Service 2", description: "This is service 2 description.", duration: 24, price: 90.01 }),
    new CreateGarageServiceCommand({ title: "Service 3", description: "This is service 3 description.", duration: 23, price: 80.01 }),
    new CreateGarageServiceCommand({ title: "Service 4", description: "This is service 4 description.", duration: 22, price: 70.01 }),
    new CreateGarageServiceCommand({ title: "Service 5", description: "This is service 5 description.", duration: 21, price: 60.01 }),
];

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const navigate = useNavigate();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const { loading, isError, garageServices } = useGarageServices();
    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [cartItems, setCartItems] = useState<any[]>([]);
    const [drawerOpen, setDialogOpen] = useState<boolean>(false);

    // Sample data
    const testData: GarageServiceItem[] = [
        new CreateGarageServiceCommand({ title: "Service 1", description: "This is service 1 description.", duration: 20, price: 100.01 }),
        ...(garageServices ? garageServices : [])
    ];

    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [currentService, setCurrentService] = useState<CreateGarageServiceCommand | undefined>(undefined);

    const handleAddClick = () => {
        setCurrentService(undefined);
        setDialogMode("create");
        setDialogOpen(true);
    }

    const handleEditClick = () => {
        if (selectedItem) {
            setCurrentService(selectedItem);
            setDialogMode("edit");
            setDialogOpen(true);
        }
    }

    const handleDeleteClick = () => {
        if (selectedItem) {
            //setDialogMode("delete");
            //setCurrentService(selectedItem); // Assuming `selectedItem` is the service you want to delete.
            //setConfirmDeleteOpen(true);
        }
    }

    //const handleTitleChange = (event: any) => {
    //    const service = defaultAvailableServices.find(item => item.id === event.target.value) as CreateGarageServiceCommand;
    //    if (!service) return;

    //    const prevService = selectedService;
    //    setSelectedService(service);

    //    const propertiesToUpdate: ServiceProperty[] = ['description', 'duration', 'price'];
    //    const item = watch();

    //    propertiesToUpdate.forEach(property => {
    //        if (!item[property] || (prevService && item[property] == prevService[property])) {
    //            setValue(property, service[property]);
    //        }
    //    });
    //};


    //const onSubmit = (data: any) => {
    //    console.log(data);
    //    // Handle the form submission logic here...
    //};
    const handleFormSubmit = (data: any) => {
        console.log(data);
        // Handle the form submission logic here...
    };

    const drawerWidth = isMobile ? '100vw' : '400px';

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
            <Box>
                {testData?.map((item) => (
                    <Card key={item.title} style={{ marginBottom: "20px" }}>
                        <CardHeader
                            action={
                                <IconButton onClick={() => setCartItems([...cartItems, item])}>
                                    <AddIcon />
                                </IconButton>
                            }
                            title={item.title}
                        />
                        <CardContent>
                            <Typography variant="body1">{item.description}</Typography>
                            <Typography variant="body2" color="textSecondary">Status: Offline</Typography>
                        </CardContent>
                        <CardActions>
                            <Box display="flex" alignItems="center">
                                <AccessTimeIcon color="action" />
                                <Typography variant="body2" color="textSecondary" style={{ marginLeft: "8px" }}>
                                    {item.duration}
                                </Typography>
                            </Box>
                        </CardActions>
                    </Card>
                ))}
            </Box>

            <GarageServiceDialog
                isOpen={drawerOpen}
                onClose={() => setDialogOpen(false)}
                onResponse={handleFormSubmit}
                mode={dialogMode}
                service={currentService}
            />

            

        </>
    );
}
