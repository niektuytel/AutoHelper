import ITag from "./ITag";
import ITagFilter from "./ITagFilter";

export const emptyTagFilterSituation:ITagFilterSituation = {
    id: 0,
    filterTargetId: 0,
    filterId: 0,
    nextFilterId: 0,
    text: "",
    missingItems: []
}

export default interface ITagFilterSituation {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;

    /**
     * Id of TagFilterTarget entity, used to know his specific target.
    */
    filterTargetId: number;
    
    /**
     * Id of TagFilter entity, used to know his specific filter.
     * Represent possible situation matching under de filter.
    */
    filterId: number;
    
    /**
     * Id of TagFilter entity, used to know next specific filter.
     * The next filter that is the most related on this situation.
     * 
     * This value will been defined bij the backend Bot...
    */
    nextFilterId: number;
    
    /**
     * Info that is representing a situation
    */
    text: string;
    
    /**
     * The situation has missing supplements that can infect/increase the important supplements
    */
    missingItems: ITag[];
}
