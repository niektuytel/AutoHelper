import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { CreateGarageEmployeeCommand, GarageClient, UpdateGarageEmployeeCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { ROUTES } from "../../../constants/routes";
import { useAuth0 } from "@auth0/auth0-react";
import { GetGarageClient } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useConfirmationStep from "../../../hooks/useConfirmationStep";

function useGarageEmployees(onResponse: (data: any) => void) {
    const { userRole } = useUserRole()
    const { configurationIndex, setConfigurationIndex } = useConfirmationStep();
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageClient(accessToken);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageEmployeesData = async () => {
        try {
            const response = await garageClient.getEmployees();

            return response;
        } catch (response: any) {
            // redirect + enable garage register page
            if (response.status === 404) {
                setConfigurationIndex(1, userRole);
                navigate(ROUTES.GARAGE.SETTINGS);
                return;
            }

            throw response;
        }
    }

    const { data: garageEmployees, isLoading, isError } = useQuery(
        ['garageEmployees'],
        fetchGarageEmployeesData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const createMutation = useMutation(garageClient.createEmployee.bind(garageClient), {
        onSuccess: (response) => {
            // Enable garage colleagues page
            setConfigurationIndex(3, userRole)
            dispatch(showOnSuccess("Garage service has been created!"));

            // Update the garageSettings in the cache after creating
            queryClient.setQueryData(['garageEmployees'], [...garageEmployees!, response]);
            onResponse(response);
        },
        onError: (response) => {
            console.error(response)
            //guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const updateMutation = useMutation(garageClient.updateEmployee.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service has been updated!"));

            // Update the garageSettings in the cache after updating
            const updatedGarageEmployees = garageEmployees?.map((service) => {
                if (service.id === response.id) {
                    return response;
                } else {
                    return service;
                }
            });

            queryClient.setQueryData(['garageEmployees'], updatedGarageEmployees);
            onResponse(response);
        },
        onError: (response) => {
            console.error(response)
            //guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const deleteMutation = useMutation(garageClient.deleteEmployee.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Employee has been deleted!"));

            // Delete the garageSettings in the cache after updating
            const updatedGarageEmployees = garageEmployees?.filter((service) => service.id !== response.id);

            console.log(updatedGarageEmployees);
            queryClient.setQueryData(['garageEmployees'], updatedGarageEmployees);
            onResponse(response);
        },
        onError: (response) => {
            console.error(response)
            //guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const createEmployee = (data: any) => {
        var command = new CreateGarageEmployeeCommand();
        //command.type = data.type;
        //command.description = data.description;
        //command.price = data.price;
        //command.durationInMinutes = data.durationInMinutes;

        console.log(command.toJSON());
        createMutation.mutate(command);
    }

    const updateEmployee = (data: any) => {
        var command = new UpdateGarageEmployeeCommand();
        command.id = data.id;
        //command.type = data.type;
        //command.description = data.description;
        //command.price = data.price;
        //command.durationInMinutes = data.durationInMinutes;

        console.log(command.toJSON());
        updateMutation.mutate(command);
    }

    const deleteEmployee = (data: any) => {
        console.log(data);
        deleteMutation.mutate(data.id);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || createMutation.isLoading || updateMutation.isLoading || deleteMutation.isLoading;
    return {
        loading, isError, garageEmployees, createEmployee, updateEmployee, deleteEmployee
    }
}

export default useGarageEmployees;
