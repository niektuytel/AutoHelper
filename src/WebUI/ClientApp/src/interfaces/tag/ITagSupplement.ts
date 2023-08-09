import ITag, { emptyTag } from "./ITag";

export const emptyTagSupplement:ITagSupplement = {
    id: 0,
    micrograms: 0,
    tag: emptyTag
}

export default interface ITagSupplement {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;

    /** 
     * The Weight of this supplement. [Based on a product weight of 1 gram]
    */
    micrograms: number;

    /** 
     * The tag what represents this supplement.
    */
    tag: ITag;
}









