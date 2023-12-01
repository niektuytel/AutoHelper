import React, { createContext, useContext, useState } from 'react';
import { VehicleServiceLogDtoItem } from '../../app/web-api-client';
import useVehicleServiceLogs from './useVehicleServiceLogs';

// Define the context type
interface DrawerContextType {
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => void;
    addServiceLog: (newServiceLog: VehicleServiceLogDtoItem) => void;
}

const DrawerContext = createContext<DrawerContextType>({
    drawerOpen: false,
    toggleDrawer: () => { },
    addServiceLog: () => { }
});

export const useDrawer = () => useContext(DrawerContext);

interface IProps {
    children: React.ReactNode;
    license_plate: string;
}

export default ({ children, license_plate }: IProps) => {
    const [drawerOpen, setDrawerOpen] = useState(false);
    const { addServiceLog } = useVehicleServiceLogs(license_plate);

    const toggleDrawer = (open: boolean) => setDrawerOpen(open);

    return (
        <DrawerContext.Provider value={{ drawerOpen, toggleDrawer, addServiceLog }}>
            {children}
        </DrawerContext.Provider>
    );
};
