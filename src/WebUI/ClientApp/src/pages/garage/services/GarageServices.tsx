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
    Drawer
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
import { useParams } from "react-router";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    // Sample data
    const testData = [
        { id: 1, title: "Service 1", description: "This is service 1 description.", duration: "10 min" },
        // ... add more sample data
    ];

    const { garage_guid } = useParams();
    const { loading, isError, garageServices } = useGarageServices(garage_guid);
    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState<boolean>(false);
    const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
    const [cartItems, setCartItems] = useState<any[]>([]);
    const [drawerOpen, setDrawerOpen] = useState<boolean>(false);

    return (
        <>
            <Box pt={4}>
                <Typography variant="h4" gutterBottom display="flex" alignItems="center">
                    {t("Services")}
                    {loading && <CircularProgress size={20} style={{ marginLeft: '10px' }} />}
                    <Tooltip title={t("Services.Description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
            </Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <div>
                    <IconButton onClick={() => setDrawerOpen(true)}>
                        <AddIcon />
                        {!isMobile && t("Create")}
                    </IconButton>
                    <IconButton disabled={!selectedItem} onClick={() => setEditDialogOpen(true)}>
                        <EditIcon />
                        {!isMobile && t("Edit")}
                    </IconButton>
                    <IconButton disabled={!selectedItem} onClick={() => setConfirmDeleteOpen(true)}>
                        <DeleteIcon />
                        {!isMobile && t("Delete")}
                    </IconButton>
                </div>
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <Box>
                {testData.map((item) => (
                    <Card key={item.id} style={{ marginBottom: "20px" }}>
                        <CardHeader
                            action={
                                <IconButton onClick={() => setCartItems([...cartItems, item])}>
                                    <ShoppingCartIcon />
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

            <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
                {isMobile && (
                    <IconButton onClick={() => setDrawerOpen(false)}>
                        <CloseIcon />
                    </IconButton>
                )}
                <Box p={3} display="flex" flexDirection="column" gap={2}>
                    <TextField label={t("Title")} fullWidth />
                    <TextField label={t("Description")} fullWidth multiline />
                    <TextField label={t("Duration")} type="number" inputProps={{ min: 0 }} fullWidth />
                    <TextField label={t("Price")} type="number" fullWidth inputProps={{ step: '0.01' }} />
                    <Box mt={2} display="flex" justifyContent="space-between">
                        <Button onClick={() => setDrawerOpen(false)}>
                            {t("Cancel")}
                        </Button>
                        <Button variant="contained" color="primary">
                            {t("Create")}
                        </Button>
                    </Box>
                </Box>
            </Drawer>
        </>
    );
}
