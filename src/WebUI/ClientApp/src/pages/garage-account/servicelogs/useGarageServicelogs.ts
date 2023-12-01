import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { BadRequestResponse, CreateGarageServiceCommand, GarageClient, UpdateGarageServiceCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { ROUTES } from "../../../constants/routes";
import { useAuth0 } from "@auth0/auth0-react";
import { GetGarageAccountClient } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useConfirmationStep from "../../../hooks/useConfirmationStep";

function useGarageServiceLogs(onResponse: (data: any) => void) {
    const { userRole } = useUserRole()
    const { configurationIndex, setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageAccountClient(accessToken);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageServiceLogsData = async () => {
        try {
            const response = await garageClient.getServiceLogs(null);

            return response;
        } catch (response: any) {
            // redirect + enable garage register page
            if (response.status === 404) {
                setConfigurationIndex(1, userRole);
                navigate(ROUTES.GARAGE_ACCOUNT.SETTINGS);
                return;
            }

            throw response;
        }
    }

    const { data: garageServiceLogs, isLoading, isError } = useQuery(
        ['garageServiceLogs'],
        fetchGarageServiceLogsData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    //const createMutation = useMutation(garageClient.createServiceLog.bind(garageClient),, {
    //    onSuccess: (response) => {
    //        // Enable garage colleagues page
    //        setConfigurationIndex(3, userRole)
    //        dispatch(showOnSuccess("Garage service log is been created!"));

    //        // Update the garageSettings in the cache after creating
    //        queryClient.setQueryData(['garageServiceLogs'], [...garageServiceLogs!, response]);
    //        onResponse(response);
    //    },
    //    onError: (response) => {
    //        console.error(response)
    //        //guardHttpResponse(response, setError, t, dispatch);
    //    }
    //});

    const updateMutation = useMutation(garageClient.updateService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service log is been updated!"));

            // Update the garageSettings in the cache after updating
            const updatedGarageServices = garageServiceLogs?.map((service) => {
                if (service.id === response.id) {
                    return response;
                } else {
                    return service;
                }
            });

            queryClient.setQueryData(['garageServiceLogs'], updatedGarageServices);
            onResponse(response);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const deleteMutation = useMutation(garageClient.deleteService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service log is been deleted!"));

            // Delete the garageSettings in the cache after updating
            const updatedGarageServices = garageServiceLogs?.filter((service) => service.id !== response.id);

            console.log(updatedGarageServices);
            queryClient.setQueryData(['garageServiceLogs'], updatedGarageServices);
            onResponse(response);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const createServiceLog = (data: any, file: any) => {
        //var command = new CreateVehicleServiceAsGarageLogDto();
        //command.serviceLogCommand = new CreateVehicleServiceLogAsGarageCommand()
        //{
        //    vehicleLicensePlate: data.licensePlate,
        //    type: data.type,
        //    description: data.description,
        //    date: data.date.toISOString(),
        //    expectedNextDate: data.expectedNextDate ? data.expectedNextDate.toISOString() : null,
        //    odometerReading: data.odometerReading,
        //    expectedNextOdometerReading: data.expectedNextOdometerReading
        //}
        //command.attachmentFile = new VehicleServiceLogAttachmentDtoItem()
        //{

        //}


        //    file ? {
        //    fileData: data.file,
        //    fileName: data.file?.name || ''
        //} : null;

        //console.log(command.toJSON());
        //createMutation.mutate(command);
    }

    const updateServiceLog = (data: any, file: any) => {
        //var command = new UpdateVehicleServiceAsGarageLogDto();
        //command.serviceLogCommand = new CreateVehicleServiceLogAsGarageCommand()
        //{
        //    vehicleLicensePlate = data.licensePlate;
        //    type = data.type;
        //    description = data.description;
        //    date = data.date.toISOString();
        //    expectedNextDate = data.expectedNextDate ? data.expectedNextDate.toISOString() : null;
        //    odometerReading = data.odometerReading;
        //    expectedNextOdometerReading = data.expectedNextOdometerReading;
        //}

        //command.attachmentFile = file ? {
        //    fileData: data.file;
        //    fileName: data.file?.name || '';
        //} : null;



        //        dispatch(showOnSuccess(t('AddMaintenanceLog.Succeeded')));

        //        // Reset only specific form fields
        //        setValue('type', '');
        //        setValue('description', '');
        //        setValue('licensePlate', '');
        //        setValue('date', null);
        //        setValue('expectedNextDate', null);
        //        setValue('odometerReading', 0);
        //        setValue('expectedNextOdometerReading', 0);


        //console.log(command.toJSON());
        //updateMutation.mutate(command);
    }

    const deleteServiceLog = (data: any) => {
        console.log(data);
        deleteMutation.mutate(data.id);
    }

    // only reset the form when the data is loaded
    const loading = isLoading  || updateMutation.isLoading || deleteMutation.isLoading;// || createMutation.isLoading
    return {
        loading, isError, garageServiceLogs, updateServiceLog, deleteServiceLog
    }
}

export default useGarageServiceLogs;
