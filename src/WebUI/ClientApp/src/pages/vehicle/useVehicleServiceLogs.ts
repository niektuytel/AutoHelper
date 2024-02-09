import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { ValidationProblemDetails, VehicleClient, VehicleServiceLogDtoItem } from "../../app/web-api-client";
import { showOnError } from "../../redux/slices/statusSnackbarSlice";

function useVehicleServiceLogs(license_plate: string) {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();

    const fetchVehicleServiceLogsData = async (licensePlate: string) => {
        try {
            const response = await vehicleClient.getServiceLogs(licensePlate);
            return response;
        } catch (error) {
            console.error('Error:', error);

            // Display specific error message from server response
            if (error instanceof ValidationProblemDetails && error.errors) {
                dispatch(showOnError(Object.entries(error.errors)[0][1]));
            }

            // Re-throw the error to let React Query's useQuery handle it
            throw error;
        }
    }

    const { data: vehicleServiceLogs, isLoading, isError } = useQuery(
        [`vehicleServiceLogs-${license_plate}`],
        () => fetchVehicleServiceLogsData(license_plate),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const fetchVehicleByPlate = async (licensePlate: string) => {
        // Check if data exists in cache first
        const cachedData = queryClient.getQueryData(
            [`vehicleServiceLogs-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData;
        }

        return fetchVehicleServiceLogsData(licensePlate);
    }

    const addServiceLog = (newServiceLog: VehicleServiceLogDtoItem) => {
        // Optimistically update the cache with the new log at the beginning of the list
        queryClient.setQueryData<VehicleServiceLogDtoItem[]>([`vehicleServiceLogs-${license_plate}`], (oldLogs) => [newServiceLog, ...(oldLogs ?? [])]);
    };

    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, vehicleServiceLogs, fetchVehicleByPlate, addServiceLog
    }
}

export default useVehicleServiceLogs;
