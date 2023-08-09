import React from "react"
import { makeStyles } from "@material-ui/core";
import CustomPagination from "./SmallPaginator";

const useStyles = makeStyles(() => ({
    root:{
        paddingTop: 10,
        margin: "auto",
        display: "flex",
        outline: "0",
        position: "relative",
        justifyContent: "center"
    }
}));

interface IProps {
    sortOnTags: string[];
    sortOnPrice: boolean;
    sortOnPopularity:boolean;
    pageNumber:number;
    setPageNumber: (page:number) => void;
    hasPrev:boolean|undefined;
    hasNext:boolean|undefined;
}

export default ({sortOnTags, sortOnPopularity, sortOnPrice, pageNumber, setPageNumber, hasPrev, hasNext}:IProps) => {
    const classes = useStyles();
    return <>
        <div className={classes.root}>
            <CustomPagination 
                sortOnTags={sortOnTags} 
                sortOnPrice={sortOnPrice} 
                sortOnPopularity={sortOnPopularity} 
                pageNumber={pageNumber} 
                setPageNumber={setPageNumber}
                hasPrev={hasPrev}
                hasNext={hasNext}
            />
        </div>
    </>
}