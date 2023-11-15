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
import { GarageClient, GarageLookupSimplefiedDto, GarageServiceType, VehicleClient } from '../../../app/web-api-client';
import { enumToKeyValueArray } from '../../../app/utils';

const convertFileToByteArray = async (file: File) => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onload = () => {
            const arrayBuffer = reader.result as ArrayBuffer;
            const byteArray = new Uint8Array(arrayBuffer);
            resolve(byteArray);
        };

        reader.onerror = (error) => {
            reject(error);
        };

        if (file) {
            reader.readAsArrayBuffer(file);
        } else {
            reject(new Error("No file provided"));
        }
    });
};

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
    licensePlate: string,
    drawerOpen: boolean,
    toggleDrawer: (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => void
}

export default ({ licensePlate, drawerOpen, toggleDrawer }: IProps) => {
    const { control, handleSubmit, formState: { errors } } = useForm();
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const [selectedType, setSelectedType] = useState("");
    const [isMaintanance, setMaintanance] = useState(false);

    const [file, setFile] = useState<File | null>(null);
    const [fileName, setFileName] = useState<string | null>(null);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files ? event.target.files[0] : null;
        setFile(file);
        setFileName(file ? file.name : null);
    };

    const onSubmit = (data: any) => {
        console.log(data);

        // Send command data and file to your backend endpoint
        const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
        const formData = new FormData();

        // Append the command data as JSON
        formData.append('ServiceLogCommand', JSON.stringify({
            vehicleLicensePlate: licensePlate,
            performedByGarageName: data.performedByGarageName,
            type: data.type,
            description: data.description,
            date: data.date,
            expectedNextDate: data.expectedNextDate,
            odometerReading: data.odometerReading,
            expectedNextOdometerReading: data.expectedNextOdometerReading,
        }));

        // Append the file
        if (file) {
            formData.append('AttachmentFile', file);
        }

        // Example values for the service log command
        let vehicleLicensePlate = "ABC123";
        let performedByGarageName = "Garage XYZ";
        let type = GarageServiceType.MOTServiceHeavyVehicle; // Example enum value
        let description = "Service Description";
        let date = new Date();
        let expectedNextDate = new Date();
        let odometerReading = 12345;
        let expectedNextOdometerReading = 13000;

        // Example file attachment
        let attachmentFile = {
            data: file, // Your file object
            fileName: file?.name || ''
        };

        // Create the service log
        const response = vehicleClient.createServiceLog(
            vehicleLicensePlate,
            performedByGarageName,
            type,
            description,
            date,
            expectedNextDate,
            odometerReading,
            expectedNextOdometerReading,
            file?.name || '',
            null, // Assuming FileData is handled separately
            attachmentFile
        ).then(response => {
            console.log(response);
        })

        //const response = await vehicleClient.createServiceLog(,

        //if (file) {
        //    const array = await convertFileToByteArray(file);
        //    response = await vehicleClient.createServiceLog(
        //        licensePlate,
        //        ,
        //        ,
        //        data.description,
        //        data.date,
        //        data.expectedNextDate,
        //        data.odometerReading,
        //        data.expectedNextOdometerReading,
        //        file.name,
        //        array as any,
        //        file.type,
        //        file.name,
        //        null,
        //        file.size,
        //        file.name,
        //        file.name

        //        //vehicleLicensePlate: string | null | undefined,
        //        //performedByGarageName: string | null | undefined, 
        //        //type: GarageServiceType | undefined, 
        //        //description: string | null | undefined, 
        //        //date: Date | undefined, 
        //        //expectedNextDate: Date | null | undefined, 
        //        //odometerReading: number | undefined, 
        //        //expectedNextOdometerReading: number | null | undefined, 
        //        //attachment_FileName: string | null | undefined, 
        //        //attachment_FileData: string | null | undefined, 
        //        //contentType: string | null | undefined, 
        //        //contentDisposition: string | null | undefined, 
        //        //headers: IHeaderDictionary | null | undefined, 
        //        //length: number | undefined, 
        //        //name: string | null | undefined, 
        //        //fileName: string | null | undefined
        //    );
        //} else {
        //    // Handle the case where there is no file to upload
        //}
        
        //console.log(response);
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
        const [garageName, setGarageName] = useState('');
        const [options, setOptions] = useState<GarageLookupSimplefiedDto[]>([]);
        const [debouncedInputValue] = useDebounce(garageName, 500); // Debounce for 500ms
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
                    setGarageName(newInputValue);
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

    // TODO: Add validation for file size and type (only images) 
    // TODO: We need more information about the user (name, phone, email)
    console.log(errors);

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
                            {/*rules={{ required: 'Garage is required' }}*/}
                        <Controller
                            name="performedByGarageName"
                            control={control}
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
                        <div>
                            <Button component="label" variant="outlined" startIcon={<AttachFileIcon />} sx={{ color: "gray", borderColor: "gray" }}>
                                {t("Attachments")}
                                <input
                                    type="file"
                                    hidden
                                    onChange={handleFileChange}
                                />
                                {/*<VisuallyHiddenInput type="file" />*/}
                            </Button>
                            {fileName && <div>{fileName}</div>}
                        </div>
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
