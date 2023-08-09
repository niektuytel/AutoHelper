

export const emptyInfoLine: IProductInfoLine = {
    id: 0,
    subject: "",
    value: ""
};

export default interface IProductInfoLine {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Main subject this information line is about
    */
    subject: string;
    
    /**
     * Information about this subject
    */
    value: string;
}