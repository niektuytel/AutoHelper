import React, { useState, useRef } from 'react';
import { useSpring, animated } from 'react-spring';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { COLORS } from '../../../../constants/colors';
import NotificationDialog from './NotificationDialog';
import { Box, styled, TableCell } from '@mui/material';

interface NotificationIconProps {
    expiryDate: Date | undefined;
    isMobile: boolean;
}

export default function NotificationIcon({ expiryDate, isMobile }: NotificationIconProps) {
    const [showDialog, setShowDialog] = useState(false);
    const iconRef = useRef<SVGSVGElement>(null);

    const calculateIconColor = (expiryDate?: Date) => {
        if (expiryDate) {
            const expiry = new Date(expiryDate);
            const today = new Date();
            const diffDays = (expiry.getTime() - today.getTime()) / (1000 * 3600 * 24);
            const diffMonths = diffDays / 30;

            return diffMonths <= 2 ? 'red' : COLORS.BLUE;
        }
        return COLORS.BLUE;
    };

    const iconColor = calculateIconColor(expiryDate);

    // Inside your component
    const animationStyles = useSpring({
        to: async (next, cancel) => {
            while (true) {
                await next({ transform: 'rotate(10deg)' });
                await new Promise(resolve => setTimeout(resolve, 200)); // pause at 10deg
                await next({ transform: 'rotate(-10deg)' });
                await new Promise(resolve => setTimeout(resolve, 200)); // pause at -10deg
            }
        },
        from: { transform: 'rotate(-10deg)' },
        config: { duration: 200 },
        reset: true,
    });

    const handleMouseOver = () => iconRef.current?.style.setProperty('color', COLORS.BLUE);
    const handleMouseOut = () => iconRef.current?.style.setProperty('color', iconColor);
    const handleClick = () => setShowDialog(true);

    const ClickableTableCell = styled(TableCell)({
        cursor: 'pointer',
        '&:hover': {
            backgroundColor: '#f5f5f5',
        },
    });

    const cellStyle:any = {
        textAlign: 'left',
        paddingRight: isMobile ? '' : '0',
    };

    return (
        <>
            <ClickableTableCell
                key={`cell-expirydate-value`}
                onClick={handleClick}
                onMouseOver={handleMouseOver}
                onMouseOut={handleMouseOut}
                style={cellStyle}
            >
                <Box display="flex" alignItems="center">
                    <Box marginRight={1}>
                        {expiryDate?.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                    </Box>
                    <animated.div style={animationStyles}>
                        <NotificationsIcon ref={iconRef} style={{ color: iconColor, cursor: 'pointer' }} />
                    </animated.div>
                </Box>
            </ClickableTableCell>
            {showDialog && <NotificationDialog open={showDialog} onClose={() => setShowDialog(false)} />}
        </>
    );
}
