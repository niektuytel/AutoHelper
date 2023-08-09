import { makeStyles, Theme } from "@material-ui/core";

export default makeStyles((theme: Theme) => ({
    chips: {
        display: 'flex',
        justifyContent: 'center',
        flexWrap: 'wrap',
        marginBottom:"2px",
        '& > *': {
          margin: theme.spacing(0.1),
        }
    },
    chips_ellipsis: {
        display: "-webkit-box",
        lineClamp: 2,
        boxOrient:"vertical",
        overflow: "hidden",
        textOverflow: "ellipsis",
    },
    admin_chip: { 
        backgroundColor:"rgb(223 255 212)",
        borderColor:"rgb(223 255 212)", 
        color:"#000", 
        borderRadius: "5px" 
    }
}));
