
import { AppThunkAction } from '..';
import ITagFilterSituation from '../../interfaces/tag/ITagFilterSituation';
import ITagFilterTarget from '../../interfaces/tag/ITagFilterTarget';

interface SetFilteringTargetAction {
    type: 'SET_FILTERING_TARGET';
    target: ITagFilterTarget;
}

interface SetCurrentStepAction {
    type: 'SET_CURRENT_STEP';
    currentStep: number;
}

interface SetCompletedStepsAction {
    type: 'SET_COMPLETED_STEPS';
    completedSteps: { [k: number]: boolean };
}

interface SetUsedFilterSituationsAction {
    type: 'SET_USED_FILTER_SITUATIONS';
    usedSituations: { [k: number]: ITagFilterSituation };
}

export type KnownFilterSituationAction = 
    SetFilteringTargetAction | 
    SetCurrentStepAction | 
    SetCompletedStepsAction | 
    SetUsedFilterSituationsAction;

export const setFilteringTarget = (target:ITagFilterTarget): AppThunkAction<KnownFilterSituationAction> => (dispatch) => {
    dispatch({type:"SET_FILTERING_TARGET", target:target});
};

export const setCurrentStep = (index:number): AppThunkAction<KnownFilterSituationAction> => (dispatch) => {
    dispatch({type:"SET_CURRENT_STEP", currentStep:index});
};

export const setCompletedSteps = (steps:{ [k: number]: boolean }): AppThunkAction<KnownFilterSituationAction> => (dispatch) => {
    dispatch({type:"SET_COMPLETED_STEPS", completedSteps:steps});
};

export const setUsedFilterSituation = (situations:{ [k: number]: ITagFilterSituation }): AppThunkAction<KnownFilterSituationAction> => (dispatch) => {
    dispatch({type:"SET_USED_FILTER_SITUATIONS", usedSituations:situations});
};
