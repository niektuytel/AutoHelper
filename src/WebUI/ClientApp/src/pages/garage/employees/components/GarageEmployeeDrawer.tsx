import React, { CSSProperties, useEffect, useState } from "react";
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
    Box,
    Switch,
    FormControlLabel
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import AddIcon from '@mui/icons-material/Add';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { GarageEmployeeWorkExperienceItemDto, GarageEmployeeWorkSchemaItemDto, GarageServiceItemDto, UpdateGarageEmployeeCommand } from "../../../../app/web-api-client";
import useGarageEmployees from "../useGarageEmployees";
import { getTitleForServiceType } from "../../defaultGarageService";
import PersonOffIcon from '@mui/icons-material/PersonOff';
import PersonIcon from '@mui/icons-material/Person';
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
import { green, red } from "@mui/material/colors";

// own imports



interface IProps {
    mode: 'create' | 'edit';
    employee?: UpdateGarageEmployeeCommand;
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    createEmployee: (data: any) => void;
    updateEmployee: (data: any) => void;
    loading: boolean;
}

export default ({ dialogOpen, setDialogOpen, mode, employee, createEmployee, updateEmployee, loading }: IProps) => {
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

    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [timeUnit, setTimeUnit] = useState("minutes");

    const [selectedExperience, setSelectedExperience] = useState<GarageEmployeeWorkExperienceItemDto | undefined>(undefined);
    const [selectedExperiences, setSelectedExperiences] = useState<GarageEmployeeWorkExperienceItemDto[]>([]);
    const [experienceDialogOpen, setExperienceDialogOpen] = useState(false);

    const [selectedWorkSchema, setSelectedWorkSchema] = useState<Array<GarageEmployeeWorkSchemaItemDto>>([]);
    const [workSchemaDialogOpen, setWorkSchemaDialogOpen] = useState(false);

    //workSchema ?: GarageEmployeeWorkSchemaItem[];
    //workExperiences ?: GarageEmployeeWorkExperienceItemDto[];

    //const defaultAvailableEmployees = getDefaultCreateGarageEmployees(t);
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    useEffect(() => {
        if (mode === 'edit' && employee) {
            setDialogMode('edit');

            setValue("id", employee.id);
            setValue("isActive", employee.isActive);
            setValue("fullName", employee.contact?.fullName);
            setValue("email", employee.contact?.email);
            setValue("phoneNumber", employee.contact?.phoneNumber);
            setValue("WorkExperiences", employee.workExperiences);
            setValue("WorkSchema", employee.workSchema);

            setSelectedExperiences(employee.workExperiences as any[]);
            setSelectedWorkSchema(employee.workSchema || []);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [employee, mode, setValue]);

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

    const addExperience = (data: GarageEmployeeWorkExperienceItemDto) => {
        console.log(data);
        const updatedExperiences = [...selectedExperiences, data];
        setSelectedExperiences(updatedExperiences);
        setValue("WorkExperiences", updatedExperiences);
        setExperienceDialogOpen(false);
    };

    const editExperience = (data: GarageEmployeeWorkExperienceItemDto) => {
        if (!selectedExperience) return;
        console.log(data);

        const updatedExperiences = selectedExperiences.map((item) => {
            if (item.serviceId === selectedExperience.serviceId)
            {
                return data;
            }

            return item;
        });

        setSelectedExperience(undefined);
        setSelectedExperiences(updatedExperiences);
        setValue("WorkExperiences", updatedExperiences);
        setExperienceDialogOpen(false);
    };

    const setWorkSchema = (data: Array<GarageEmployeeWorkSchemaItemDto>) => {
        console.log(data);
        setSelectedWorkSchema(data);
        setValue("WorkSchema", data);
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
                                <FormControlLabel
                                    control={
                                        <Controller
                                            name="isActive"
                                            control={control}
                                            defaultValue={false}
                                            render={({ field }) => (
                                                <Switch
                                                    checked={field.value}
                                                    onChange={(e) => field.onChange(e.target.checked)}
                                                    color="default"
                                                    sx={{
                                                        '.MuiSwitch-thumb': {
                                                            backgroundColor: field.value ? green[500] : red[500], // Adjusting the thumb color
                                                        },
                                                        '.MuiSwitch-track': {
                                                            backgroundColor: field.value ? green[300] : red[300], // Adjusting the track color
                                                        }
                                                    }}
                                                />
                                            )}
                                        />
                                    }
                                    label={t(watch("isActive") ? 'Online' : 'Offline')}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <Controller
                                    name="fullName"
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
                                    name="email"
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
                                    name="phoneNumber"
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
                        {isLoading ? (
                            <CircularProgress size={24} />
                        ) : (
                            selectedExperiences.map((experience, index) => {
                                const service = garageServices!.find(item => item.id === experience.serviceId);
                                console.log(garageServices, experience);

                                if (!service)
                                {
                                    return <></>;
                                }

                                return <Box
                                    key={index}
                                    title={experience.description}
                                    sx={{
                                        marginBottom: '10px',
                                        border: '1px solid',
                                        padding: '10px',
                                        position: 'relative',
                                        borderRadius: "4px"
                                    }}
                                    onClick={() => {
                                        setSelectedExperience(experience);
                                        setExperienceDialogOpen(true);
                                    }}
                                >
                                    {getTitleForServiceType(t, service.type!, experience.description)}
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
                            })
                        )}
                        <Button
                            variant="outlined"
                            color="primary"
                            startIcon={<AddIcon />}
                            onClick={() => {
                                setSelectedExperience(undefined);
                                setExperienceDialogOpen(true);
                            }}
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
            {!isLoading &&
                <GarageEmployeeWorkExperienceDialog
                    dialogOpen={experienceDialogOpen}
                    setDialogOpen={setExperienceDialogOpen}
                    selectedExperience={selectedExperience}
                    editExperience={editExperience}
                    addExperience={addExperience}
                    services={garageServices!}
                />
            }
            <GarageEmployeeWorkSchemaDialog
                mode={dialogMode}
                dialogOpen={workSchemaDialogOpen}
                setDialogOpen={setWorkSchemaDialogOpen}
                workSchema={selectedWorkSchema}
                setWorkSchema={setWorkSchema}
            />
        </>
    );
}
