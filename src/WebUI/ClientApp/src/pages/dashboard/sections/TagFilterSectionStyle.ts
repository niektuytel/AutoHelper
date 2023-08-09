import { makeStyles } from '@material-ui/core';

export default makeStyles(() => ({
    root: {
        flexGrow: 1
    },
    question:{
        padding: "10px",
        marginBottom:"20px",
        display: 'flex',
        flexDirection: 'column',
        textAlign:"center"
    },
    list_paper: {
        padding: 2,
        display: 'flex',
        flexDirection: 'column',
        height: "100%",
    },
    paper:{
        padding: "10px",
        marginBottom:"20px",
        display: 'flex',
        flexDirection: 'column',
        textAlign:"center"
    },
    transfer_tags_paper:{ 
        margin: 2, 
        display: 'flex', 
        flexDirection: 'column' 
    },
    control_button:{
        textAlign:"right", 
        marginTop:"10px"
    }
}));
