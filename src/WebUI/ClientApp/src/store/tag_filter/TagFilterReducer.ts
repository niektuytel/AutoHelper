import { Action, Reducer } from 'redux';
import TagFilterState from './TagFilterState';
import { KnownTagFilterAction } from './TagFilterActions';

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: TagFilterState = { type: undefined, isLoading: undefined, filters:[], filterQuestions:[], targets:[] };

export const reducer: Reducer<TagFilterState> = (state: TagFilterState | undefined, incomingAction: Action): TagFilterState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownTagFilterAction;
    switch (action.type) {
        case 'TAG_FILTERS_REQUEST':
            return {
                ...state,
                type: action.type,
                isLoading: true
            };
        case 'TAG_FILTERS_RESPONSE':
            return {
                ...state,
                type: action.type,
                filters: action.filters,
                isLoading: false
            };
        case 'TAG_FILTER_TARGET_REQUEST':
            return {
                ...state,
                type: action.type,
                isLoading: true
            };
        case 'TAG_FILTER_TARGET_RESPONSE':
            return {
                ...state,
                type: action.type,
                targets: action.targets,
                isLoading: false
            };
        case 'TAG_FILTER_QUESTION_REQUEST':
            return {
                ...state,
                type: action.type,
                isLoading: true
            };
        case 'TAG_FILTER_QUESTION_RESPONSE':
            return {
                ...state,
                type: action.type,
                filterQuestions: action.filterQuestions,
                isLoading: false
            };
        default: 
            break;
    }

    return state;
};

