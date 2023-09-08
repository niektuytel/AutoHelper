import { useState } from 'react';
import { ROLES } from '../constants/roles';

function useConfirmationStep() {
    const [configurationIndex, setConfigurationIndex] = useState(Number(localStorage.getItem('confirmationStepIndex')) || 0);

    const setLocalConfigurationIndex = (index: number, role: string) => {
        // Enable garage register page
        if (index == 1 && configurationIndex == 0 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
        }

        // Enable garage services & overview pages
        if (index == 2 && configurationIndex == 1 && role === ROLES.GARAGE) {
            localStorage.setItem('confirmationStepIndex', index.toString());
            setConfigurationIndex(index);
        }
    }

    return {
        configurationIndex,
        setConfigurationIndex: setLocalConfigurationIndex
    };
}

export default useConfirmationStep;
