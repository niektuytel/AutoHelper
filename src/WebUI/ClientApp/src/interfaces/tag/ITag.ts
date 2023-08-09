
export const emptyTag:ITag = {
    id: 0,
    name: "",
    scientificName: "",
    description: "",
    importanceRate: 0,
    hasResource: false
}

export default interface ITag {
    /**
     * Id that represents parent POCO identifier. 
     */
    id: number;

    /**
     * Tag name
     */
    name: string;
    
    /**
     * Scientific name
     */
    scientificName: string;

    /**
     * Description on the title
     */
    description: string;

    /**
     * (low)0 <-> 255(high) is the importance scale.
     * Low importance is that we have enough in a daily day.
     * High importance is that we have a shortage in a daily day.
     */
    importanceRate: number;

    /**
     * When it has resources, Than there will be a button on the description that redirect to a page with 
     * more detailed information about this title.
     */
    hasResource: boolean;
}