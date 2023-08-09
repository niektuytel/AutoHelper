import { Box, Button, ButtonBase, Grid, Paper, TextField, Typography } from "@material-ui/core";
import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useHistory } from "react-router";
import CartItemStyle from "../../../components/cart/CartItemStyle";
import TagsSection from "../../../components/tags/TagsSection";
import SaveIcon from '@material-ui/icons/Save';
import { httpPutProductTypeStock } from "../../../services/ProductService";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";
import IProductType from "../../../interfaces/product/IProductType";
import ITagSupplement from "../../../interfaces/tag/ITagSupplement";

var defualt_image = "https://i.vimeocdn.com/portrait/1274237_300x300.jpg";

interface IProps {
    productType:IProductType;
    productId: number;
    title: string;
    supplements: ITagSupplement[];
}

export default ({productType, productId, title, supplements: tags}:IProps) => {
    const classes = CartItemStyle();
    const dispatch = useDispatch();
    const history = useHistory();
    const [stockAmount, setStockAmount] = useState(productType.stock.stockAmount);
    
    const saveStockValue = () => {
        httpPutProductTypeStock(
            String(productId), 
            String(productType.id), 
            {...productType.stock, stockAmount:stockAmount},
            (id) => {
                dispatch(setSuccessStatus(`success on ${id}`));
            },
            (message) => {
                dispatch(setErrorStatus(message));
            }
        );
    }

    const onChange = (event:any) => {
        setStockAmount(Math.max(event.target.value as number, 1))
    }

    return <>
        <Paper elevation={3} style={{margin:"20px", padding:"3px"}}>
            <Grid container spacing={2}>
                <Grid item>
                    <ButtonBase onClick={() => history.push(`/product/${productId}?type=${productType.id}`)} className={classes.image}>
                        <img 
                            className={classes.img} 
                            alt="complex" 
                            src={(productType && productType.image) ? productType.image : defualt_image}
                        />
                    </ButtonBase>
                </Grid>
                <Grid item xs={12} sm container>
                    <Grid item xs container direction="column" spacing={2}>
                        <Grid item xs>
                            <Typography gutterBottom variant="subtitle1">
                                {`${title} [${productType.title}]`} 
                            </Typography>
                        </Grid>
                        <Grid item xs>
                            <TagsSection productId={productId} default_data={tags} isAdmin={true}/>
                        </Grid>
                        <Grid item xs container>
                            <Grid item xs={12} sm={6}>
                                <Grid item xs container>
                                    <TextField
                                        id="outlined-number"
                                        label="Stock"
                                        type="number"
                                        InputLabelProps={{ shrink: true }}
                                        style={{marginLeft:"10px"}}
                                        variant="outlined"
                                        value={stockAmount}
                                        onChange={onChange}
                                    />
                                    <Box>
                                        <Button style={{ height: "100%", marginLeft:"10px" }} onClick={saveStockValue}>
                                            <SaveIcon/>
                                        </Button>
                                    </Box>
                                </Grid>
                                <Typography gutterBottom variant="subtitle1" style={{color: "gray", fontSize: 12, marginLeft:"10px"}}>
                                    {`Total Sold: ${productType.stock.totalSold}`}
                                </Typography>
                            </Grid>
                        </Grid>
                    </Grid>
                </Grid>
            </Grid>
        </Paper>
    </>
}

