import { makeStyles } from "@material-ui/core";

export default makeStyles(() => ({
    total_height: {
        height:"100%"
    },
    // cart_box: {
    //     // width:"390px"
    // },
    delivery_box: {
        bottom: "0%",
        position: "fixed",
        backgroundColor:"#FFFFFF"
    },
    delivery_button:{
        margin:"20px", 
        width:"-webkit-fill-available", 
        color:"black", 
        borderColor:"black"
    },
    align_right: { 
        textAlign:"right",
        // marginLeft:"10px",
        // marginRight:"10px"
    },
    margin_left: {
        marginLeft:"20px"
    }
}));