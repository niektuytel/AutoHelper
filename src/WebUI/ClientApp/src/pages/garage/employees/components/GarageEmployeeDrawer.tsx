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
    Drawer
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
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

    const [companyServices, setCompanyServices] = useState<GarageServiceItemDto[]>([]);
    const [selectedExperiences, setSelectedExperiences] = useState<GarageEmployeeWorkExperienceItemDto[]>([]);
    const [experienceDialogOpen, setExperienceDialogOpen] = useState(false);

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


    const onSubmit = (data: any) => {
        if (mode === 'edit') {
            updateEmployee(data);
        } else {
            createEmployee(data);
        }
    };

    return (
        <>
            <Drawer anchor={'right'} open={dialogOpen} onClose={() => setDialogOpen(false)}>
                <DialogTitle>{t(dialogMode === 'create' ? 'Create Employee' : 'Edit Employee')}</DialogTitle>
                <DialogContent>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <List>
                            <ListItem>
                                <ListItemText>
                                    <Controller
                                        name="Contact.FullName"
                                        control={control}
                                        defaultValue=""
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                label="Full Name"
                                                fullWidth
                                                required
                                            />
                                        )}
                                    />
                                </ListItemText>
                            </ListItem>
                            <ListItem>
                                <ListItemText>
                                    <Controller
                                        name="Contact.PhoneNumber"
                                        control={control}
                                        defaultValue=""
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                label="Phone Number"
                                                fullWidth
                                            />
                                        )}
                                    />
                                </ListItemText>
                            </ListItem>
                            <ListItem>
                                <ListItemText>
                                    <Controller
                                        name="Contact.Email"
                                        control={control}
                                        defaultValue=""
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                label="Email"
                                                fullWidth
                                                type="email"
                                            />
                                        )}
                                    />
                                </ListItemText>
                            </ListItem>
                            <ListItem>
                                <ListItemText>
                                    {selectedExperiences.map((experience, index) => (
                                        <div key={index} style={{ marginBottom: '10px' }}>
                                            {experience.description}
                                            {/* Add any other fields you'd like to display */}
                                        </div>
                                    ))}
                                    <Button
                                        variant="contained"
                                        color="primary"
                                        onClick={() => setExperienceDialogOpen(true)}
                                    >
                                        {t('Add Experience')}
                                    </Button>
                                </ListItemText>
                            </ListItem>


                            {/* Placeholder for WorkSchema. You'd probably use a dynamic form or list input here */}
                            <ListItem>
                                <ListItemText>
                                    {/* ... WorkSchema Controls ... */}
                                    <span>Work Schedule will go here.</span>
                                </ListItemText>
                            </ListItem>

                            {/* Placeholder for WorkExperiences. Again, you'd likely use a dynamic form or list input */}

                        </List>
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
            <Dialog open={experienceDialogOpen} onClose={() => setExperienceDialogOpen(false)}>
                <DialogTitle>{t('Add Experience')}</DialogTitle>
                <DialogContent>
                    <FormControl fullWidth>
                        <InputLabel>{t('Select Experience')}</InputLabel>
                        <Controller
                            name="experience"
                            control={control}
                            defaultValue=""
                            render={({ field }) => (
                                <Select {...field}>
                                    {!isLoading && garageServices!.map((service, index) => (
                                        <MenuItem key={index} value={service.id} title={service.description}>
                                            {getTitleForServiceType(t, service.type!, service.description)}
                                        </MenuItem>
                                    ))}
                                </Select>
                            )}
                        />
                    </FormControl>
                    <Controller
                        name="experienceDescription"
                        control={control}
                        defaultValue=""
                        render={({ field }) => (
                            <TextField
                                {...field}
                                label={t('Experience Description')}
                                fullWidth
                                margin="normal"
                            />
                        )}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setExperienceDialogOpen(false)}>
                        {t("Cancel")}
                    </Button>
                    <Button 
                        onClick={() => {
                            // Logic to add selected experience to the `workExperiences` array
                            const experienceValue = watch("experience");
                            const experienceDescription = watch("experienceDescription");
                            setSelectedExperiences([...selectedExperiences,
                                new GarageEmployeeWorkExperienceItemDto({
                                    serviceId: experienceValue,
                                    description: experienceDescription
                                })
                            ]);
                            setExperienceDialogOpen(false);
                        }}
                        variant="contained" 
                        color="primary"
                    >
                        {t("Create")}
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}
