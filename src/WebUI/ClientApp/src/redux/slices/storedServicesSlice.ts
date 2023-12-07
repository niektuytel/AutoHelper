import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Cookies } from 'react-cookie';
import { SelectedService } from '../../app/web-api-client';

const cookies = new Cookies();
const initialState: SelectedService[] = [];

const getServicesFromCookie = (): SelectedService[] => {
    const services = cookies.get('storedServices');
    return services || [];
};

const setServicesToCookie = (services: SelectedService[]) => {
    cookies.set('storedServices', services, { path: '/', maxAge: 604800 }); // maxAge is in seconds (7 days)
};

const storedServicesSlice = createSlice({
    name: 'storedServices',
    initialState: getServicesFromCookie(),
    reducers: {
        addService: (state, action: PayloadAction<SelectedService>) => {
            const newState = [...state, action.payload];
            setServicesToCookie(newState);
            return newState;
        },
        removeService: (state, action: PayloadAction<SelectedService>) => {
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
