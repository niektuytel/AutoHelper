import { CSSProperties } from "react";
import { colorOnIndex } from "../../i18n/ColorValues";

export default (index:number): CSSProperties => ({ 
    borderColor:colorOnIndex(index), 
    color:"#000", 
    borderRadius: "5px" ,
    marginLeft:"1px"
});

