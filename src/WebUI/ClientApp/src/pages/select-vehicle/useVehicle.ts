import { useRef, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { VehicleClient } from "../../app/web-api-client";

function useVehicle(initialLicensePlate: string) {
    const [license_plate, setLicensePlate] = useState(initialLicensePlate);
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchVehicleBriefInfoData = async () => {
        try {
            const response = await vehicleClient.getVehicleBriefInfo(license_plate);
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: vehicleBriefInfo, isLoading, isError, refetch } = useQuery(
        [`vehicleBriefInfo-${license_plate}`],
        fetchVehicleBriefInfoData,
        {
            enabled: false,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const fetchVehicleData = (newLicensePlate: string) => {
        setLicensePlate(newLicensePlate);
        refetch();
    }

    const loading = isLoading;

    return {
        loading,
        isError,
        vehicleBriefInfo,
        fetchVehicleData
    }
}

export default useVehicle;
