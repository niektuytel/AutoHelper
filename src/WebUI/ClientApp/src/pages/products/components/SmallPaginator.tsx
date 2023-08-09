import Pagination from "@material-ui/lab/Pagination";
import React from "react";
import { useDispatch } from "react-redux";
import { requestCardProducts } from "../../../store/products/ProductsActions";

interface IProps {
    sortOnTags: string[];
    sortOnPrice: boolean;
    sortOnPopularity:boolean;
    pageNumber:number;
    setPageNumber: (page:number) => void;
    hasPrev: boolean|undefined;
    hasNext: boolean|undefined;
}

export default ({sortOnTags, sortOnPopularity, sortOnPrice, pageNumber, setPageNumber, hasPrev, hasNext}:IProps) => {
    const dispatch = useDispatch();

    const gotoPage = (event:any, page:number) => {
        setPageNumber(page);

        dispatch(requestCardProducts("TODO: Admin Azure account ID from azure!!!", page, sortOnTags, sortOnPopularity, sortOnPrice));
        console.log(event, page);
    }

    return <>
        <Pagination 
            count={pageNumber} 
            onChange={gotoPage}
            size="medium" 
            variant="outlined" 
            shape="rounded"
            hidePrevButton={!hasPrev}
            hideNextButton={!hasNext}
        />
        
    </>
}

