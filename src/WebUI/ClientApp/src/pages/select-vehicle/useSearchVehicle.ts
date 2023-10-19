import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { VehicleBriefDtoItem, VehicleClient } from "../../app/web-api-client";
import { useState } from "react";

function useSearchVehicle() {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const [loading, setLoading] = useState(false);

    const fetchVehicleBriefInfoData = async (licensePlate: string) => {
        setLoading(true);
        try {
            const response = await vehicleClient.searchByLicensePlate(licensePlate);
            setLoading(false);
            return response;
        } catch (response: any) {
            setLoading(false);
            throw response;
        }
    }

    const fetchVehicleByPlate = async (licensePlate: string) => {
        // Check if data exists in cache first
        const cachedData = queryClient.getQueryData(
            [`vehicleBriefInfo-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData as VehicleBriefDtoItem;
        }

        const response = await fetchVehicleBriefInfoData(licensePlate);
        queryClient.setQueryData([`vehicleBriefInfo-${licensePlate}`], response);

        return response;
    }

    return {
        loading,
        fetchVehicleByPlate
    }
}

export default useSearchVehicle;