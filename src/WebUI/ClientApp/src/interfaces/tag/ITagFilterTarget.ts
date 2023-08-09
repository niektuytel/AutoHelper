import ITag, { emptyTag } from "./ITag";
import ITagFilter from "./ITagFilter";
import ITagSupplement from "./ITagSupplement";

export const emptyFilterTarget:ITagFilterTarget = {
    id: 0,
    title: "",
    description: "",
    gender: "",
    age: 0,
    supplements: []
}

export default interface ITagFilterTarget {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Target title
    */
    title: string;

    /**
     * Describe target with broughter explanation
    */
    description: string;
    
    /**
     * The Gender [Man, Woman]
    */
    gender: string;

    /**
     * Group out different stadium, based on the age
    */
    age: number;

    /**
     * All needed supplements for a healthy/idealistic target for the specific grouping
    */
    supplements: ITagSupplement[];
}



