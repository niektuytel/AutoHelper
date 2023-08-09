import React from "react";
import { FormControl, Grid, MenuItem, OutlinedInput, Paper, Select } from "@material-ui/core";
import CustomPagination from "../components/SmallPaginator";
import { useTranslation } from "react-i18next";
import { requestCardProducts } from "../../../store/products/ProductsActions";
import { useDispatch } from "react-redux";
import PersonalSortingSectionStyle from "./SortingSectionStyle";

interface IProps {
    sortOnTags: string[];
    sortOnPrice: boolean;
    setSortOnPrice: (value:boolean) => void;
    sortOnPopularity:boolean;
    setSortOnPopularity: (value:boolean) => void;
    pageNumber:number;
    setPageNumber: (page:number) => void;
    hasPrev: boolean|undefined;
    hasNext: boolean|undefined;
}

export default ({sortOnTags, sortOnPopularity, setSortOnPopularity, sortOnPrice, setSortOnPrice, pageNumber, setPageNumber, hasPrev, hasNext}:IProps) => {
    const { t } = useTranslation();
    const classes = PersonalSortingSectionStyle();
    const dispatch = useDispatch();

    const onSortChange = (value:string) => {
        if(value == t("popular"))
        {
            setSortOnPopularity(true);
            setSortOnPrice(false);
            dispatch(requestCardProducts("TODO: Admin Azure account ID from azure!!!", pageNumber, sortOnTags, true, false));
        }
        else if(value == t("price"))
        {
            setSortOnPopularity(false);
            setSortOnPrice(true);
            dispatch(requestCardProducts("TODO: Admin Azure account ID from azure!!!", pageNumber, sortOnTags, false, true));
        }
    }

    return <>
        <Paper 
            elevation={0} 
            className={classes.paper}
            style={{ marginBottom:"5px" }}
        >
            <FormControl fullWidth>
                <Grid container>
                    <Grid item xs className={classes.align_left}>
                        <Select
                            classes={{ root: classes.rootFirstSelect}}
                            input={<OutlinedInput/>}
                            value={sortOnPrice ? t("price") : t("popular")}
                            onChange={(e) => onSortChange(e.target.value as string)}
                        >
                            <MenuItem value={t("popular")}>{t("popular")}</MenuItem>
                            <MenuItem value={t("price")}>{t("price")}</MenuItem>
                        </Select>
                    </Grid>
                    <Grid item xs={6} className={classes.align_center}>
                        {/* empty field for now */}
                    </Grid>
                    <CustomPagination 
                        sortOnTags={sortOnTags} 
                        sortOnPrice={sortOnPrice} 
                        sortOnPopularity={sortOnPopularity} 
                        pageNumber={pageNumber} 
                        setPageNumber={setPageNumber}
                        hasPrev={hasPrev}
                        hasNext={hasNext}
                    />
                </Grid>
            </FormControl>
        </Paper>
    </>
}