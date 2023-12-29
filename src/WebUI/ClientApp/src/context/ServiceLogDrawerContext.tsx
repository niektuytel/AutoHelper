import React, { createContext, useState, ReactNode, useCallback } from 'react';

type ServiceLogDrawerContextType = {
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => void;
};

export const ServiceLogDrawerContext = createContext<ServiceLogDrawerContextType | undefined>(undefined);

type DrawerProviderProps = {
    children: ReactNode;
};

export const ServiceLogDrawerProvider = ({ children }: DrawerProviderProps) => {
    const [drawerOpen, setDrawerOpen] = useState(false);

    const toggleDrawer = useCallback((open: boolean) => {
        setDrawerOpen(open);
    }, []);

    return (
        <ServiceLogDrawerContext.Provider value={{ drawerOpen, toggleDrawer }}>
            {children}
        </ServiceLogDrawerContext.Provider>
    );
};