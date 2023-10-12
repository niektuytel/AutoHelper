import React, { useEffect, useState } from "react";
import {
    Button, Dialog, DialogActions, DialogContent,
    DialogTitle, TextField, useTheme, useMediaQuery, Select,
    MenuItem, FormControl, InputLabel,
} from "@mui/material";
import {Controller, useForm} from "react-hook-form";
import { useTranslation } from "react-i18next";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router";

// own imports
import { GarageEmployeeWorkExperienceItemDto, GarageServiceItemDto, UpdateGarageEmployeeCommand } from "../../../../app/web-api-client";
import { ROUTES } from "../../../../constants/routes";
import { getTitleForServiceType } from "../../defaultGarageService";
import { GetGarageClient } from "../../../../app/GarageClient";
import useGarageEmployees from "../useGarageEmployees";
import useConfirmationStep from "../../../../hooks/useConfirmationStep";
import useUserRole from "../../../../hooks/useUserRole";

interface IProps {
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    selectedExperience: GarageEmployeeWorkExperienceItemDto | undefined;
    editExperience: (data: GarageEmployeeWorkExperienceItemDto) => void;
    addExperience: (data: GarageEmployeeWorkExperienceItemDto) => void;
    services: GarageServiceItemDto[];
}

export default function ExperienceDialog({ dialogOpen, setDialogOpen, selectedExperience, addExperience, editExperience, services}: IProps) {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { userRole } = useUserRole();
    const { setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageClient(accessToken);
    const navigate = useNavigate();
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    useEffect(() => {
        if (selectedExperience) {
            setDialogMode('edit');
            setValue("serviceId", selectedExperience.serviceId);
            setValue("description", selectedExperience.description);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [selectedExperience, setValue]);

    // Handle adding the selected experience
    const handleAddExperience = () => {
        const serviceId = watch("serviceId");
        const description = watch("description");

        addExperience(
            new GarageEmployeeWorkExperienceItemDto({
                serviceId: serviceId,
                description: description
            })
        );
    }

    const handleEditExperience = () => {
        const serviceId = watch("serviceId");
        const description = watch("description");

        editExperience(
            new GarageEmployeeWorkExperienceItemDto({
                serviceId: serviceId,
                description: description
            })
        );
    }

    const onSubmit = (data: any) => {
        if (dialogMode === 'edit') {
            handleEditExperience();
        } else {
            handleAddExperience();
        }
    };
    
    return (
        <Dialog
            open={dialogOpen}
            onClose={() => setDialogOpen(false)}
            fullWidth
            maxWidth="sm"
            fullScreen={isMobile}
        >
            <DialogTitle>{dialogMode === 'create' ? t('experience_add_title') : t('experience_edit_title')}</DialogTitle>
            <form onSubmit={handleSubmit(onSubmit)}>
                <DialogContent dividers>
                    <Controller
                        name="serviceId"
                        control={control}
                        rules={{ required: t("Service is required!") }}
                        defaultValue=""
                        render={({ field }) => (
                            <FormControl fullWidth variant="outlined" error={Boolean(errors.serviceId)}>
                                <InputLabel htmlFor="select-title">{t("Select Service")}</InputLabel>
                                <Select
                                    {...field}
                                    label={t("Select Service")}
                                >
                                    {services.map((service, index) => (
                                        <MenuItem key={index} value={service.id} title={service.description}>
                                            {getTitleForServiceType(t, service.type!, service.description)}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        )}
                    />
                    <Controller
                        name="description"
                        control={control}
                        rules={{ }}
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
                    <Button type="submit" variant="contained" color="primary">
                        {t(dialogMode === 'create' ? "add" : "edit")}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
}
