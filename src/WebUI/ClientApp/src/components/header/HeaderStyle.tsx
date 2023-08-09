import { makeStyles, Theme } from "@material-ui/core";
import { colorOnIndex } from "../../i18n/ColorValues";

export default makeStyles((theme: Theme) => ({
    marginLeft16: {
        marginLeft: "16px"
    },
    margin_5: { 
        margin:"5px" 
    },
    chip: {
      margin: theme.spacing(0.5),
    },
    headerHeight:{
        margin:"75px"
    },
    appbar: {
        background:"white"
    },
    toolbar: {
        minHeight:"64px",
        margin: "0",
        padding: "0"
    },
    container:{
        padding:0
    },
    centerVertically: {
        display: 'flex',
        alignItems: 'center',
    },
    iconGrid: {
        textAlign:"right", 
        color:"black"
    },
    icon: {
        color: "black"
    },
    badge: {
        backgroundColor: colorOnIndex(0),
        color:"white"
    }
}));
