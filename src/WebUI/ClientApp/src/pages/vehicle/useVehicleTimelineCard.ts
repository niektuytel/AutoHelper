﻿import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { ValidationProblemDetails, VehicleClient } from "../../app/web-api-client";
import { showOnError } from "../../redux/slices/statusSnackbarSlice";

function useVehicleTimelineCard(license_plate: string) {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchVehicleTimeline = async (licensePlate: string) => {
        try {
            const response = await vehicleClient.getTimeline(licensePlate, 4);
            return response;
        } catch (error) {
            console.error('Error:', error);

            // Display specific error message from server response
            if (error instanceof ValidationProblemDetails && error.errors) {
                dispatch(showOnError(Object.entries(error.errors)[0][1]));
            }
        }
    }

    const { data: vehicleTimelineCard, isLoading, isError } = useQuery(
        [`vehicleTimelineCard-${license_plate}`],
        () => fetchVehicleTimeline(license_plate),
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
            [`vehicleTimelineCard-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData;
        }

        return fetchVehicleTimeline(licensePlate);
    }


    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, vehicleTimelineCard, fetchVehicleByPlate
    }
}

export default useVehicleTimelineCard;
