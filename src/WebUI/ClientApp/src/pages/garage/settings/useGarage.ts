import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "react-query";
import { Dispatch } from "react";
import { FieldValues, UseFormReset, UseFormSetError } from "react-hook-form";
import { TFunction } from "i18next";
import { BankingDetailsItem, BriefBankingDetailsDto, BriefLocationDto, CreateGarageItemCommand, GarageClient, GarageSettings, LocationItem, UpdateGarageItemSettingsCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RoutesGarageSettings } from "../../../constants/routes";

//own imports

function initialGarageLocation(): LocationItem {
    const location = new LocationItem();
    location.country = "Netherlands";
    return location;
}

function guardHttpResponse(response: any, setError: UseFormSetError<FieldValues>, t: TFunction, dispatch: Dispatch<any>): any | null {
    if (response.status === 400 && response.errors) {
        // Iterate over all the error keys
        Object.keys(response.errors).forEach((errorKey: string) => {
            // Split the key by '.' and convert to lowercase
            const field = errorKey.split('.').pop()?.toLowerCase();
            if (!field) return;

            setError(field, {
                type: "manual",
                message: t("Incorrect input, this is an required field")
            });
        });

        return null;
    } else {
        response.errors.forEach((error: any) => {
            dispatch(showOnError(error.message));
            console.error(error);
        });

        return null;
    }

    return response;
}

function useGarage(reset: UseFormReset<FieldValues>, setError: UseFormSetError<FieldValues>, notFound: boolean, garage_guid?: string) {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const initialGarageSettings = new GarageSettings({
        name: "",
        email: "",
        phoneNumber: "",
        whatsAppNumber: "",
        location: initialGarageLocation(),
        bankingDetails: new BankingDetailsItem(),
        contacts: []
    });
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageData = async () => {
        try {
            if (!garage_guid || notFound) {
                dispatch(showOnError(t("Garage not found!")));
                return initialGarageSettings;
            }

            const response = await garageClient.settings(garage_guid!);

            return response;
        } catch (response: any) {
            if (response && response.status === 404) {
                navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
                return initialGarageSettings;
            } else {
                throw response;
            }
        }
    }

    //const cachedData = queryClient.getQueryData(['garageSettings', garage_guid]);
    const { data: garageSettings, isLoading, isError } = useQuery(
        ['garageSettings', garage_guid],
        fetchGarageData,
        {
            enabled: garage_guid != undefined,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour

            //initialData: cachedData,
            onSuccess: (data: any) => {
                reset({
                    name: data.name,
                    address: `${data.location?.address}, ${data.location?.city}`,
                    country: data.location?.country,
                    city: data.location?.city,
                    latitude: data.location?.latitude,
                    longitude: data.location?.longitude,
                    postalCode: data.location?.postalCode,
                    phoneNumber: data.phoneNumber,
                    whatsAppNumber: data.whatsAppNumber,
                    email: data.email,
                    kvKNumber: data.bankingDetails?.kvKNumber,
                    bankName: data.bankingDetails?.bankName,
                    accountHolderName: data.bankingDetails?.accountHolderName,
                    iban: data.bankingDetails?.iban,
                });
            }
        }
    );


    const mutation = useMutation(garageClient.create.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage has been created!"));

            // Step 3: Invalidate and refetch the garageSettings query
            queryClient.invalidateQueries(['garageSettings', garage_guid]);
        },
        onError: (response) => {
            guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const createGarage = (data: any) => {
        var command = new CreateGarageItemCommand();
        command.id = garage_guid;
        command.name = data.name;
        command.location = new BriefLocationDto({
            address: data.address,
            postalCode: data.postalCode,
            city: data.city,
            country: data.country,
            longitude: data.longitude,
            latitude: data.latitude

        });
        command.phoneNumber = data.phoneNumber;
        command.whatsAppNumber = data.whatsAppNumber;
        command.email = data.email;
        command.bankingDetails = new BriefBankingDetailsDto({
            bankName: data.bankName,
            kvKNumber: data.kvKNumber,
            accountHolderName: data.accountHolderName,
            iban: data.iban
        });

        console.log(command.toJSON());
        mutation.mutate(command);
    }

    const updateGarageSettings = (data: any) => {
        console.log(data);

        var command = new UpdateGarageItemSettingsCommand();
        command.id = garage_guid;
        command.name = data.name;
        command.location = new BriefLocationDto({
            address: data.address,
            postalCode: data.postalCode,
            city: data.city,
            country: data.country,
            longitude: data.longitude,
            latitude: data.latitude

        });
        //command.phoneNumber = data.phoneNumber;
        //command.whatsAppNumber = data.whatsAppNumber;
        //command.email = data.email;
        command.bankingDetails = new BriefBankingDetailsDto({
            bankName: data.bankName,
            kvKNumber: data.kvKNumber,
            accountHolderName: data.accountHolderName,
            iban: data.iban
        });

        //mutation.mutate(command);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || mutation.isLoading;
    useEffect(() => {
        if (garageSettings && !loading) {
            console.log("reset form with data garageSettings: ", garageSettings);
            reset({
                name: garageSettings.name,
                address: `${garageSettings.location?.address}, ${garageSettings.location?.city}`,
                country: garageSettings.location?.country,
                city: garageSettings.location?.city,
                latitude: garageSettings.location?.latitude,
                longitude: garageSettings.location?.longitude,
                postalCode: garageSettings.location?.postalCode,
                phoneNumber: garageSettings.phoneNumber,
                whatsAppNumber: garageSettings.whatsAppNumber,
                email: garageSettings.email,
                kvKNumber: garageSettings.bankingDetails?.kvKNumber,
                bankName: garageSettings.bankingDetails?.bankName,
                accountHolderName: garageSettings.bankingDetails?.accountHolderName,
                iban: garageSettings.bankingDetails?.iban,
            });
        }
    }, [garageSettings, loading, reset]);

    return {
        loading, isError, garageSettings, createGarage, updateGarageSettings
    }
}

export default useGarage;
