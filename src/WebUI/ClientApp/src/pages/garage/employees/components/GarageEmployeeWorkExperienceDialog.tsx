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
    addService: (data: any) => void;
}

export default function ExperienceDialog({ dialogOpen, setDialogOpen, addService }: IProps) {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { userRole } = useUserRole();
    const { setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageClient(accessToken);
    const navigate = useNavigate();
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    // Fetch data for garage services
    const fetchGarageServicesData = async () => {
        try {
            return await garageClient.getServices();
        } catch (response: any) {
            if (response.status === 404) {
                setConfigurationIndex(1, userRole);
                navigate(ROUTES.GARAGE.SETTINGS);
                return;
            }
            throw response;
        }
    }

    const { data: garageServices, isLoading, isError } = useQuery(
        ['garageServices'], fetchGarageServicesData, {
        enabled: true,
        retry: 1,
        refetchOnWindowFocus: false,
        cacheTime: 30 * 60 * 1000,  // 30 minutes
        staleTime: 60 * 60 * 1000, // 1 hour
    }
    );

    // Handle adding the selected experience
    const handleAddExperience = () => {
        const serviceId = watch("serviceId");
        const description = watch("description");

        if (!serviceId || !description) return;

        const service = garageServices?.find(item => item.id === serviceId);
        addService({
            ...service,
            description: description
        });
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
                                {!isLoading && garageServices!.map((service, index) => (
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
                <Button onClick={handleAddExperience} variant="contained" color="primary">
                    {t("Create")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
