import * as React from 'react';
import { Box, Checkbox, Grid, Paper, Table, TableBody, TableContainer, TableRow, Typography } from '@material-ui/core';
import TableToolBar from '../components/OrderTableToolBar';
import TableHeader, { TableHeadStyle } from '../components/OrderTableHeader';
import { useEffect, useState } from 'react';
import IOrder from '../../../interfaces/IOrder';
import { httpGetAllOrders } from '../../../services/OrderService';
import { useDispatch } from 'react-redux';
import { setErrorStatus } from '../../../store/status/StatusActions';
import ProgressBox from '../../../components/progress/ProgressBox';
import { colorOnRow } from '../../../i18n/ColorValues';
import OrderTableActionCell from '../components/OrderTableActionCell';
import OrderTableDeliveryCell from '../components/OrderTableDeliveryCell';
import OrderTableProductsCell from '../components/OrderTableProductsCell';
import OrderTableCheckboxCell from '../components/OrderTableCheckboxCell';

export default () => {
    const classes = TableHeadStyle();
    const dispatch = useDispatch();
    const [isLoading, setLoading] = useState<boolean|undefined>(undefined);
    const [orders, setOrders] = useState<IOrder[]|undefined>(undefined);
    const [selected, setSelected] = useState<number[]>([]);
    const [includeDelivery, setIncludeDelivery] = useState<boolean>(false);

    // initialize
    useEffect(() => {
        if(orders === undefined && isLoading === undefined)
        {
            setLoading(true);
            httpGetAllOrders(includeDelivery,
                (response) => {
                    setOrders(response);
                    setLoading(false);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message))
                }
            )
        }  
    });

    const updateOrders = (includeDelivery: boolean) => {
        setIncludeDelivery(includeDelivery);
        httpGetAllOrders(includeDelivery, 
            (response:any) => {
                setOrders(response);
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
            }
        );
    }

    const handleSelectAllClick = (_: React.ChangeEvent<HTMLInputElement>) => {
        let items:number[] = [];
        if(!orders) return;

        orders.forEach((order:IOrder, index:number) => {
            if(order.status !== "delivered")
            {
                items.push(index);
            }
        });

        if(items.length === selected.length)
        {
            items = [];
        }
        setSelected(items);
    };

    const handleClick = (_: React.MouseEvent<unknown>, index: number) => {
        if(orders && orders[index].status === "delivered") 
        {
            return;
        }

        const selectedIndex = selected.indexOf(index);
        let newSelected: number[] = [];

        if (selectedIndex === -1) {
            newSelected = newSelected.concat(selected, index);
        } else if (selectedIndex === 0) {
            newSelected = newSelected.concat(selected.slice(1));
        } else if (selectedIndex === selected.length - 1) {
            newSelected = newSelected.concat(selected.slice(0, -1));
        } else if (selectedIndex > 0) {
            newSelected = newSelected.concat(
                selected.slice(0, selectedIndex),
                selected.slice(selectedIndex + 1),
            );
        }

        setSelected(newSelected);
    };

    const isSelected = (value: number) => selected.indexOf(value) !== -1;

    return <Grid container spacing={3}>
        <Grid item xs={12}>
            <Box>
                <Typography>
                    <Checkbox
                        checked={includeDelivery}
                        onChange={() => updateOrders(!includeDelivery)}
                        style={{color:"black"}}
                    />
                    Include delivered items
                </Typography>
            </Box>
            <Paper className={classes.paper_2}>
                {isLoading || !orders ?
                    <ProgressBox/>
                    :
                    <>
                        <TableToolBar 
                            numSelected={selected.length}
                            selectedOrders={orders ? orders.filter((_, index: number) => selected.includes(index)) : []} 
                            setSelectedOrders={setSelected} 
                            orders={orders} 
                            setOrders={setOrders}                
                        />
                        <TableContainer>
                            <Table className={classes.table} size='small'>
                                <TableHeader
                                    numSelected={selected.length}
                                    onSelectAllClick={handleSelectAllClick}
                                    rowCount={orders.length}
                                />
                                <TableBody>
                                    {orders.map((order:IOrder, index:number) => 
                                        <TableRow
                                            key={`order-${index}`}
                                            style={{cursor:"pointer", backgroundColor: colorOnRow(index), borderBottom:"0"}}
                                            onClick={(event) => handleClick(event, index)}
                                            aria-checked={isSelected(index)}
                                            selected={isSelected(index)}
                                            role="checkbox"
                                            hover
                                        >
                                            <OrderTableCheckboxCell status={order.status} checked={isSelected(index)}/>
                                            <OrderTableDeliveryCell order={order}/>
                                            <OrderTableProductsCell order={order}/>
                                            <OrderTableActionCell order={order}/>
                                        </TableRow>
                                    )}
                                </TableBody>
                            </Table>
                            {/* <LargePaginator 
                                sortOnTags={[]} 
                                sortOnPrice={false} 
                                sortOnPopularity={false} 
                                pageNumber={0} 
                                setPageNumber={
                                    function (page: number): void {
                                        throw new Error('Function not implemented.');
                                    } 
                                } 
                                hasPrev={undefined} 
                                hasNext={undefined}
                            /> */}
                        </TableContainer>
                    </>
                }
            </Paper>
        </Grid>
    </Grid>
}