import { useState } from 'react';
import { ROLES } from '../constants/roles';

export default () => {
    const [roleIndex, setRoleIndex] = useState(Number(localStorage.getItem('roleIndex')) || 0);

    const setLocalroleIndex = (index: number, role: string) => {
        // looks like you have access to everything on first login (100)
        if (index == 100 && role === ROLES.GARAGE) {
            localStorage.setItem('roleIndex', index.toString());
            setRoleIndex(index);
            return;
        }

        // Enable garage register page
        if (index == 1 && role === ROLES.GARAGE) {
            localStorage.setItem('roleIndex', index.toString());
            setRoleIndex(index);
            return;
        }

        // Enable garage services & overview pages
        if (index == 2 && role === ROLES.GARAGE) {
            localStorage.setItem('roleIndex', index.toString());
            setRoleIndex(index);
            return;
        }

        // Enable colleagues page
        if (index == 3 && roleIndex < 3 && role === ROLES.GARAGE) {
            localStorage.setItem('roleIndex', index.toString());
            setRoleIndex(index);
            return;
        }

        // Enable planning page
        if (index == 4 && roleIndex < 4 && role === ROLES.GARAGE) {
            localStorage.setItem('roleIndex', index.toString());
            setRoleIndex(index);
            return;
        }
    }

    return {
        roleIndex,
        setRoleIndex: setLocalroleIndex
    };
}
