import { useQuery, useQueryClient } from "react-query";
import { useState, useEffect } from "react";
import { GarageClient, GarageServiceDtoItem } from "../../../app/web-api-client";
import { useAuth0 } from "@auth0/auth0-react";
import { GetGarageAccountClient, useHandleApiRequest } from "../../../app/GarageClient";
import { useTranslation } from "react-i18next";

export default (license: string) => {
    const { t } = useTranslation();
    const [licensePlate, setLicensePlate] = useState(license);
    const garageClient = GetGarageAccountClient();
    const handleApiRequest = useHandleApiRequest<GarageServiceDtoItem[]>();

    const fetchGarageServiceTypes = async () => {
        const response = await handleApiRequest(
            async () => await garageClient.getServices(licensePlate),
            t("GarageClient.404.Message")
        );

        return response;
    }

    const { data: garageServiceTypes, isLoading, isError, refetch } = useQuery(
        ['garageServices-ForLogs', licensePlate],
        fetchGarageServiceTypes,
        {
            enabled: (licensePlate?.length > 0),
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000, // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const triggerFetch = (newLicensePlate: string) => {
        setLicensePlate(newLicensePlate);
    }

    useEffect(() => {
        if (licensePlate.length > 0) {
            refetch();
        }
    }, [licensePlate, refetch]);

    return {
        loading: isLoading,
        isError,
        garageServiceTypes: garageServiceTypes,
        triggerFetch
    };
}
