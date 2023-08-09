import React from "react";
import { useTranslation } from "react-i18next";
import { List, ListItem, ListItemIcon, ListItemText } from "@material-ui/core";
import LocalShippingIcon from '@material-ui/icons/LocalShipping';

export default () => {
    const { t } = useTranslation();
    
    return <>
        <List>
            <ListItem dense>
                <ListItemIcon>
                    <LocalShippingIcon />
                </ListItemIcon>
                <ListItemText
                    primary={`€${process.env.REACT_APP_DELIVERY_PRICE} ${t("shipping_costs")}`}
                    secondary={`${t("from")} €${process.env.REACT_APP_FREE_DELIVERY_FROM} ${t("free_shipping")}`}
                />
            </ListItem>
        </List>
    </>
}


