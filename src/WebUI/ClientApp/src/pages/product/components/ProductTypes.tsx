import { Checkbox, Grid, List, ListItem, ListItemIcon } from "@material-ui/core";
import React from "react";
import { colorOnIndex } from "../../../i18n/ColorValues";
import IProductCardType from "../../../interfaces/product/IProductCardType";
import IProductType from "../../../interfaces/product/IProductType";
import ProductTypesStyle from "./ProductTypesStyle";

interface IProps {
    currentType: IProductType|undefined;
    setCurrentType: (currentType:IProductType) => void;
    orderTypes: IProductType[];
    setImage: (image: string) => void;
}

export default ({currentType, setCurrentType, orderTypes, setImage}:IProps) => {
    const classes = ProductTypesStyle();

    const onClick = (item:IProductType) => {
        setCurrentType(item); 
        setImage(item.image);
    }

    return <>
        <List className={classes.root}>
            {orderTypes.map((productType:IProductType, index:number) => 
                <ListItem 
                    key={`checkbox-list-label-${index}`}
                    onClick={() => onClick(orderTypes[index])}
                    role={undefined} 
                    dense 
                    button 
                >
                    <ListItemIcon>
                        <Checkbox
                            edge="start"
                            tabIndex={-1}
                            disableRipple
                            checked={currentType ? (currentType.title === productType.title) : false}
                            style={{color:colorOnIndex(index)}}
                        />
                    </ListItemIcon>
                    <Grid container>
                        <Grid item xs={8} className={classes.align_left}>
                            {productType.title}
                        </Grid>
                        <Grid item xs={4} className={classes.align_right}>
                            <b>€{Number(productType.price).toFixed(2)}</b>
                        </Grid>
                    </Grid>
                </ListItem>
            )}
        </List>
    </>
}