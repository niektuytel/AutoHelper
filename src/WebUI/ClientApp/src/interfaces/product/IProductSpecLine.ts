

export const emptySpecLine: IProductSpecLine = {
    id: 0,
    subject: "",
    value: "",
    childOf: ""
};

export default interface IProductSpecLine {
    /**
     * Id that represents parent POCO identifier. 
    */
    id: number;
    
    /**
     * Specification name
    */
    subject: string;
    
    /**
     * The value inside this parent product subject
    */
    value: string;

    /**
     * Defines if is a extention of a other subject
     */
    childOf: string;
}