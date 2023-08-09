import { makeStyles } from '@material-ui/core';

export default makeStyles(() => ({
    panel:{ 
        minHeight:"80vh"
    },
    container:{ 
        justifyContent: "center" 
    },
    paper: {
        padding:"15px",
        display: 'flex',
        flexDirection: 'column',
        height: "100%",
    },
    control_button:{
        textAlign:"right", 
        marginTop:"10px"
    }
    
}))