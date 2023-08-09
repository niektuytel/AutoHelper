import { createStyles, makeStyles } from "@material-ui/core";

export default makeStyles(() =>
    createStyles({
        root: {
            flexGrow: 1,
        },
        paper: {
            margin:"10px", 
            padding:"3px",
            borderRadius: "0px"
        },
        align_right: {
            marginRight:"7px", 
            width:"100%", 
            textAlign:"right"
        },
        image: {
            width: 128,
            height: 128,
        },
        img: {
            margin: 'auto',
            display: 'block',
            maxWidth: '100%',
            maxHeight: '100%',
        },
        float: {
            float:"right"
        },
        margin_top: {
            marginTop:"10px"
        }
    }),
);
