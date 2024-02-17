import React, { useState, useEffect, useRef, CSSProperties } from 'react';
import { useSpring, animated } from 'react-spring';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { COLORS } from '../../../../constants/colors';
import NotificationDialog from './NotificationDialog';
import { Box, styled, TableCell } from '@mui/material';

interface NotificationIconProps {
    expiryDate: Date | undefined;
    isMobile: boolean;
}

export default ({ expiryDate, isMobile }: NotificationIconProps) => {
    const [showDialog, setShowDialog] = useState(false);
    const [animationToggle, setAnimationToggle] = useState(false);
    const [shakeCount, setShakeCount] = useState(0);

    // Refactor color logic into a separate function
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

    useEffect(() => {
        if (shakeCount < 10) {
            const interval = setInterval(() => {
                setAnimationToggle(toggle => !toggle);
                setShakeCount(count => count + 1);
            }, 200);
            return () => clearInterval(interval);
        } else {
            const timeout = setTimeout(() => setShakeCount(0), 1000);
            return () => clearTimeout(timeout);
        }
    }, [shakeCount]);

    const animationStyles = useSpring({
        to: { transform: animationToggle ? 'rotate(10deg)' : 'rotate(-10deg)' },
        config: { duration: 100 },
    });

    const iconRef = useRef<SVGSVGElement>(null);

    // Simplify hover handlers
    const handleMouseOver = () => iconRef.current?.style.setProperty('color', COLORS.BLUE);
    const handleMouseOut = () => iconRef.current?.style.setProperty('color', iconColor);

    // Styled TableCell
    const ClickableTableCell = styled(TableCell)({
        cursor: 'pointer',
        '&:hover': {
            backgroundColor: '#f5f5f5',
        },
    });

    const cellStyle: CSSProperties = {
        textAlign: 'left',
        paddingRight: isMobile ? '' : '0',
    };

    const handleClick = () => setShowDialog(true);

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
};

