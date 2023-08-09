import ITagFilterSituation from "../../interfaces/tag/ITagFilterSituation";
import ITagFilterTarget from "../../interfaces/tag/ITagFilterTarget";

export default interface ActiveSituationState {
    type: string|undefined;
    target: ITagFilterTarget|undefined;
    currentStep: number;
    completedSteps: { [k: number]: boolean };
    usedSituations: { [k: number]: ITagFilterSituation };
}