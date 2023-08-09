import { makeStyles } from "@material-ui/core";

export default makeStyles(() => ({
    paper:{
        padding: "10px",
        marginBottom:"20px",
        display: 'flex',
        flexDirection: 'column',
        textAlign:"center"
    },
    text_field:{
        width:"100%"
    },
    control_button:{
        textAlign:"right", 
        marginTop:"10px"
    }
}));
