import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { VehicleClient } from "../../app/web-api-client";

function useVehicle(license_plate: string) {
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

    const { data: vehicleBriefInfo, isLoading, isError } = useQuery(
        [`vehicleBriefInfo-${license_plate}`],
        fetchVehicleBriefInfoData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, vehicleBriefInfo
    }
}

export default useVehicle;
