import * as React from 'react';
import clsx from 'clsx';
import { useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import LocalShippingIcon from '@material-ui/icons/LocalShipping';
import { IconButton, Toolbar, Tooltip, Typography } from '@material-ui/core';

import TableToolBarStyle from './OrderTableToolBarStyle';
import { httpPutShippingOrders } from '../../../services/OrderService';
import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import IOrder from '../../../interfaces/IOrder';

interface IProps {
    numSelected: number;
    selectedOrders: IOrder[];
    setSelectedOrders: (indexes: number[]) => void;
    orders: IOrder[];
    setOrders: (data: IOrder[]) => void; 
}

export default ({ numSelected, selectedOrders, setSelectedOrders, orders, setOrders }: IProps) => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const classes = TableToolBarStyle();

    const deliverOrders = () => () => {
        let orderIds:string[] = selectedOrders.map((order:IOrder) => order.id);
        httpPutShippingOrders(orderIds, 
            (response:string[]) => {
                if (response.length > 0)
                {
                    // change status on orders 
                    let newOrders = orders.map((order:IOrder) => response.includes(order.id) ? {...order, status:"delivered"} : order);
                    setOrders(newOrders);
                    setSelectedOrders([]);
                    
                    dispatch(setSuccessStatus("Return response message that succeeded"));
                }
                else
                {
                    dispatch(setErrorStatus("Return response message that failed"));
                }
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        )
    }
    
    return (
        <Toolbar className={clsx(classes.root, {
            [classes.highlight]: numSelected > 0,
        })}>
            {numSelected > 0 ?
                <Typography className={classes.title} color="inherit" variant="subtitle1" component="div">
                    {numSelected} {t("selected")}
                </Typography>
            :
                <Typography className={classes.title} variant="h6" id="tableTitle" component="div">
                    {t("orders")}
                </Typography>
            }
            {numSelected > 0 &&
                <IconButton aria-label="delivered" onClick={deliverOrders()}>
                    <LocalShippingIcon style={{color:"black"}}/>
                </IconButton>
            }
        </Toolbar>
    );
  };
  