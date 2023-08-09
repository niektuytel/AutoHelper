import { makeStyles } from '@material-ui/core';

export default makeStyles(() => ({
    container: {
        marginBottom:"100px",
        minHeight:"70vh"
    },
    paper: {
        padding:"15px"
    },
    btn_box: { 
        display: 'flex', 
        justifyContent: 'flex-end', 
        paddingTop:"5px" 
    },
    btn_back: { 
        marginTop: 1, 
        marginLeft: 1 
    },
    btn_next: { 
        marginTop: 1, 
        marginLeft: 1 
    }
}));
