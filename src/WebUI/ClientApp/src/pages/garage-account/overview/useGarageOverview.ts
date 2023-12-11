import { useNavigate } from "react-router";
import { useQuery } from "react-query";
import { useTranslation } from "react-i18next";
import { useAuth0 } from "@auth0/auth0-react";

//own imports
import { ROUTES } from "../../../constants/routes";
import { GetGarageAccountClient } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useConfirmationStep from "../../../hooks/useConfirmationStep";

export default () => {
    const { userRole } = useUserRole()
    const { setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageAccountClient(accessToken);
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageOverview = async () => {
        try {
            const response = await garageClient.getOverview();
            return response;
        } catch (response: any) {
            // redirect + enable garage register page
            if (response.status === 404) {
                setConfigurationIndex(0, userRole);
                navigate(ROUTES.GARAGE_ACCOUNT.SETTINGS);
                return;
            }

            throw response;
        }
    }

    const { data: garageOverview, isLoading, isError } = useQuery(
        ['garageOverview'],
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

