import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Cookies } from 'react-cookie';
import { VehicleService } from '../../app/web-api-client';

const cookies = new Cookies();
const initialState: VehicleService[] = [];

const getServicesFromCookie = (): VehicleService[] => {
    const services = cookies.get('storedServices');
    return services || [];
};

const setServicesToCookie = (services: VehicleService[]) => {
    cookies.set('storedServices', services, { path: '/', maxAge: 604800 }); // maxAge is in seconds (7 days)
};

const storedServicesSlice = createSlice({
    name: 'storedServices',
    initialState: getServicesFromCookie(),
    reducers: {
        addService: (state, action: PayloadAction<VehicleService>) => {
            const newState = [...state, action.payload];
            setServicesToCookie(newState);
            return newState;
        },
        removeService: (state, action: PayloadAction<VehicleService>) => {
            const serviceToRemove = action.payload;
            const newState = state.filter((service) =>
                service.relatedGarageLookupIdentifier !== serviceToRemove.relatedGarageLookupIdentifier //||
                //service.relatedServiceType !== serviceToRemove.relatedServiceType
            );
            //TODO: set services on cookie
            setServicesToCookie(newState);
            return newState;
        },
        getServices: () => {
            return getServicesFromCookie();
        },
    },
});

export const { addService, removeService, getServices } = storedServicesSlice.actions;
export default storedServicesSlice.reducer;
