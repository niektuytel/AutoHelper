﻿import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { VehicleClient } from "../../app/web-api-client";

function useVehicleServiceLogs(license_plate: string) {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchVehicleServiceLogsData = async (licensePlate: string) => {
        try {
            const response = await vehicleClient.searchByLicensePlate(licensePlate);
            return response;
        } catch (response: any) {
            throw response;
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


    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, vehicleServiceLogs, fetchVehicleByPlate
    }
}

export default useVehicleServiceLogs;
