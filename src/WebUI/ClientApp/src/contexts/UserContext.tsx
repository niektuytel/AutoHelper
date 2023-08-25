import React, { createContext, useState } from 'react';

interface UserContextProps {
    userRoles: string[] | null;
    setUserRoles: React.Dispatch<React.SetStateAction<string[] | null>>;
    garageGUID: string | null;
    setGarageGUID: React.Dispatch<React.SetStateAction<string | null>>;
}

export const UserContext = createContext<UserContextProps | undefined>(undefined);

interface UserProviderProps {
    children: React.ReactNode;
}


// TODO: use redux instead of context
export const UserProvider: React.FC<UserProviderProps> = ({ children }) => {
    const [userRoles, setUserRoles] = useState<string[] | null>(null);
    const [garageGUID, setGarageGUID] = useState<string | null>(null);

    return (
        <UserContext.Provider value={{ userRoles, setUserRoles, garageGUID, setGarageGUID }}>
            {children}
        </UserContext.Provider>
    );
};

