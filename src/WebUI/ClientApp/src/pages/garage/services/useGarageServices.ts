import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "react-query";
import { Dispatch } from "react";
import { FieldValues, UseFormReset, UseFormSetError } from "react-hook-form";
import { TFunction } from "i18next";
import { BriefBankingDetailsDto, BriefLocationDto, CreateGarageItemCommand, GarageBankingDetailsItem, GarageClient, GarageLocationItem, GarageSettings, UpdateGarageItemSettingsCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RoutesGarageSettings } from "../../../constants/routes";

//own imports

//function initialGarageLocation(): GarageLocationItem {
//    const location = new GarageLocationItem();
//    location.country = "Netherlands";
//    return location;
//}

//function guardHttpResponse(response: any, setError: UseFormSetError<FieldValues>, t: TFunction, dispatch: Dispatch<any>): any | null {
//    if (response.status === 400 && response.errors) {
//        // Iterate over all the error keys
//        Object.keys(response.errors).forEach((errorKey: string) => {
//            // Split the key by '.' and convert to lowercase
//            const field = errorKey.split('.').pop()?.toLowerCase();
//            if (!field) return;

//            setError(field, {
//                type: "manual",
//                message: t("Incorrect input, this is an required field")
//            });
//        });

//        return null;
//    } else {
//        response.errors.forEach((error: any) => {
//            dispatch(showOnError(error.message));
//            console.error(error);
//        });

//        return null;
//    }

//    return response;
//}

function useGarage(reset: UseFormReset<FieldValues>, setError: UseFormSetError<FieldValues>, notFound: boolean, garage_guid?: string) {
    //const garageClient = new GarageClient(process.env.PUBLIC_URL);
    //const initialGarageSettings = new GarageSettings({
    //    name: "",
    //    email: "",
    //    phoneNumber: "",
    //    whatsAppNumber: "",
    //    location: initialGarageLocation(),
    //    bankingDetails: new GarageBankingDetailsItem(),
    //    contacts: []
    //});
    //const queryClient = useQueryClient();
    //const dispatch = useDispatch();
    //const navigate = useNavigate();
    //const { t } = useTranslation();

    //const fetchGarageData = async () => {
    //    try {
    //        if (!garage_guid || notFound) {
    //            dispatch(showOnError(t("Garage not found!")));
    //            return initialGarageSettings;
    //        }

    //        const response = await garageClient.settings(garage_guid!);

    //        return response;
    //    } catch (response: any) {
    //        if (response && response.status === 404) {
    //            navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
    //            return initialGarageSettings;
    //        } else {
    //            throw response;
    //        }
    //    }
    //}

    //const { data: garageSettings, isLoading, isError } = useQuery(
    //    ['garageServices', garage_guid],
    //    fetchGarageData,
    //    {
    //        enabled: garage_guid != undefined,
    //        retry: 1,
    //        refetchOnWindowFocus: false,
    //        cacheTime: 30 * 60 * 1000,  // 30 minutes
    //        staleTime: 60 * 60 * 1000, // 1 hour
    //    }
    //);

    //const createMutation = useMutation(garageClient.create.bind(garageClient), {
    //    onSuccess: (response) => {
    //        dispatch(showOnSuccess("Garage has been created!"));

    //        // Update the garageSettings in the cache after creating
    //        queryClient.setQueryData(['garageSettings', garage_guid], response);
    //    },
    //    onError: (response) => {
    //        guardHttpResponse(response, setError, t, dispatch);
    //    }
    //});

    //const updateMutation = useMutation(garageClient.updateSettings.bind(garageClient), {
    //    onSuccess: (response) => {
    //        dispatch(showOnSuccess("Garage has been updated!"));

    //        // Update the garageSettings in the cache after updating
    //        queryClient.setQueryData(['garageSettings', garage_guid], response);
    //    },
    //    onError: (response) => {
    //        guardHttpResponse(response, setError, t, dispatch);
    //    }
    //});

    //const createGarage = (data: any) => {
    //    //var command = new CreateGarageItemCommand();
    //    //command.id = garage_guid;
    //    //command.name = data.name;
    //    //command.phoneNumber = data.phoneNumber;
    //    //command.whatsAppNumber = data.whatsAppNumber;
    //    //command.email = data.email;

    //    //const cleanAddress = data.address.replace(`, ${data.city}`, '');
    //    //command.location = new BriefLocationDto({
    //    //    address: cleanAddress,
    //    //    postalCode: data.postalCode,
    //    //    city: data.city,
    //    //    country: data.country,
    //    //    longitude: data.longitude,
    //    //    latitude: data.latitude

    //    //});
    //    //command.bankingDetails = new BriefBankingDetailsDto({
    //    //    bankName: data.bankName,
    //    //    kvKNumber: data.kvKNumber,
    //    //    accountHolderName: data.accountHolderName,
    //    //    iban: data.iban
    //    //});

    //    //console.log(command.toJSON());
    //    //createMutation.mutate(command);
    //}

    //const updateGarageSettings = (data: any) => {

    //    //var command = new UpdateGarageItemSettingsCommand();
    //    //command.id = garage_guid;
    //    //command.name = data.name;
    //    //command.phoneNumber = data.phoneNumber;
    //    //command.whatsAppNumber = data.whatsAppNumber;
    //    //command.email = data.email;

    //    //command.location = new GarageLocationItem();
    //    //command.location.id = garageSettings?.location?.id,
    //    //command.location.address = data.address.replace(`, ${data.city}`, ''),
    //    //command.location.postalCode = data.postalCode,
    //    //command.location.city = data.city,
    //    //command.location.country = data.country,
    //    //command.location.longitude = data.longitude,
    //    //command.location.latitude = data.latitude

    //    //command.bankingDetails = new GarageBankingDetailsItem();
    //    //command.bankingDetails.id = garageSettings?.bankingDetails?.id;
    //    //command.bankingDetails.bankName = data.bankName;
    //    //command.bankingDetails.kvKNumber = data.kvKNumber;
    //    //command.bankingDetails.accountHolderName = data.accountHolderName;
    //    //command.bankingDetails.iban = data.iban;

    //    //command.servicesSettings = garageSettings?.servicesSettings;

    //    console.log(data);
    //    //console.log(command.toJSON());
    //    //updateMutation.mutate(command);
    //}

    //// only reset the form when the data is loaded
    //const loading = isLoading || createMutation.isLoading;
    //return {
    //    loading, isError, garageSettings, createGarage, updateGarageSettings
    //}
}

export default useGarage;
