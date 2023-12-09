import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

interface IRequestData {
    declined: number[];
    approved: number[];
    open: number[];
}

interface SimpleLineChartProps {
    requestData?: IRequestData;
}

const SimpleLineChart: React.FC<SimpleLineChartProps> = ({ requestData }) => {
    // Sample data structure
    const data = requestData ? requestData.declined.map((item, index) => ({
        name: `Day ${index + 1}`,
        Declined: requestData.declined[index],
        Approved: requestData.approved[index],
        Open: requestData.open[index]
    })) : [];

    return (
        <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="Declined" stroke="#8884d8" />
                <Line type="monotone" dataKey="Approved" stroke="#82ca9d" />
                <Line type="monotone" dataKey="Open" stroke="#ffc658" />
            </LineChart>
        </ResponsiveContainer>
    );
};

export default SimpleLineChart;
