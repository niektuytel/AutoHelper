import ITagFilter from "../../interfaces/tag/ITagFilter";
import ITagFilterTarget from "../../interfaces/tag/ITagFilterTarget";
import ITagFilterWithSituations from "../../interfaces/tag/ITagFilterWithSituations";

export default interface TagFilterState {
    type: string | undefined;
    isLoading: boolean | undefined;
    filters: ITagFilter[] | undefined;
    filterQuestions: ITagFilterWithSituations[] | undefined;
    targets: ITagFilterTarget[] | undefined;
}