import React from "react";
import { Box, Button, ButtonBase, ButtonGroup, Grid, IconButton, Paper, Typography } from "@material-ui/core";
import CloseIcon from '@material-ui/icons/Close';
import AddIcon from '@material-ui/icons/Add';
import RemoveIcon from '@material-ui/icons/Remove';
import { useDispatch } from "react-redux";
import { useHistory } from "react-router";

import { ICookieProduct } from "../../interfaces/ICookieProduct";
import { removeCartItem, updateCartItem } from "../../store/cart/CartActions";
import CartItemStyle from "./CartItemStyle";
import AdvancedTags from "../tags/TagItems";

interface IProps {
    item:ICookieProduct;
    show_text?: boolean;
    show_tags?: boolean;
    show_description?: boolean;
    controllable?: boolean;
}

export default ({item, show_text, show_tags, show_description, controllable}:IProps) => {
    const classes = CartItemStyle();
    const dispatch = useDispatch();
    const history = useHistory();
    
    const onRemoveItem = () => {
        dispatch(removeCartItem(item));
    }
    
    const increaseQuantity = () => {
        dispatch(updateCartItem({...item, quantity:(item.quantity+1)}));
    }
    
    const decreaseQuantity = () => {
        if(item.quantity > 1)
        {
            dispatch(updateCartItem({...item, quantity:(item.quantity-1)}));
        }
        else
        {
            dispatch(removeCartItem(item));
        }
    }

    return <>
        <Paper elevation={1} className={classes.paper}>
            <Grid container spacing={2}>
                <Grid item>
                    {controllable ? 
                        <ButtonBase onClick={() => history.push(`/product/${item.id}`)} className={classes.image}>
                            <img src={item.currentType.image} className={classes.img} alt={item.currentType.image}/>
                        </ButtonBase>
                        :
                        <Box className={classes.image}>
                            <img src={item.currentType.image} className={classes.img} alt={item.currentType.image}/>
                        </Box>
                    }
                </Grid>
                <Grid item xs container>
                    <Grid item container>
                        <Grid item xs={9}>
                            {show_text && 
                                <Typography gutterBottom variant="subtitle1" className={classes.margin_top}>
                                    {item.currentType.title}
                                </Typography>
                            }
                        </Grid>
                        <Grid item xs={3}>
                            {controllable && 
                                <IconButton onClick={onRemoveItem} className={classes.float}>
                                    <CloseIcon/>
                                </IconButton>
                            }
                        </Grid>
                    </Grid>
                    {show_tags &&
                        <AdvancedTags data={item.supplements}/>
                    }
                    {show_description &&
                        <Typography variant="body2" color="textSecondary">
                            {item.description}
                        </Typography>
                    }
                    <Box className={classes.align_right}>
                        <Typography variant="subtitle1">
                            {item.quantity} X €{item.currentType.price.toFixed(2)}
                        </Typography>
                        {controllable &&
                            <ButtonGroup disableElevation variant="outlined">
                                <Button onClick={decreaseQuantity}><RemoveIcon/></Button>
                                <Button onClick={increaseQuantity}><AddIcon/></Button>
                            </ButtonGroup>
                        }
                    </Box>
                </Grid>
            </Grid>
        </Paper>
    </>
}

