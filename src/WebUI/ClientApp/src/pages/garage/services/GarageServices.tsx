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
import { CreateGarageServiceCommand, GarageServiceType } from "../../../app/web-api-client";
import GarageServiceDialog from "./components/GarageServiceDialog";
import { getTitleForServiceType } from "./defaultGarageService";
import { COLORS } from "../../../constants/colors";
import OtherGarageServiceCard from "./components/GarageServiceCard";

// own imports


interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const navigate = useNavigate();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));


    const [otherServiceDescription, setOtherServiceDescription] = useState("");
    const [otherServiceDuration, setOtherServiceDuration] = useState(0);
    const [otherServicePrice, setOtherServicePrice] = useState(0);

    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [cartItems, setCartItems] = useState<any[]>([]);
    const [drawerOpen, setDialogOpen] = useState<boolean>(false);


    const [dialogMode, setDialogMode] = useState<"create" | "edit">("create");
    const [currentService, setCurrentService] = useState<CreateGarageServiceCommand | undefined>(undefined);


    const handleFormSubmit = (data: any) => {
        setDialogOpen(false);
        // Handle the form submission logic here...
    };

    const { loading, createService, updateService, isError, garageServices } = useGarageServices(handleFormSubmit);

    // Sample data
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
            <OtherGarageServiceCard addCartItem={(item) => setCartItems([...cartItems, item ])} />
        
            {garageServices?.map((item, index) => {
                const title = getTitleForServiceType(t, item.type!, item.description);
                return <Card
                    key={`service-card-${title}-${index}`}
                    style={{
                        marginBottom: "10px",
                        padding: "8px",
                        cursor: "pointer",
                        border: selectedItem === item ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
                    }}
                    title={item.description}
                    onClick={() => setSelectedItem(item)}
                >
                    <CardHeader
                        action={
                            <IconButton
                                onClick={(e) => {
                                    e.stopPropagation();
                                    setCartItems([...cartItems, item])
                                }}
                            >
                                <AddIcon />
                            </IconButton>
                        }
                        title={title}
                        titleTypographyProps={{ variant: "body1" }}
                        style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
                    />
                    <CardActions style={{ padding: "0", justifyContent: "space-between" }}>
                        <Box display="flex" alignItems="center">
                            <AccessTimeIcon color="action" fontSize="small" />
                            <Typography variant="caption" color="textSecondary" style={{ marginLeft: "8px" }}>
                                {item.durationInMinutes}
                            </Typography>
                        </Box>
                        <Box display="flex" alignItems="center" style={{ marginRight: "10px" }} >
                            <EuroIcon color="action" fontSize="small" style={{ marginRight: "5px" }} />
                            <Typography variant="body2" align="right">
                                {item.price}
                            </Typography>
                        </Box>
                    </CardActions>
                </Card>
                
            })}

            <GarageServiceDialog
                isOpen={drawerOpen}
                onClose={() => setDialogOpen(false)}
                createService={createService}
                updateService={updateService}
                loading={loading}
                mode={dialogMode}
                service={currentService}
            />

            

        </>
    );
}
