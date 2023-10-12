import React, { CSSProperties, useEffect, useState } from "react";
import {
    Button,
    IconButton,
    DialogActions,
    DialogContent,
    DialogTitle,
    TextField,
    useTheme,
    useMediaQuery,
    CircularProgress,
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
import { useQuery, useQueryClient } from "react-query";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router";

// own imports
import { GarageEmployeeWorkExperienceItemDto, GarageEmployeeWorkSchemaItemDto, GarageServiceItemDto, UpdateGarageEmployeeCommand } from "../../../../app/web-api-client";
import { getTitleForServiceType } from "../../defaultGarageService";
import { ROUTES } from "../../../../constants/routes";
import useConfirmationStep from "../../../../hooks/useConfirmationStep";
import useUserRole from "../../../../hooks/useUserRole";
import { GetGarageClient } from "../../../../app/GarageClient";
import GarageEmployeeWorkExperienceDialog from "./GarageEmployeeWorkExperienceDialog";
import GarageEmployeeWorkSchemaDialog from "./GarageEmployeeWorkSchemaDialog";

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
    const { userRole } = useUserRole()
    const { configurationIndex, setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageClient(accessToken);
    const navigate = useNavigate();

    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [selectedExperience, setSelectedExperience] = useState<GarageEmployeeWorkExperienceItemDto | undefined>(undefined);
    const [selectedExperiences, setSelectedExperiences] = useState<GarageEmployeeWorkExperienceItemDto[]>([]);
    const [experienceDialogOpen, setExperienceDialogOpen] = useState(false);

    const [selectedWorkSchema, setSelectedWorkSchema] = useState<Array<GarageEmployeeWorkSchemaItemDto>>([]);
    const [workSchemaDialogOpen, setWorkSchemaDialogOpen] = useState(false);

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
        const updatedExperiences = [...selectedExperiences, data];
        setSelectedExperiences(updatedExperiences);
        setValue("WorkExperiences", updatedExperiences);
        setExperienceDialogOpen(false);
    };

    const editExperience = (data: GarageEmployeeWorkExperienceItemDto) => {
        if (!selectedExperience) return;

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
        setSelectedWorkSchema(data);
        setValue("WorkSchema", data);
        setWorkSchemaDialogOpen(false);
    };

    const removeExperience = (experience: GarageEmployeeWorkExperienceItemDto) => {
        const newExperiences = selectedExperiences.filter((item) => item.serviceId !== experience.serviceId);

        setSelectedExperiences(newExperiences);
        setValue("WorkExperiences", newExperiences);
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
                <DialogTitle>{t(dialogMode === 'create' ? 'employee_add_title' : 'employee_edit_title')}</DialogTitle>
                <DialogContent>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <Box display="flex" flexDirection="column" height="calc(100vh - 84px)">
                            <Grid container spacing={2}>
                                <Grid item xs={12}></Grid>
                                <Grid item xs={12}>
                                    <Controller
                                        name="fullName"
                                        control={control}
                                        defaultValue=""
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                label={t("Name")}
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
                                                label={t("Telephone number")}
                                                size="small"
                                                fullWidth
                                                required
                                            />
                                        )}
                                    />
                                </Grid>
                                <Grid item xs={12}>
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        color="primary"
                                        startIcon={<AccessTimeIcon />}
                                        onClick={(e) => {
                                            e.preventDefault();
                                            setWorkSchemaDialogOpen(true);
                                        }}
                                    >
                                        {t("work schema")}
                                    </Button>
                                </Grid>
                            </Grid>
                            <Box flexGrow={1}>
                                <Divider sx={{ my: 2 }} />
                                {isLoading ? (
                                    <CircularProgress size={24} />
                                ) : (
                                    selectedExperiences.map((experience, index) => {
                                        const service = garageServices!.find(item => item.id === experience.serviceId)!;
                                        return (
                                            <Box
                                                key={index}
                                                title={experience.description}
                                                sx={{
                                                    marginBottom: '10px',
                                                    border: '1px solid',
                                                    padding: '10px',
                                                    position: 'relative',
                                                    borderRadius: "4px",
                                                    transition: 'background-color 0.3s', // smooth transition for the hover effect
                                                    cursor: 'pointer', // change the cursor to a hand when hovering
                                                    '&:hover': {
                                                        backgroundColor: 'rgba(0, 0, 0, 0.05)', // slightly grey background on hover
                                                    },
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
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        removeExperience(experience);
                                                    }}
                                                >
                                                    <CloseIcon />
                                                </IconButton>
                                            </Box>
                                        );
                                    })
                                )}
                                <Button
                                    fullWidth
                                    variant="outlined"
                                    color="primary"
                                    startIcon={<AddIcon />}
                                    sx={{ marginRight: theme.spacing(2) }}
                                    onClick={() => {
                                        setSelectedExperience(undefined);
                                        setExperienceDialogOpen(true);
                                    }}
                                >
                                    {t("work experience")}
                                </Button>
                            </Box>
                            <Divider sx={{ my: 2 }} />
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
                        </Box>
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
