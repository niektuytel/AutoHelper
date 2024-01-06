import { useLocation, useNavigate } from "react-router";
import { useQuery } from "react-query";
import { useTranslation } from "react-i18next";

//own imports
import { GetGarageAccountClient, useHandleApiRequest } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import { GarageOverviewDtoItem } from "../../../app/web-api-client";

export default () => {
    const { t } = useTranslation();
    const { userRole } = useUserRole()
    const garageClient = GetGarageAccountClient();
    const handleApiRequest = useHandleApiRequest<GarageOverviewDtoItem>();

    const fetchGarageOverview = async () => {
        const response = await handleApiRequest(
            async () => await garageClient.getOverview(),
            t("GarageClient.404.Message")
        );

        return response;
    }

    const { data: garageOverview, isLoading, isError } = useQuery(
        ['garageOverview', userRole],
        fetchGarageOverview,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000, // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    // only reset the form when the data is loaded
    const loading = isLoading;
    return { loading, isError, garageOverview }
}

