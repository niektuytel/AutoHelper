import ITagFilterSituation from "./ITagFilterSituation";

export const emptyTagFilter:ITagFilter = {
    id: 0,
    filterTargetId: 0,
    title: ""
}

export default interface ITagFilter {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Id that represents The connected Filter Target Id.
    */
    filterTargetId: number;
    
    /**
     * A global view that represents multiplied situations.
    */
    title: string;
}
