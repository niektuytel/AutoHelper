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
    Toolbar
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

    const { garage_guid } = useParams();
    const { loading, isError, garageServices } = useGarageServices(garage_guid);
    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState<boolean>(false);
    const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
    const [cartItems, setCartItems] = useState<any[]>([]);
    const [drawerOpen, setDrawerOpen] = useState<boolean>(false);

    const drawerWidth = isMobile ? '100vw' : '50vw';

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
                        <IconButton onClick={() => setDrawerOpen(true)}>
                            <AddIcon />
                        </IconButton>
                        <IconButton disabled={!selectedItem} onClick={() => setEditDialogOpen(true)}>
                            <EditIcon />
                        </IconButton>
                        <IconButton disabled={!selectedItem} onClick={() => setConfirmDeleteOpen(true)}>
                            <DeleteIcon />
                        </IconButton>
                    </div>
                    :
                    <ButtonGroup aria-label="Buttons used for create, edit and delete">
                        <Button onClick={() => setDrawerOpen(true)}>
                            <AddIcon />{t("Create")}
                        </Button>
                        <Button onClick={() => setEditDialogOpen(true)} disabled={!selectedItem}>
                            <EditIcon />{t("Edit")}
                        </Button>
                        <Button onClick={() => setEditDialogOpen(true)} disabled={!selectedItem}>
                            <DeleteIcon />{t("Delete")}
                        </Button>
                    </ButtonGroup>

                }
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <Box>
                {garageServices?.map((item) => (
                    <Card key={item.id} style={{ marginBottom: "20px" }}>
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
            <Drawer
                elevation={0}
                variant="temporary"
                anchor="right"
                open={drawerOpen}
                onClose={() => setDrawerOpen(false)}
                ModalProps={{
                    keepMounted: true, // Better open performance on mobile.
                    BackdropProps: {
                        invisible: false
                    }
                }}
            >
                <Box
                    sx={{
                        width: drawerWidth,
                        maxWidth: drawerWidth
                    }}
                    role="presentation"
                >
                    <Toolbar />
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
                </Box>
            </Drawer>

        </>
    );
}
