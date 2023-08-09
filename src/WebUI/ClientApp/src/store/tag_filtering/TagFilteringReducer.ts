import { Action, Reducer } from 'redux';
import { KnownFilterSituationAction } from './TagFilteringActions';
import FilterSituationState from './TagFilteringState';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: FilterSituationState = { 
    type: undefined,
    target: undefined,
    currentStep: 0,
    completedSteps: {},
    usedSituations: {}
};

export const reducer: Reducer<FilterSituationState> = (state: FilterSituationState | undefined, incomingAction: Action): FilterSituationState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownFilterSituationAction;
    switch (action.type) {
        case 'SET_FILTERING_TARGET':
            return {
                ...state,
                type: action.type,
                target:action.target
            };

        case 'SET_CURRENT_STEP':
            return {
                ...state,
                type: action.type,
                currentStep:action.currentStep
            };
        case 'SET_COMPLETED_STEPS':
            return {
                ...state,
                type: action.type,
                completedSteps:action.completedSteps
            };
        case 'SET_USED_FILTER_SITUATIONS':
            return {
                ...state,
                type: action.type,
                usedSituations: action.usedSituations
            };
        default: 
            break;
    }

    return state;
};
