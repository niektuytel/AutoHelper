﻿import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { ValidationProblemDetails, VehicleClient } from "../../app/web-api-client";
import { showOnError } from "../../redux/slices/statusSnackbarSlice";

function useVehicleInformation(license_plate: string) {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchVehicleInfoData = async (licensePlate: string) => {
        try {
            const response = await vehicleClient.getSpecifications(licensePlate);
            return response;
        } catch (error) {
            console.error('Error:', error);

            // Display specific error message from server response
            if (error instanceof ValidationProblemDetails && error.errors) {
                dispatch(showOnError(Object.entries(error.errors)[0][1]));
            }
        }
    }

    const { data: vehicleInfo, isLoading, isError } = useQuery(
        [`vehicleInfo-${license_plate}`],
        () => fetchVehicleInfoData(license_plate),
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
            [`vehicleInfo-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData;
        }

        return fetchVehicleInfoData(licensePlate);
    }


    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, vehicleInfo, fetchVehicleByPlate
    }
}

export default useVehicleInformation;
