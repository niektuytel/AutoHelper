import React, { useEffect, useState } from "react";
import {
    Button,
    IconButton,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    TextField,
    useTheme,
    useMediaQuery,
    Select,
    InputAdornment,
    MenuItem,
    FormControl,
    InputLabel
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { CreateGarageServiceCommand } from "../../../../app/web-api-client";
import useGarageServices from "../useGarageServices";

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
    isOpen: boolean;
    onClose: () => void;
    onResponse: (data: any) => void;
    mode: 'create' | 'edit';
    service?: CreateGarageServiceCommand;
}

export default ({ isOpen, onClose, onResponse, mode, service }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedService, setSelectedService] = useState<CreateGarageServiceCommand | undefined>(service);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [drawerOpen, setDialogOpen] = useState<boolean>(false);
    const [timeUnit, setTimeUnit] = useState("minutes");

    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();
    const { loading, isError, createService, updateService } = useGarageServices();

    // Additional state variables

    useEffect(() => {
        if (mode === 'edit' && service) {
            setDialogMode('edit');
            setValue("title", service.title);
            setValue("description", service.description);
            setValue("duration", service.duration);
            setValue("price", service.price);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [service, mode, setValue]);

    type ServiceProperty = 'description' | 'duration' | 'price';

    const handleTitleChange = (event: any) => {
        const service = defaultAvailableServices.find(item => item.title === event.target.value) as CreateGarageServiceCommand;
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

        if (mode === 'edit') {
            updateService(data);
        } else {
            createService(data);
        }
    };

    return (
        <>
            <Dialog
                open={isOpen}
                onClose={onClose}
                fullWidth
                maxWidth="sm"
                fullScreen={isMobile}
            >
                <DialogTitle>
                    {t(dialogMode === 'create' ? "Add new garage service" : "Edit garage service")}
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
                                <FormControl fullWidth variant="outlined" error={Boolean(errors.title)}>
                                    <InputLabel htmlFor="select-title">{t("Title")}</InputLabel>
                                    <Select
                                        {...field}
                                        onChange={(event) => {
                                            field.onChange(event);
                                            handleTitleChange(event);
                                        }}
                                        label={t("Title")}
                                    >
                                        {defaultAvailableServices.map(service => (
                                            <MenuItem key={service.title} value={service.title}>
                                                {service.title}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
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
                            {t(dialogMode === 'create' ? "Create" : "Update")}
                        </Button>
                    </DialogActions>
                </form>
            </Dialog>

        </>
    );
}
