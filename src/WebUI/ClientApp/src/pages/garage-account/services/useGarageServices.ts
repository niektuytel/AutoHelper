import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { BadRequestResponse, CreateGarageServiceCommand, GarageClient, UpdateGarageServiceCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { ROUTES } from "../../../constants/routes";
import { useAuth0 } from "@auth0/auth0-react";
import { GetGarageAccountClient } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useConfirmationStep from "../../../hooks/useConfirmationStep";

export default (onResponse: (data: any) => void) => {
    const { userRole } = useUserRole()
    const { configurationIndex, setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageAccountClient(accessToken);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageServicesData = async () => {
        try {
            const response = await garageClient.getServices(undefined);

            return response;
        } catch (response: any) {
            // redirect + enable garage register page
            if (response.status === 404) {
                setConfigurationIndex(1, userRole);
                navigate(ROUTES.GARAGE_ACCOUNT.SETTINGS);
                return;
            }

            throw response;
        }
    }

    const { data: garageServices, isLoading, isError } = useQuery(
        ['garageServices'],
        fetchGarageServicesData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const createMutation = useMutation(garageClient.createService.bind(garageClient), {
        onSuccess: (response) => {
            // Enable garage colleagues page
            setConfigurationIndex(3, userRole)
            dispatch(showOnSuccess("Garage service has been created!"));

            // Update the garageSettings in the cache after creating
            queryClient.setQueryData(['garageServices'], [...garageServices!, response]);
            onResponse(response);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const updateMutation = useMutation(garageClient.updateService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service has been updated!"));

            // Update the garageSettings in the cache after updating
            const updatedGarageServices = garageServices?.map((service) => {
                if (service.id === response.id) {
                    return response;
                } else {
                    return service;
                }
            });

            queryClient.setQueryData(['garageServices'], updatedGarageServices);
            onResponse(response);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const deleteMutation = useMutation(garageClient.deleteService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service has been deleted!"));

            // Delete the garageSettings in the cache after updating
            const updatedGarageServices = garageServices?.filter((service) => service.id !== response.id);

            console.log(updatedGarageServices);
            queryClient.setQueryData(['garageServices'], updatedGarageServices);
            onResponse(response);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const createService = (data: any) => {
        var command = new CreateGarageServiceCommand();
        command.type = data.type;
        command.vehicleType = data.vehicleType;
        command.title = data.title;
        command.description = data.description;
        command.expectedNextOdometerReadingIsRequired = data.expectedNextOdometerReadingIsRequired;
        command.expectedNextDateIsRequired = data.expectedNextDateIsRequired;

        
        console.log(command.toJSON());
        createMutation.mutate(command);
    }

    const updateService = (data: any) => {
        var command = new UpdateGarageServiceCommand();
        command.id = data.id;
        command.type = data.type;
        command.vehicleType = data.vehicleType;
        command.title = data.title;
        command.description = data.description;
        command.expectedNextOdometerReadingIsRequired = data.expectedNextOdometerReadingIsRequired;
        command.expectedNextDateIsRequired = data.expectedNextDateIsRequired;
        
        console.log(command.toJSON());
        updateMutation.mutate(command);
    }

    const deleteService = (data: any) => {
        console.log(data);
        deleteMutation.mutate(data.id);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || createMutation.isLoading || updateMutation.isLoading || deleteMutation.isLoading;
    return {
        loading, isError, garageServices, createService, updateService, deleteService
    }
}

