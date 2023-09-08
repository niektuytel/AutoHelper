import { useState, useEffect } from 'react';
import { ROLES } from '../constants/roles';

function useUserRole() {
    const [userRole, setUserRole] = useState(localStorage.getItem('userRole') || ROLES.USER);

    const setLocalStorageUserRole = (role: string) => {
        if (Object.values(ROLES).includes(role)) {
            localStorage.setItem('userRole', role);
            setUserRole(role);
        } else {
            console.warn(`Provided role "${role}" is not a valid role.`);
        }
    }

    return {
        userRole,
        setUserRole: setLocalStorageUserRole,
    };
}

export default useUserRole;
