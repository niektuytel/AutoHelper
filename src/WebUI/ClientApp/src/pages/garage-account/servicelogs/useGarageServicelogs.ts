import { useLocation, useNavigate } from "react-router";
import { useDispatch } from "react-redux";
import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useAuth0 } from "@auth0/auth0-react";

//own imports
import { BadRequestResponse, FileParameter, GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogStatus } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { ROUTES } from "../../../constants/routes";
import { GetGarageAccountClient, useHandleApiRequest } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useConfirmationStep from "../../../hooks/useConfirmationStep";

function useGarageServiceLogs(onResponse: (data: any) => void) {
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const garageClient = GetGarageAccountClient();
    const handleApiRequest = useHandleApiRequest<VehicleServiceLogAsGarageDtoItem[]>();

    const fetchGarageServiceLogsData = async () => {
        const response = await handleApiRequest(
            async () => await garageClient.getServiceLogs(null),
            t("GarageClient.404.Message")
        );

        return response;
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

    type CreateServiceLogParams = {
        vehicleLicensePlate: string | undefined;
        garageServiceId: string | undefined;
        description: string | undefined;

        date: string | undefined;
        expectedNextDate: string | undefined;
        odometerReading: number | undefined;
        expectedNextOdometerReading: number | undefined;
        attachmentFile: FileParameter | null;
    };

    const createMutationFunction = async ({
        vehicleLicensePlate,
        garageServiceId,
        description,
        date,
        expectedNextDate,
        odometerReading,
        expectedNextOdometerReading,
        attachmentFile
    }: CreateServiceLogParams) => {
        return await garageClient.createServiceLog(
            vehicleLicensePlate,
            garageServiceId,
            description,
            date,
            expectedNextDate,
            odometerReading,
            expectedNextOdometerReading,
            attachmentFile
        );
    };

    const createMutation = useMutation(createMutationFunction, {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service log is been created!"));

            // Update the garageSettings in the cache after creating
            queryClient.setQueryData(['garageServiceLogs'], [...garageServiceLogs!, response]);
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

    const createServiceLog = (data: any, file: File | null) => {
        createMutation.mutate({
            vehicleLicensePlate: data.licensePlate,
            garageServiceId: data.garageServiceId,
            description: data.description,

            date: data.date?.toISOString(),
            expectedNextDate: data.expectedNextDate?.toISOString(),
            odometerReading: data.odometerReading,
            expectedNextOdometerReading: data.expectedNextOdometerReading,
            attachmentFile: file ? { data: file, fileName: file?.name || '' } : null
        });
    }

    type UpdateServiceLogParams = {
        id: string;
        vehicleLicensePlate: string | undefined;
        garageServiceId: string | undefined;
        description: string | undefined;

        date: string | undefined;
        expectedNextDate: string | undefined;
        odometerReading: number | undefined;
        expectedNextOdometerReading: number | undefined;
        status: VehicleServiceLogStatus | undefined;
        attachmentFile: FileParameter | null;
    };

    const updateMutationFunction = async ({
        id,
        vehicleLicensePlate,
        garageServiceId,
        description,
        date,
        expectedNextDate,
        odometerReading,
        expectedNextOdometerReading,
        status,
        attachmentFile
    }: UpdateServiceLogParams) => {
        return await garageClient.updateServiceLog(
            id,
            vehicleLicensePlate,
            garageServiceId,
            description,
            date,
            expectedNextDate,
            odometerReading,
            expectedNextOdometerReading,
            status,
            attachmentFile
        );
    };

    const updateMutation = useMutation(updateMutationFunction, {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service log is been updated!"));

            // Update the garageSettings in the cache after updating
            const updatedGarageServices = garageServiceLogs?.map((service: any) => {
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

    const updateServiceLog = (data: any, file: File | null) => {
        console.log(data);
        updateMutation.mutate({
            id: data.id,
            vehicleLicensePlate: data.licensePlate,
            garageServiceId: data.garageServiceId,
            description: data.description,
            date: data.date?.toISOString(),
            expectedNextDate: data.expectedNextDate?.toISOString(),
            odometerReading: data.odometerReading,
            expectedNextOdometerReading: data.expectedNextOdometerReading,
            status: data.status,
            attachmentFile: file ? { data: file, fileName: file?.name || '' } : null
        });
    }

    const deleteMutation = useMutation(garageClient.deleteServiceLog.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service log is been deleted!"));

            // Delete the garageSettings in the cache after updating
            const updatedGarageServices = garageServiceLogs?.filter((service: any) => service.id !== response.id);
            queryClient.setQueryData(['garageServiceLogs'], updatedGarageServices);
            console.log(updatedGarageServices);

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

    const deleteServiceLog = (data: any) => {
        deleteMutation.mutate(data.id);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || createMutation.isLoading || updateMutation.isLoading || deleteMutation.isLoading;
    return {
        loading, isError, garageServiceLogs, createServiceLog, updateServiceLog, deleteServiceLog
    }
}

export default useGarageServiceLogs;
