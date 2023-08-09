import { makeStyles } from "@material-ui/core";

export default makeStyles(() => ({
    product: {
        width: 300
    },
    media: {
        height: 250,
    },
    progress_parent:{
        textAlign:"center",
        padding:"100px"
    },
    progress : {
        color:"black"
    },
    price_to_left: {
        textAlign:"left", 
        paddingLeft:"10px"
    },
    btn_to_right: {
        textAlign:"right" 
    },
    margin_left:{
        marginLeft:"10px"
    },
    card: {
        "&:hover": { 
            cursor: "pointer"
        }
    }
}));
