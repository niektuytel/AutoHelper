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
import { useParams } from "react-router";
import { Controller, useForm } from "react-hook-form";
import { CreateGarageServiceCommand } from "../../../app/web-api-client";

// own imports


// Sample data
const defaultAvailableServices: CreateGarageServiceCommand[] = [
    new CreateGarageServiceCommand({ id: "22dd50db-45cc-455f-a8c8-866e4edf1b16", title: "Service 1", description: "This is service 1 description.", duration: 25, price: 100.01 }),
    new CreateGarageServiceCommand({ id: "014e41a1-3d1b-45f5-ba6c-de6013298b79", title: "Service 2", description: "This is service 2 description.", duration: 24, price: 90.01 }),
    new CreateGarageServiceCommand({ id: "60249a72-7d45-4f05-b0b1-0ca1f4548dca", title: "Service 3", description: "This is service 3 description.", duration: 23, price: 80.01 }),
    new CreateGarageServiceCommand({ id: "87a2bb5b-5362-4ee1-a421-8aa4d680fe57", title: "Service 4", description: "This is service 4 description.", duration: 22, price: 70.01 }),
    new CreateGarageServiceCommand({ id: "45f67b94-8562-4240-af1a-5b7ceedcb0e3", title: "Service 5", description: "This is service 5 description.", duration: 21, price: 60.01 }),
];

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
    const [drawerOpen, setDialogOpen] = useState<boolean>(false);
    const [timeUnit, setTimeUnit] = useState("minutes");


    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();


    // Sample data
    const testData: CreateGarageServiceCommand[] = [
        new CreateGarageServiceCommand({ id: "22dd50db-45cc-455f-a8c8-866e4edf1b16", title: "Service 1", description: "This is service 1 description.", duration: 20, price: 100.01 }),
    ];

    const [selectedService, setSelectedService] = useState<CreateGarageServiceCommand | null>(null);
    type ServiceProperty = 'description' | 'duration' | 'price';

    const handleTitleChange = (event: any) => {
        const service = defaultAvailableServices.find(item => item.id === event.target.value) as CreateGarageServiceCommand;
        if (!service) return;

        const prevService = selectedService;
        setSelectedService(service);

        const propertiesToUpdate: ServiceProperty[] = ['description', 'duration', 'price'];
        const item = watch();

        propertiesToUpdate.forEach(property => {
            if (!item[property] || (prevService && item[property] == prevService[property])) {
                setValue(property, service[property]);
            }
        });
    };


    const onSubmit = (data: any) => {
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
                        <IconButton onClick={() => setDialogOpen(true)}>
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
                        <Button onClick={() => setDialogOpen(true)}>
                            <AddIcon />{t("Add")}
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
                {testData?.map((item) => (
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

            <Dialog
                open={drawerOpen}
                onClose={() => setDialogOpen(false)}
                fullWidth
                maxWidth="sm"
                fullScreen={isMobile}
            >
                <DialogTitle>
                    {t("Add new garage service")}
                    {isMobile && (
                        <IconButton onClick={() => setDialogOpen(false)} style={{ position: 'absolute', right: '8px', top: '8px' }}>
                            <CloseIcon />
                        </IconButton>
                    )}
                </DialogTitle>

                <form onSubmit={handleSubmit(onSubmit)}>
                    <DialogContent dividers>
                        <Controller
                            name="title"
                            control={control}
                            rules={{ required: t("Title is required!") }}
                            defaultValue=""
                            render={({ field }) => (
                                <Select
                                    {...field}
                                    fullWidth
                                    variant="outlined"
                                    onChange={(event) => {
                                        field.onChange(event);
                                        handleTitleChange(event);
                                    }}
                                    label={t("Title")}
                                >
                                    {defaultAvailableServices.map(service => (
                                        <MenuItem key={service.id} value={service.id}>
                                            {service.title}
                                        </MenuItem>
                                    ))}
                                </Select>
                            )}
                        />
                        <Controller
                            name="description"
                            control={control}
                            rules={{ required: t("Description is required!") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Description")}
                                    fullWidth
                                    size="small"
                                    multiline
                                    rows={3}
                                    variant="outlined"
                                    error={Boolean(errors.description)}
                                    helperText={errors.description ? t(errors.description.message as string) : ' '}
                                    margin="normal"
                                />
                            )}
                        />
                        <Controller
                            name="duration"
                            control={control}
                            rules={{ required: t("Duration is required!") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Duration")}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ min: 0 }}
                                    variant="outlined"
                                    error={Boolean(errors.duration)}
                                    helperText={errors.duration ? t(errors.duration.message as string) : ' '}
                                    margin="normal"
                                     
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                <AccessTimeIcon />
                                            </InputAdornment>
                                        ),
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <Select
                                                    value={timeUnit}
                                                    onChange={(e) => setTimeUnit(e.target.value)}
                                                    sx={{
                                                        minWidth: "100%",
                                                        fontSize: '0.8rem',
                                                        border: 'none',
                                                        boxShadow: 'none',
                                                        '&:focus': {
                                                            border: 'none',
                                                            boxShadow: 'none',
                                                            outline: 'none',   // Remove the outline when focused
                                                        },
                                                        '& .MuiOutlinedInput-notchedOutline': {   // Remove the outline for the outlined variant
                                                            border: 'none',
                                                        },
                                                        '&:hover .MuiOutlinedInput-notchedOutline': {   // Remove the outline when hovered
                                                            border: 'none',
                                                        },
                                                    }}
                                                    size="small"
                                                >
                                                    <MenuItem value="minutes">{t("minutes")}</MenuItem>
                                                    <MenuItem value="hours">{t("hours")}</MenuItem>
                                                    <MenuItem value="days">{t("days")}</MenuItem>
                                                </Select>
                                            </InputAdornment>
                                        ),
                                        style: { paddingRight: '0' } // Reducing the padding to give more space
                                    }}
                                />
                            )}
                        />

                        <Controller
                            name="price"
                            control={control}
                            rules={{ required: t("Price is required!") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Price")}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ step: '0.01' }}
                                    variant="outlined"
                                    error={Boolean(errors.price)}
                                    helperText={errors.price ? t(errors.price.message as string) : ' '}
                                    margin="none"
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                €
                                            </InputAdornment>
                                        ),
                                    }}
                                />
                            )}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDialogOpen(false)}>
                            {t("Cancel")}
                        </Button>
                        <Button type="submit" variant="contained" color="primary">
                            {t("Create")}
                        </Button>
                    </DialogActions>
                </form>
            </Dialog>

        </>
    );
}
