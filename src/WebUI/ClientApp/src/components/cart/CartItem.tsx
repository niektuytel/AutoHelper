import React from "react";
import { Box, Button, ButtonBase, ButtonGroup, Grid, IconButton, Paper, Typography } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import AddIcon from '@mui/icons-material/Add';
import RemoveIcon from '@mui/icons-material/Remove';
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";

import { ICookieProduct } from "../../interfaces/ICookieProduct";
// import { removeCartItem, updateCartItem } from "../../store/cart/CartActions";
//import CartItemStyle from "./CartItemStyle";
import AdvancedTags from "../tags/TagItems";


export const styles = {
    root: {
        flexGrow: 1,
    },
    paper: {
        margin: "10px",
        padding: "3px",
        borderRadius: "0px"
    },
    align_right: {
        marginRight: "7px",
        width: "100%",
        textAlign: "right"
    },
    image: {
        width: 128,
        height: 128,
    },
    img: {
        margin: 'auto',
        display: 'block',
        maxWidth: '100%',
        maxHeight: '100%',
    },
    float: {
        float: "right"
    },
    margin_top: {
        marginTop: "10px"
    }
}
interface IProps {
    item:ICookieProduct;
    show_text?: boolean;
    show_tags?: boolean;
    show_description?: boolean;
    controllable?: boolean;
}

export default ({ item, show_text, show_tags, show_description, controllable }: IProps) => {
    // we are now using the styles object instead of the makeStyles function
    //const classes = CartItemStyle();
    // const dispatch = use// dispatch();
    const navigate = useNavigate();
    
    const onRemoveItem = () => {
        // dispatch(removeCartItem(item));
    }
    
    const increaseQuantity = () => {
        // dispatch(updateCartItem({...item, quantity:(item.quantity+1)}));
    }
    
    const decreaseQuantity = () => {
        if(item.quantity > 1)
        {
            // dispatch(updateCartItem({...item, quantity:(item.quantity-1)}));
        }
        else
        {
            // dispatch(removeCartItem(item));
        }
    }

    return <>
        <Paper elevation={1} sx={styles.paper}>
            <Grid container spacing={2}>
                <Grid item>
                    {controllable ? 
                        <ButtonBase onClick={() => navigate(`/product/${item.id}`)} sx={styles.image}>
                            <img src={item.currentType.image} style={styles.img} alt={item.currentType.image}/>
                        </ButtonBase>
                        :
                        <Box sx={styles.image}>
                            <img src={item.currentType.image} style={styles.img} alt={item.currentType.image}/>
                        </Box>
                    }
                </Grid>
                <Grid item xs container>
                    <Grid item container>
                        <Grid item xs={9}>
                            {show_text && 
                                <Typography gutterBottom variant="subtitle1" sx={styles.margin_top}>
                                    {item.currentType.title}
                                </Typography>
                            }
                        </Grid>
                        <Grid item xs={3}>
                            {controllable && 
                                <IconButton onClick={onRemoveItem} sx={styles.float}>
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
                    <Box sx={styles.align_right}>
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

