import * as React from 'react';
import { Checkbox, makeStyles, TableCell, TableHead, TableRow, Theme, Typography } from '@material-ui/core';

export interface Data {
  address: string;
  products: string;
  action: string;
}

export type Order = 'asc' | 'desc';

export interface HeadCell {
  disablePadding: boolean;
  id: keyof Data;
  label: string;
  numeric: boolean;
}

export const headCells: HeadCell[] = [
  { id: 'address', numeric: false, disablePadding: true, label: 'Address' },
  { id: 'products', numeric: false, disablePadding: false, label: 'Products' },
  { id: 'action', numeric: false, disablePadding: false, label: 'Action' },
];

export const TableHeadStyle = makeStyles((theme: Theme) => ({
    root: {
      width: '100%',
    },
    paper: {
      width: '100%',
      marginBottom: theme.spacing(2),
    },
    paper_2: { padding: 2, display: 'flex', flexDirection: 'column' },
    table: {
      minWidth: 750,
    },
    visuallyHidden: {
      border: 0,
      clip: 'rect(0 0 0 0)',
      height: 1,
      margin: -1,
      overflow: 'hidden',
      padding: 0,
      position: 'absolute',
      top: 20,
      width: 1,
    },
}));

interface IProps {
    numSelected: number;
    onSelectAllClick: (event: React.ChangeEvent<HTMLInputElement>) => void;
    rowCount: number;
}

export default ({ 
    numSelected,
    onSelectAllClick,
    rowCount
}: IProps) => {
    return <TableHead>
        <TableRow>
            <TableCell padding="checkbox">
                <Checkbox
                    indeterminate={numSelected > 0 && numSelected < rowCount}
                    checked={rowCount > 0 && numSelected === rowCount}
                    onChange={onSelectAllClick}
                    inputProps={{ 'aria-label': 'select all desserts' }}
                    style={{color:"black"}}
                />
            </TableCell>
            <TableCell align='left' padding='normal'>
                <Typography>
                    Address
                </Typography>
            </TableCell>
            <TableCell align='left' padding='normal'>
                <Typography>
                    Products
                </Typography>
                {/* <TableSortLabel onClick={() => updateOrders(!sortProducts, sortDate, includeDelivery)}>
                    Products
                </TableSortLabel> */}
            </TableCell>
            <TableCell align='left' padding='normal'>
                <Typography>
                    Action
                </Typography>
            </TableCell>
        </TableRow>
    </TableHead>
}
