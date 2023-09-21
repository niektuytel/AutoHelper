import { useState } from 'react';
import { ROLES } from '../constants/roles';

function useConfirmationStep() {
    const [configurationIndex, setConfigurationIndex] = useState(Number(localStorage.getItem('confirmationStepIndex')) || 0);

    const setLocalConfigurationIndex = (index: number, role: string) => {
        // looks like you have access to everything on first login (100)
        if (index == 100 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
            return;
        }

        // Enable garage register page
        if (index == 1 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
            return;
        }

        // Enable garage services & overview pages
        if (index == 2 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
            return;
        }

        // Enable colleagues page
        if (index == 3 && configurationIndex < 3 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
            return;
        }

        // Enable planning page
        if (index == 4 && configurationIndex < 4 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
            return;
        }
    }

    return {
        configurationIndex,
        setConfigurationIndex: setLocalConfigurationIndex
    };
}

export default useConfirmationStep;
