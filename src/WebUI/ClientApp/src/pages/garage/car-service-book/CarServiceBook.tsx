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
//import useGarageServices from "./useGarageServices";
import { useParams } from "react-router";
import { Controller, useForm } from "react-hook-form";

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    //const { t } = useTranslation();
    //const theme = useTheme();
    //const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    //const { garage_guid } = useParams();
    //const { loading, isError, garageServices } = useGarageServices(garage_guid);
    //const [selectedItem, setSelectedItem] = useState<any>(null);
    //const [confirmDeleteOpen, setConfirmDeleteOpen] = useState<boolean>(false);
    //const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
    //const [cartItems, setCartItems] = useState<any[]>([]);
    //const [drawerOpen, setDrawerOpen] = useState<boolean>(false);


    //const { control, handleSubmit, formState: { errors }, setError } = useForm();


    //// Sample data
    //const testData = [
    //    { id: 1, title: "Service 1", description: "This is service 1 description.", duration: "10 min" },
    //    // ... add more sample data
    //];

    //const onSubmit = (data: any) => {
    //    console.log(data);
    //    // Handle the form submission logic here...
    //};

    //const drawerWidth = isMobile ? '100vw' : '400px';
//<Box pt={4}>
//                <Typography variant="h4" gutterBottom display="flex" alignItems="center">
//                    {t("Services")}
//                    {loading ?
//                        <CircularProgress size={20} style={{ marginLeft: '10px' }} />
//                        :
//                        <Tooltip title={t("Services.Description")}>
//                            <IconButton size="small">
//                                <InfoOutlinedIcon fontSize="inherit" />
//                            </IconButton>
//                        </Tooltip>
//                    }
//                </Typography>
//            </Box>
//            <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
//                {isMobile ?
//                    <div>
//                        <IconButton onClick={() => setDrawerOpen(true)}>
//                            <AddIcon />
//                        </IconButton>
//                        <IconButton disabled={!selectedItem} onClick={() => setEditDialogOpen(true)}>
//                            <EditIcon />
//                        </IconButton>
//                        <IconButton disabled={!selectedItem} onClick={() => setConfirmDeleteOpen(true)}>
//                            <DeleteIcon />
//                        </IconButton>
//                    </div>
//                    :
//                    <ButtonGroup aria-label="Buttons used for create, edit and delete">
//                        <Button onClick={() => setDrawerOpen(true)}>
//                            <AddIcon />{t("Create")}
//                        </Button>
//                        <Button onClick={() => setEditDialogOpen(true)} disabled={!selectedItem}>
//                            <EditIcon />{t("Edit")}
//                        </Button>
//                        <Button onClick={() => setEditDialogOpen(true)} disabled={!selectedItem}>
//                            <DeleteIcon />{t("Delete")}
//                        </Button>
//                    </ButtonGroup>

//                }
//            </Box>
//            <Divider style={{ marginBottom: "20px" }} />
//            <Box>
//                {testData?.map((item) => (
//                    <Card key={item.id} style={{ marginBottom: "20px" }}>
//                        <CardHeader
//                            action={
//                                <IconButton onClick={() => setCartItems([...cartItems, item])}>
//                                    <AddIcon />
//                                </IconButton>
//                            }
//                            title={item.title}
//                        />
//                        <CardContent>
//                            <Typography variant="body1">{item.description}</Typography>
//                            <Typography variant="body2" color="textSecondary">Status: Offline</Typography>
//                        </CardContent>
//                        <CardActions>
//                            <Box display="flex" alignItems="center">
//                                <AccessTimeIcon color="action" />
//                                <Typography variant="body2" color="textSecondary" style={{ marginLeft: "8px" }}>
//                                    {item.duration}
//                                </Typography>
//                            </Box>
//                        </CardActions>
//                    </Card>
//                ))}
//            </Box>
//            <Drawer
//                elevation={0}
//                variant="temporary"
//                anchor="right"
//                open={drawerOpen}
//                onClose={() => setDrawerOpen(false)}
//                ModalProps={{
//                    keepMounted: true, // Better open performance on mobile.
//                    BackdropProps: {
//                        invisible: false
//                    }
//                }}
//            >
//                <Box
//                    sx={{
//                        width: drawerWidth,
//                        maxWidth: drawerWidth,
//                        display: 'flex',
//                        flexDirection: 'column',
//                        height: '100%',
//                    }}
//                    role="presentation"
//                >
//                    <Toolbar />
//                    {isMobile && (
//                        <IconButton onClick={() => setDrawerOpen(false)}>
//                            <CloseIcon />
//                        </IconButton>
//                    )}
//                    <Typography variant="h6" align="center" padding={2}>
//                        {t("Drawer Title")}
//                    </Typography>
//                    <Divider />
//                    <form onSubmit={handleSubmit(onSubmit)}>
//                        <Box p={2} display="flex" flexDirection="column" gap={3} flexGrow={1} overflow="auto">
//                            <Controller
//                                name="title"
//                                control={control}
//                                rules={{ required: t("Title is required!") }}
//                                defaultValue=""
//                                render={({ field }) => (
//                                    <TextField
//                                        {...field}
//                                        label={t("Title")}
//                                        fullWidth
//                                        size="small"
//                                        variant="outlined"
//                                        error={Boolean(errors.title)}
//                                        helperText={errors.title && t(errors.title.message as string)}
//                                    />
//                                )}
//                            />
//                            <Controller
//                                name="description"
//                                control={control}
//                                rules={{ required: t("Description is required!") }}
//                                defaultValue=""
//                                render={({ field }) => (
//                                    <TextField
//                                        {...field}
//                                        label={t("Description")}
//                                        fullWidth
//                                        size="small"
//                                        multiline
//                                        variant="outlined"
//                                        error={Boolean(errors.description)}
//                                        helperText={errors.description && t(errors.description.message as string)}
//                                    />
//                                )}
//                            />
//                            <Controller
//                                name="duration"
//                                control={control}
//                                rules={{ required: t("Duration is required!") }}
//                                defaultValue=""
//                                render={({ field }) => (
//                                    <TextField
//                                        {...field}
//                                        label={t("Duration")}
//                                        fullWidth
//                                        size="small"
//                                        type="number"
//                                        inputProps={{ min: 0 }}
//                                        variant="outlined"
//                                        error={Boolean(errors.duration)}
//                                        helperText={errors.duration && t(errors.duration.message as string)}
//                                    />
//                                )}
//                            />
//                            <Controller
//                                name="price"
//                                control={control}
//                                rules={{ required: t("Price is required!") }}
//                                defaultValue=""
//                                render={({ field }) => (
//                                    <TextField
//                                        {...field}
//                                        label={t("Price")}
//                                        fullWidth
//                                        size="small"
//                                        type="number"
//                                        inputProps={{ step: '0.01' }}
//                                        variant="outlined"
//                                        error={Boolean(errors.price)}
//                                        helperText={errors.price && t(errors.price.message as string)}
//                                    />
//                                )}
//                            />
//                        </Box>
//                        <Box
//                            p={2}
//                            borderTop={1}
//                            borderColor="divider"
//                            mt="auto"
//                        >
//                            <Box display="flex" justifyContent="space-between">
//                                <Button onClick={() => setDrawerOpen(false)}>
//                                    {t("Cancel")}
//                                </Button>
//                                <Button type="submit" variant="contained" color="primary">
//                                    {t("Create")}
//                                </Button>
//                            </Box>
//                        </Box>
//                    </form>
//                </Box>
//            </Drawer>

    return (
        <>
            
        </>
    );
}
