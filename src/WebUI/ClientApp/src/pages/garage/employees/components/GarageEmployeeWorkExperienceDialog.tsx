import React, { useEffect, useState } from "react";
import {
    Button, IconButton, Dialog, DialogActions, DialogContent,
    DialogTitle, TextField, useTheme, useMediaQuery, Select,
    InputAdornment, MenuItem, FormControl, InputLabel,
    CircularProgress, ListItemText, List, ListItem, Drawer,
    Grid, Divider, Box
} from "@mui/material";

import CloseIcon from '@mui/icons-material/Close';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import AddIcon from '@mui/icons-material/Add';

import {
    Controller, useForm
} from "react-hook-form";

import {
    GarageEmployeeWorkExperienceItemDto, GarageServiceItemDto,
    UpdateGarageEmployeeCommand
} from "../../../../app/web-api-client";

import { useTranslation } from "react-i18next";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router";
import { useQuery, useQueryClient } from "react-query";
import { useDispatch } from "react-redux";

import { ROUTES } from "../../../../constants/routes";
import { getTitleForServiceType } from "../../defaultGarageService";
import useGarageEmployees from "../useGarageEmployees";
import useConfirmationStep from "../../../../hooks/useConfirmationStep";
import useUserRole from "../../../../hooks/useUserRole";
import { GetGarageClient } from "../../../../app/GarageClient";

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

        if (!serviceId || !description) return;

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

        if (!serviceId || !description) return;

        editExperience(
            new GarageEmployeeWorkExperienceItemDto({
                serviceId: serviceId,
                description: description
            })
        );
    }
    
    return (
        <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
            <DialogTitle>{t('Add Experience')}</DialogTitle>
            <DialogContent>
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
                    defaultValue=""
                    render={({ field }) => (
                        <TextField
                            {...field}
                            label={t('Experience Description')}
                            fullWidth
                            margin="normal"
                            multiline
                            rows={4}
                            variant="outlined"
                        />
                    )}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setDialogOpen(false)}>
                    {t("Cancel")}
                </Button>
                <Button onClick={dialogMode == 'edit' ? handleEditExperience : handleAddExperience} variant="contained" color="primary">
                    {dialogMode == 'edit' ? t("Edit") : t("Create")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
