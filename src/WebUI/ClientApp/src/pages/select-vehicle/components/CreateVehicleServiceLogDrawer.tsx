import React, { useEffect, useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Autocomplete, TextField, Select, MenuItem, InputLabel, FormControl,
    Grid, Button, Divider, Typography, Box, IconButton, Drawer, styled, SvgIcon
} from '@mui/material';
import { useDebounce } from 'use-debounce';
import CircularProgress from '@mui/material/CircularProgress';
import { useTranslation } from 'react-i18next';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { GarageClient, GarageLookupSimplefiedDto, GarageServiceType } from '../../../app/web-api-client';
import { enumToKeyValueArray } from '../../../app/utils';

function getVehicleServicesTypes() {
    const items = [
        GarageServiceType.Other,
        GarageServiceType.Inspection,
        GarageServiceType.SmallMaintenance,
        GarageServiceType.GreatMaintenance,
        GarageServiceType.AirConditioningMaintenance,
        GarageServiceType.SeasonalTireChange
    ];

    return enumToKeyValueArray(GarageServiceType)
        .filter(({ key, value }) => items.includes(key))
        .map(({ key, value }) => ({
            key: key,
            value: value,
        }));
}

const VisuallyHiddenInput = styled('input')`
  clip: rect(0 0 0 0);
  clip-path: inset(50%);
  height: 1px;
  overflow: hidden;
  position: absolute;
  bottom: 0;
  left: 0;
  white-space: nowrap;
  width: 1px;
`;

interface IProps {
    drawerOpen: boolean,
    toggleDrawer: (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => void
}

export default ({ drawerOpen, toggleDrawer }: IProps) => {
    const { control, handleSubmit, formState: { errors } } = useForm();
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const [selectedType, setSelectedType] = useState("");
    const [isMaintanance, setMaintanance] = useState(false);


    const onSubmit = (data: any) => {
        console.log(data);
    };

    const drawerWidth = window.innerWidth < 600 ? '100%' : '600px';
    function formatDate(date:Date) {
        let d = new Date(date),
            day = '' + d.getDate(),
            month = '' + (d.getMonth() + 1),
            year = d.getFullYear();

        if (day.length < 2)
            day = '0' + day;
        if (month.length < 2)
            month = '0' + month;

        return [day, month, year].join('/');
    }
    const today = formatDate(new Date());

    const SearchGarageComponent = () => {
        const [inputValue, setInputValue] = useState('');
        const [options, setOptions] = useState<GarageLookupSimplefiedDto[]>([]);
        const [debouncedInputValue] = useDebounce(inputValue, 500); // Debounce for 500ms
        const garageClient = new GarageClient(process.env.PUBLIC_URL);
        const [loading, setLoading] = useState(false);

        useEffect(() => {
            if (debouncedInputValue) {
                fetchOptions(debouncedInputValue);
            }
        }, [debouncedInputValue]);

        const fetchOptions = async (searchTerm: string) => {
            setLoading(true); // Start loading
            try {
                const response = await garageClient.searchLookupsByName(searchTerm, 5);
                setOptions(response);
                console.log(response);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
            setLoading(false); // End loading
        };


        return (
            <Autocomplete
                freeSolo
                options={options}
                getOptionLabel={(option) =>
                    typeof option === 'string' ? option : option.name || ''
                }
                onInputChange={(event, newInputValue) => {
                    setInputValue(newInputValue);
                }}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        label="Garage"
                        sx={{ mb: 2, mt: 1 }}
                        size='small'
                        InputProps={{
                            ...params.InputProps,
                            endAdornment: (
                                <React.Fragment>
                                    {loading ? <CircularProgress color="inherit" size={20} /> : null}
                                    {params.InputProps.endAdornment}
                                </React.Fragment>
                            ),
                        }}
                    />
                )}
            />
        );
    };

