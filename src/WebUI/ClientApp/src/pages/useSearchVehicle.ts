import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { ValidationProblemDetails, VehicleSpecificationsCardItem, VehicleClient } from "../app/web-api-client";
import { useState } from "react";
import { showOnError } from "../redux/slices/statusSnackbarSlice";

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
            const response = await vehicleClient.getSpecificationsCard(licensePlate);
            setLoading(false);
            return response;
        } catch (error) {
            console.error('Error:', error);

            // Display specific error message from server response
            if (error instanceof ValidationProblemDetails && error.errors) {
                dispatch(showOnError(t(Object.entries(error.errors)[0][1])));
            }
        } finally {
            setLoading(false);
        }
    }

    const fetchVehicleByPlate = async (licensePlate: string) => {
        // Check if data exists in cache first
        const cachedData = queryClient.getQueryData(
            [`vehicleBriefInfo-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData as VehicleSpecificationsCardItem;
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