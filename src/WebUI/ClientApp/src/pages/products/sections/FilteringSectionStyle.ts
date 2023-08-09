import { makeStyles } from "@material-ui/core";

export default makeStyles((theme) => ({
    paper: {
        padding: "5px",
        textAlign: 'center',
        width: "100%",
        color:"black"
    },
    paper_card: {
      '&:hover': {
        cursor: "pointer"
      },
      padding: theme.spacing(2),
      margin: "10px",
      backgroundColor: "#f3f3f3",
      textAlign: 'center',
    },
    btn_box: {
        textAlign: "right"
    },
    btn: {
        margin:"3px"
    },
    title_box:{
        margin:"10px"
    },
    instructions: {
        display: "inline-table",
        marginTop: theme.spacing(1),
        marginBottom: theme.spacing(1),
    },
    info_icon: {
        '&:hover': {
            cursor: "pointer"
        },
        verticalAlign: "top",
        padding:"3px"
    },
    typography: {
        padding: theme.spacing(2),
    }
}));