    return (
        <Drawer anchor="right" open={drawerOpen} onClose={toggleDrawer(false)} sx={{ width: drawerWidth }}>
            <form onSubmit={handleSubmit(onSubmit)} style={{ height: "100%" }}>
                <Box
                    sx={{ width: drawerWidth, display: 'flex', flexDirection: 'column', height: '100%' }}
                    role="presentation"
                >
                    <Box display="flex" justifyContent="space-between" alignItems="center" p={1}>
                        <Typography variant="h6" component="div">
                            {t("AddMaintenanceLog.Title")}
                        </Typography>
                        <IconButton onClick={toggleDrawer(false)}>
                            <CloseIcon />
                        </IconButton>
                    </Box>
                    <Divider />
                    <Box flexGrow={1} p={1}>
                        <Controller
                            name="performedByGarageId"
                            control={control}
                            rules={{ required: 'Garage is required' }}
                            render={({ field, fieldState: { error } }) => (
                                <SearchGarageComponent/>
                            )}
                        />
                        <Controller
                            name="type"
                            control={control}
                            defaultValue={"Other"}
                            render={({ field }) => (
                                <FormControl fullWidth sx={{ mb: 2 }} size='small'>
                                    <InputLabel id="service-type-label">
                                        {t("AddMaintenanceLog.ServiceType.Label")}
                                    </InputLabel>
                                    <Select
                                        {...field}
                                        labelId="service-type-label"
                                        label={t("AddMaintenanceLog.ServiceType.Label")}
                                        onChange={(e) => {
                                            field.onChange(e);
                                            setSelectedType(e.target.value); 
                                            setMaintanance(
                                                e.target.value === GarageServiceType[GarageServiceType.Inspection] ||
                                                e.target.value === GarageServiceType[GarageServiceType.SmallMaintenance] ||
                                                e.target.value === GarageServiceType[GarageServiceType.GreatMaintenance]
                                            )
                                        }}
                                    >
                                        {getVehicleServicesTypes().map(({ key, value }) => (
                                            <MenuItem key={key} value={value}>
                                                {t(`serviceTypes:${value}.Title`)}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            )}
                        />
                        <Controller
                            name="description"
                            control={control}
                            render={({ field }) => (
                                <TextField {...field} label={t("AddMaintenanceLog.ServiceDescription.Label")} multiline rows={4} fullWidth sx={{ mb: 1 }} />
                            )}
                        />
                        <Button component="label" variant="outlined" startIcon={<AttachFileIcon />} sx={{ color:"gray", borderColor:"gray" }}>
                            {t("Attachments")}
                            <VisuallyHiddenInput type="file" />
                        </Button>
                    </Box>
                    <Divider />
                    <Box p={1} component="footer" sx={{ mt: 1 }}>
                        <Grid container spacing={2} sx={{ mb: 2 }}>
                            <Grid item xs={isMaintanance ? 6 : 12}>
                                <Controller
                                    name="date"
                                    control={control}
                                    defaultValue={today}
                                    rules={{ required: 'Service date is required' }}
                                    render={({ field, fieldState: { error } }) => (
                                        <TextField
                                            {...field}
                                            type="date"
                                            label="Service Date"
                                            fullWidth
                                            size='small'
                                            error={!!error}
                                            helperText={error ? error.message : null}
                                        />
                                    )}
                                />
                            </Grid>
                            {isMaintanance &&
                                <Grid item xs={6}>
                                    <Controller
                                        name="expectedNextDate"
                                        control={control}
                                        defaultValue={today}
                                        render={({ field }) => <TextField {...field} type="date" label="Next MOT Date" fullWidth size='small' />}
                                    />
                                </Grid>
                            }
                        </Grid>
                        <Grid container spacing={2} sx={{ mb: 3}}>
                            <Grid item xs={isMaintanance ? 6 : 12}>
                                <Controller
                                    name="odometerReading"
                                    control={control}
                                    rules={{ required: 'Odometer reading is required' }}
                                    render={({ field, fieldState: { error } }) => (
                                        <TextField
                                            {...field}
                                            label="Odometer Reading"
                                            fullWidth
                                            size='small'
                                            error={!!error}
                                            helperText={error ? error.message : null}
                                        />
                                    )}
                                />
                            </Grid>
                            {isMaintanance &&
                                <Grid item xs={6}>
                                    <Controller
                                        name="nextOdometerReading"
                                        control={control}
                                        render={({ field }) => <TextField {...field} label="Next Service Odometer Reading" fullWidth size='small' />}
                                    />
                                </Grid>
                            }
                        </Grid>
                        <Button fullWidth variant="contained" color="primary" type="submit">
                            {t("Confirm")}
                        </Button>
                    </Box>
                </Box>
            </form>
        </Drawer>
    );
};
