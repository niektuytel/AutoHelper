import ITagFilter from "./ITagFilter";
import ITagFilterSituation from "./ITagFilterSituation";

export const tagFilterWithSituations:ITagFilterWithSituations = {
    id: 0,
    filterTargetId: 0,
    title: "",
    situations: []
}

export default interface ITagFilterWithSituations extends ITagFilter {
    
    /**
     * Represented situations on this filters connected to the Filter target type.
    */
    situations: ITagFilterSituation[];
}
