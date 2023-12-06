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
    InputLabel,
    CircularProgress,
    FormControlLabel,
    Checkbox,
    Grid
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { UpdateGarageServiceCommand, GarageServiceType, VehicleType } from "../../../../app/web-api-client";
import useGarageServices from "../useGarageServices";
import { getAllGarageServiceTypes, getAllVehicleType } from "../../defaultGarageService";

// own imports



interface IProps {
    mode: 'create' | 'edit';
    service?: UpdateGarageServiceCommand;
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    createService: (data: any) => void;
    updateService: (data: any) => void;
    loading: boolean;
}

export default ({ dialogOpen, setDialogOpen, mode, service, createService, updateService, loading }: IProps) => {
    const { t } = useTranslation(['translations', 'serviceTypes']);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedService, setSelectedService] = useState<UpdateGarageServiceCommand | undefined>(service);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');

    const defaultAvailableServices = getAllGarageServiceTypes(t);
    const defaultVehicleTypes = getAllVehicleType(t);
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm({
        defaultValues: {
            id: '',
            type: GarageServiceType.Service,
            vehicleType: VehicleType.LightCar,
            title: '',
            description: '',
            expectedNextOdometerReadingIsRequired: false,
            expectedNextDateIsRequired: false
        }
    });

    useEffect(() => {
        if (mode === 'edit' && service) {
            setDialogMode('edit');
            setValue("id", service.id!);
            setValue("type", service.type!);
            setValue("vehicleType", service.vehicleType!);
            setValue("title", service.title ? service.title : t(`serviceTypes:${GarageServiceType[service.type!]}.Title`));
            setValue("description", service.description ? service.description : t(`serviceTypes:${GarageServiceType[service.type!]}.Description`));
            setValue("expectedNextOdometerReadingIsRequired", service.expectedNextOdometerReadingIsRequired ?? false);
            setValue("expectedNextDateIsRequired", service.expectedNextDateIsRequired ?? false);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [service, mode, setValue]);

    type ServiceProperty = 'type' | 'vehicleType' | 'title' | 'description';

    const handleTitleChange = (event: any) => {
        const service = defaultAvailableServices.find(item => item.type === event.target.value) as any;
        if (!service) return;

        const prevService = selectedService;
        setSelectedService(service);

        const item = watch();
        const propertiesToUpdate: ServiceProperty[] = ['type', 'vehicleType', 'title', 'description'];
        propertiesToUpdate.forEach(property => {
            if (!item[property] || (prevService && item[property] == prevService[property])) {
                setValue(property, service[property]);
            }
        });
    };


    const onSubmit = (data: any) => {
        if (mode === 'edit') {
            updateService(data);
        } else {
            createService(data);
        }
    };

    return (
        <>
            <Dialog
                open={dialogOpen}
                onClose={() => setDialogOpen(false)}
                fullWidth
                maxWidth="sm"
                fullScreen={isMobile}
            >
                <DialogTitle>
                    {t(dialogMode === 'create' ? "GarageServiceDialog.Title.Create" : "GarageServiceDialog.Title.Create")}
                    {isMobile && (
                        <IconButton onClick={() => setDialogOpen(false)} style={{ position: 'absolute', right: '8px', top: '8px' }}>
                            <CloseIcon />
                        </IconButton>
                    )}
                </DialogTitle>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <DialogContent dividers>
                        <Grid container spacing={2}>
                            <Grid item xs={12} sm={6}>
                                <Controller
                                    name="type"
                                    control={control}
                                    rules={{ required: t("GarageServiceDialog.Type.Required") }}
                                    render={({ field }) => (
                                        <FormControl fullWidth size='small'>
                                            <InputLabel htmlFor="select-title">{t("GarageServiceDialog.Type.Label")}</InputLabel>
                                            <Select
                                                {...field}
                                                onChange={(event) => {
                                                    field.onChange(event);
                                                    handleTitleChange(event);
                                                }}
                                                labelId="service-type-label"
                                                label={t("GarageServiceDialog.Type.Label")}
                                                size="small"
                                            >
                                                {defaultAvailableServices.map(item => (
                                                    <MenuItem key={item.type} value={item.type}>
                                                        {item.title}
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    )}
                                />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Controller
                                    name="vehicleType"
                                    control={control}
                                    rules={{ required: t("GarageServiceDialog.VehicleType.Required") }}
                                    render={({ field }) => (
                                        <FormControl fullWidth size='small'>
                                            <InputLabel htmlFor="select-title">{t("GarageServiceDialog.VehicleType.Label")}</InputLabel>
                                            <Select
                                                {...field}
                                                onChange={(event) => {
                                                    field.onChange(event);
                                                    handleTitleChange(event);
                                                }}
                                                labelId="service-type-label"
                                                label={t("GarageServiceDialog.VehicleType.Label")}
                                                size="small"
                                            >
                                                {defaultVehicleTypes.map(item => (
                                                    <MenuItem key={item.type} value={item.type}>
                                                        {item.title}
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    )}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <Controller
                                    name="title"
                                    control={control}
                                    rules={{ required: t("GarageServiceDialog.Title.Required") }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            label={t("GarageServiceDialog.Title.Label")}
                                            fullWidth
                                            size="small"
                                            variant="outlined"
                                            error={Boolean(errors.title)}
                                            helperText={errors.title ? t(errors.title.message as string) : ' '}
                                        />
                                    )}
                                />
                                <Controller
                                    name="description"
                                    control={control}
                                    rules={{ required: t("GarageServiceDialog.Description.Required") }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            label={t("GarageServiceDialog.Description.Label")}
                                            fullWidth
                                            size="small"
                                            multiline
                                            rows={3}
                                            variant="outlined"
                                            error={Boolean(errors.description)}
                                            helperText={errors.description ? t(errors.description.message as string) : ' '}
                                        />
                                    )}
                                />
                            </Grid>
                        </Grid>
                        <Controller
                            name="expectedNextOdometerReadingIsRequired"
                            control={control}
                            render={({ field: { onChange, onBlur, value, name, ref } }) => (
                                <FormControlLabel
                                    control={
                                        <Checkbox
                                            onChange={onChange}
                                            onBlur={onBlur}
                                            checked={value}
                                            inputRef={ref}
                                        />
                                    }
                                    label={t("GarageServiceDialog.ExpectedNextOdometerReading.Label")}
                                    sx={{ width: "100%" }}
                                />
                            )}
                        />
                        <Controller
                            name="expectedNextDateIsRequired"
                            control={control}
                            render={({ field: { onChange, onBlur, value, name, ref } }) => (
                                <FormControlLabel
                                    control={
                                        <Checkbox
                                            onChange={onChange}
                                            onBlur={onBlur}
                                            checked={value}
                                            inputRef={ref}
                                        />
                                    }
                                    label={t("GarageServiceDialog.ExpectedNextDate.Label")}
                                    sx={{ width: "100%" }}
                                />
                            )}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDialogOpen(false)}>
                            {t("Cancel")}
                        </Button>
                        {loading ?
                            <Button variant="contained" disabled style={{ color: 'white' }}>
                                <CircularProgress size={24} color="inherit" />
                            </Button>
                            :
                            <Button type="submit" variant="contained" color="primary">
                                {t(dialogMode === 'create' ? "Create" : "edit")}
                            </Button>
                        }
                    </DialogActions>
                </form>
            </Dialog>

        </>
    );
}
