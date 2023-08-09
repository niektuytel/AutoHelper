
import { AppThunkAction } from '..';
import ITagFilter from '../../interfaces/tag/ITagFilter';
import ITagFilterSituation from '../../interfaces/tag/ITagFilterSituation';
import ITagFilterTarget from '../../interfaces/tag/ITagFilterTarget';
import ITagFilterWithSituations from '../../interfaces/tag/ITagFilterWithSituations';
import { httpGetTagFilters, httpGetTagFilterTargets, httpGetTagNextFilters } from '../../services/TagFilterService';
import { KnownStatusAction } from '../status/StatusActions';

interface TagFiltersRequestAction {
    type: 'TAG_FILTERS_REQUEST';
}

interface TagFiltersResponseAction {
    type: 'TAG_FILTERS_RESPONSE';
    filters: ITagFilter[];
}

interface TagFilterTargetRequestAction {
    type: 'TAG_FILTER_TARGET_REQUEST';
}

interface TagFilterTargetResponseAction {
    type: 'TAG_FILTER_TARGET_RESPONSE';
    targets: ITagFilterTarget[];
}

interface TagFilterQuestionRequestAction {
    type: 'TAG_FILTER_QUESTION_REQUEST';
}

interface TagFilterQuestionResponseAction {
    type: 'TAG_FILTER_QUESTION_RESPONSE';
    filterQuestions: ITagFilterWithSituations[];
}

export type KnownTagFilterAction = 
    TagFiltersRequestAction | 
    TagFiltersResponseAction | 
    TagFilterTargetRequestAction | 
    TagFilterTargetResponseAction | 
    TagFilterQuestionRequestAction | 
    TagFilterQuestionResponseAction;
    
export const getFilterTargets = (): AppThunkAction<KnownTagFilterAction|KnownStatusAction> => 
(dispatch) => {
    dispatch({ type: 'TAG_FILTER_TARGET_REQUEST' });
    httpGetTagFilterTargets(
        (data:ITagFilterTarget[]) => {
            dispatch({ type: 'TAG_FILTER_TARGET_RESPONSE', targets: data });
        },
        (message:string) => {
            dispatch({ type: 'SET_ERROR_STATUS', error_message: message });
        }
    );
};

export const getNextFilters = (target:ITagFilterTarget, situations:ITagFilterSituation[]): AppThunkAction<KnownTagFilterAction|KnownStatusAction> => 
(dispatch) => {
    dispatch({ type: 'TAG_FILTER_QUESTION_REQUEST' });
    httpGetTagNextFilters(
        target, 
        situations,
        (data:ITagFilterWithSituations[]) => {
            dispatch({ type: 'TAG_FILTER_QUESTION_RESPONSE', filterQuestions: data });
        },
        (message:string) => {
            dispatch({ type: 'SET_ERROR_STATUS', error_message: message });
        }
    );
};

export const getFilters = (): AppThunkAction<KnownTagFilterAction|KnownStatusAction> => 
    (dispatch) => {
        dispatch({ type: 'TAG_FILTERS_REQUEST' });
        httpGetTagFilters(
            (data:ITagFilter[]) => {
                dispatch({ type: 'TAG_FILTERS_RESPONSE', filters: data });
            },
            (message:string) => {
                dispatch({ type: 'SET_ERROR_STATUS', error_message: message });
            }
        );
    };

export const setFilters = (data:ITagFilter[]): AppThunkAction<KnownTagFilterAction|KnownStatusAction> => 
    (dispatch) => {
        dispatch({ type: 'TAG_FILTERS_RESPONSE', filters: data });
    };




    
