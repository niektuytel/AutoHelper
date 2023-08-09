import { makeStyles } from "@material-ui/core";

export default makeStyles(() => ({
    box: {
        textAlign: "center"
    },
    flexGrow: {
        flexGrow: 1
    },
    quantity_parent: {
        flexGrow: 1,
        width:"100%", 
        paddingRight:"5px", 
        textAlign:"right"
    },
    quantity_label: {
        display: "inline-flex", 
        marginTop:"2px", 
        marginRight:"5px"
    },
    quantity_input: {
        maxWidth:"50px", 
        textAlignLast:"center"
    },
    price: {
        paddingRight:"20px"
    },
}));



