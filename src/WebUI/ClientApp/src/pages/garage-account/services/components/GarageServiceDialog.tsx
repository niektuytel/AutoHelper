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
    CircularProgress
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { UpdateGarageServiceCommand, GarageServiceType } from "../../../../app/web-api-client";
import useGarageServices from "../useGarageServices";
import { getDefaultCreateGarageServices, getTitleForServiceType } from "../../defaultGarageService";

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
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedService, setSelectedService] = useState<UpdateGarageServiceCommand | undefined>(service);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [timeUnit, setTimeUnit] = useState("minutes");


    const defaultAvailableServices = getDefaultCreateGarageServices(t);
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    useEffect(() => {
        if (mode === 'edit' && service) {
            setDialogMode('edit');
            setValue("id", service.id);
            setValue("title", getTitleForServiceType(t, service.type!));
            setValue("type", service.type);
            setValue("description", service.description);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [service, mode, setValue]);

    type ServiceProperty = 'type' | 'description';

    const handleTitleChange = (event: any) => {
        const service = defaultAvailableServices.find(item => item.type === event.target.value) as UpdateGarageServiceCommand;
        if (!service) return;

        const prevService = selectedService;
        setSelectedService(service);

        const item = watch();
        const propertiesToUpdate: ServiceProperty[] = ['type', 'description'];
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
                    {t(dialogMode === 'create' ? "service_add_title" : "service_edit_title")}
                    {isMobile && (
                        <IconButton onClick={() => setDialogOpen(false)} style={{ position: 'absolute', right: '8px', top: '8px' }}>
                            <CloseIcon />
                        </IconButton>
                    )}
                </DialogTitle>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <DialogContent dividers>
                        <Controller
                            name="type"
                            control={control}
                            rules={{ required: t("What type of service do you need?") }}
                            defaultValue=""
                            render={({ field }) => (
                                <FormControl fullWidth variant="outlined" error={Boolean(errors.type)}>
                                    <InputLabel htmlFor="select-title">{t("Service type")}</InputLabel>
                                    <Select
                                        {...field}
                                        onChange={(event) => {
                                            field.onChange(event);
                                            handleTitleChange(event);
                                        }}
                                        label={t("Service type")}
                                    >
                                        {defaultAvailableServices.map(item => (
                                            <MenuItem key={item.type} value={item.type}>
                                                {getTitleForServiceType(t, item.type!)}
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
