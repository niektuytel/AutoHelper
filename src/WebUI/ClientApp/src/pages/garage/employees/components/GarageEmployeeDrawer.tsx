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
    ListItemText,
    List,
    ListItem,
    Drawer,
    Grid,
    Divider,
    Box
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import AddIcon from '@mui/icons-material/Add';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { GarageEmployeeWorkExperienceItemDto, GarageServiceItemDto, UpdateGarageEmployeeCommand } from "../../../../app/web-api-client";
import useGarageEmployees from "../useGarageEmployees";
import { getTitleForServiceType } from "../../defaultGarageService";
import { useQuery, useQueryClient } from "react-query";
import { ROUTES } from "../../../../constants/routes";
import useConfirmationStep from "../../../../hooks/useConfirmationStep";
import { useAuth0 } from "@auth0/auth0-react";
import useUserRole from "../../../../hooks/useUserRole";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { GetGarageClient } from "../../../../app/GarageClient";
import GarageEmployeeWorkExperienceDialog from "./GarageEmployeeWorkExperienceDialog";
import GarageEmployeeWorkSchemaDialog from "./GarageEmployeeWorkSchemaDialog";

// own imports



interface IProps {
    mode: 'create' | 'edit';
    service?: UpdateGarageEmployeeCommand;
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    createEmployee: (data: any) => void;
    updateEmployee: (data: any) => void;
    loading: boolean;
}

export default ({ dialogOpen, setDialogOpen, mode, service, createEmployee, updateEmployee, loading }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { userRole } = useUserRole()
    const { configurationIndex, setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageClient(accessToken);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const [selectedEmployee, setSelectedEmployee] = useState<UpdateGarageEmployeeCommand | undefined>(service);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [timeUnit, setTimeUnit] = useState("minutes");

    const [selectedExperiences, setSelectedExperiences] = useState<any[]>([]);
    const [experienceDialogOpen, setExperienceDialogOpen] = useState(false);
    const [workSchemaDialogOpen, setWorkSchemaDialogOpen] = useState(false);

    //workSchema ?: GarageEmployeeWorkSchemaItem[];
    //workExperiences ?: GarageEmployeeWorkExperienceItemDto[];

    //const defaultAvailableEmployees = getDefaultCreateGarageEmployees(t);
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    useEffect(() => {
        if (mode === 'edit' && service) {
            setDialogMode('edit');
            setValue("id", service.id);
            //setValue("title", getTitleForEmployeeType(t, service.type!));
            //setValue("type", service.type);
            //setValue("description", service.description);
            //setValue("durationInMinutes", service.durationInMinutes);
            //setValue("price", service.price);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [service, mode, setValue]);

    type EmployeeProperty = 'type' | 'description' | 'durationInMinutes' | 'price';

    //const handleTitleChange = (event: any) => {
    //    const service = defaultAvailableEmployees.find(item => item.type === event.target.value) as UpdateGarageEmployeeCommand;
    //    if (!service) return;

    //    const prevEmployee = selectedEmployee;
    //    setSelectedEmployee(service);

    //    const item = watch();
    //    const propertiesToUpdate: EmployeeProperty[] = ['type', 'description', 'durationInMinutes', 'price'];
    //    propertiesToUpdate.forEach(property => {
    //        if (!item[property] || (prevEmployee && item[property] == prevEmployee[property])) {
    //            setValue(property, service[property]);
    //        }
    //    });
    //};

    const fetchGarageServicesData = async () => {
        try {
            const response = await garageClient.getServices();

            return response;
        } catch (response: any) {
            // redirect + enable garage register page
            if (response.status === 404) {
                setConfigurationIndex(1, userRole);
                navigate(ROUTES.GARAGE.SETTINGS);
                return;
            }

            throw response;
        }
    }

    const { data: garageServices, isLoading, isError } = useQuery(
        ['garageServices'],
        fetchGarageServicesData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const addExperience = (data: any) => {
        console.log(data)

        setExperienceDialogOpen(false);
        setSelectedExperiences([...selectedExperiences, data]);
    };

    const setWorkSchema = (data: any) => {
        console.log(data)

        setWorkSchemaDialogOpen(false);
    };

    const removeExperience = (index: number) => {
        const newExperiences = [...selectedExperiences];
        newExperiences.splice(index, 1);
        setSelectedExperiences(newExperiences);
    };


    const onSubmit = (data: any) => {
        if (mode === 'edit') {
            updateEmployee(data);
        } else {
            createEmployee(data);
        }
    };

    return (
        <>
            <Drawer anchor={'right'} open={dialogOpen} onClose={() => setDialogOpen(false)} sx={{zIndex:"1"}}>
                <DialogTitle>{t(dialogMode === 'create' ? 'Create Employee' : 'Edit Employee')}</DialogTitle>
                <DialogContent>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <Controller
                                    name="Contact.FullName"
                                    control={control}
                                    defaultValue=""
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            label="Name"
                                            size="small"
                                            fullWidth
                                            required
                                        />
                                    )}
                                />
                            </Grid>
                            <Grid item xs={6}>
                                <Controller
                                    name="Contact.Email"
                                    control={control}
                                    defaultValue=""
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            label="Email"
                                            size="small"
                                            fullWidth
                                            required
                                        />
                                    )}
                                />
                            </Grid>
                            <Grid item xs={6}>
                                <Controller
                                    name="Contact.PhoneNumber"
                                    control={control}
                                    defaultValue=""
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            label="Phone Number"
                                            size="small"
                                            fullWidth
                                            required
                                        />
                                    )}
                                />
                            </Grid>
                        </Grid>
                        <Divider sx={{ my: 2 }} />
                        {selectedExperiences.map((service, index) => (
                            <Box key={index} title={service.description} sx={{
                                marginBottom: '10px',
                                border: '1px solid',
                                padding: '10px',
                                position: 'relative',
                                borderRadius: "4px"
                            }}>
                                {getTitleForServiceType(t, service.type!, service.description)}
                                <IconButton
                                    size="small"
                                    sx={{
                                        position: 'absolute', top: '5px', right: '5px'
                                    }}
                                    onClick={() => removeExperience(index)}
                                >
                                    <CloseIcon />
                                </IconButton>
                            </Box>
                        ))}
                        <Button
                            variant="outlined"
                            color="primary"
                            startIcon={<AddIcon />}
                            onClick={() => setExperienceDialogOpen(true)}
                        >
                            {t("work experience")}
                        </Button>
                        <Divider sx={{ my: 2 }} />
                        <Button
                            variant="outlined"
                            color="primary"
                            startIcon={<AddIcon />}
                            onClick={() => setWorkSchemaDialogOpen(true)}
                        >
                            {t("work schema")}
                        </Button>
                        {/*<GarageEmployeeWorkSchema/>*/}
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
                                    {t(dialogMode === 'create' ? "Create" : "Update")}
                                </Button>
                            }
                        </DialogActions>
                    </form>
                </DialogContent>
            </Drawer>
            <GarageEmployeeWorkExperienceDialog
                dialogOpen={experienceDialogOpen}
                setDialogOpen={setExperienceDialogOpen}
                addService={addExperience}
            />
            <GarageEmployeeWorkSchemaDialog
                dialogOpen={workSchemaDialogOpen}
                setDialogOpen={setWorkSchemaDialogOpen}
                setWorkSchema={setWorkSchema}
            />
        </>
    );
}
