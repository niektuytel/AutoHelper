import { createStyles, makeStyles } from '@material-ui/core';

export default makeStyles(() =>
    createStyles({
        dialog: {
            width:"400px"
        },
        width: {
            width:"100%"
        },
        inputLabel: {
            textAlign:"left"
        },
        selectBox: {
            width:"100%"
        },
        percentageBox: {
            margin:"30px", 
            textAlign:"center"
        }
    }),
);
