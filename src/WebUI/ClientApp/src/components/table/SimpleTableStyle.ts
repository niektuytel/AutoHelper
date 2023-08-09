import { createStyles, makeStyles, TableRow, Theme, withStyles } from "@material-ui/core";

export const StyledTableRow = withStyles((theme: Theme) =>
    createStyles({
        root: {
            '&:nth-of-type(odd)': {
            backgroundColor: theme.palette.action.hover,
            },
        },
    }),
)(TableRow);

export const UseStyles = makeStyles((theme: Theme) => ({
    table: {
        width: "100%",
    }
}));
