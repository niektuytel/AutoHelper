import React from 'react';
import { List, ListItem, ListItemText, Paper } from '@mui/material';

interface IServiceLog {
    id: string;
    date: string;
    description: string;
    // other relevant fields
}

interface ServiceLogListProps {
    logs: IServiceLog[];
}

const ServiceLogList: React.FC<ServiceLogListProps> = ({ logs }) => {
    return (
        <Paper style={{ maxHeight: 200, overflow: 'auto' }}>
            <List>
                {logs.map(log => (
                    <ListItem key={log.id} divider>
                        <ListItemText primary={log.description} secondary={`Date: ${log.date}`} />
                    </ListItem>
                ))}
            </List>
        </Paper>
    );
};

export default ServiceLogList;
