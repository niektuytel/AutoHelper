import React, { useState, useEffect } from 'react';
import { useSpring, animated } from 'react-spring';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { COLORS } from '../../../../constants/colors';
import NotificationDialog from './NotificationDialog';

interface NotificationIconProps {
    expiryDate: Date | undefined;
}

const NotificationIcon: React.FC<NotificationIconProps> = ({ expiryDate }) => {
    const [iconColor, setIconColor] = useState('darkgray');
    const [showDialog, setShowDialog] = useState(false);
    const [animationToggle, setAnimationToggle] = useState(false);
    const [shakeCount, setShakeCount] = useState(0);

    useEffect(() => {
        if (expiryDate) {
            const expiry = new Date(expiryDate);
            const today = new Date();
            const diffDays = (expiry.getTime() - today.getTime()) / (1000 * 3600 * 24);
            const diffMonths = diffDays / 30;

            if (diffMonths <= 2) {
                setIconColor('red');
            }
        }
    }, [expiryDate]);

    useEffect(() => {
        let interval: NodeJS.Timeout;
        if (shakeCount < 10) {
            interval = setInterval(() => {
                setAnimationToggle(toggle => !toggle);
                setShakeCount(count => count + 1);
            }, 200);
        } else {
            setTimeout(() => {
                setShakeCount(0);
            }, 2000);
        }
        return () => clearInterval(interval);
    }, [shakeCount]);

    const animationStyles = useSpring({
        to: { transform: animationToggle ? 'rotate(10deg)' : 'rotate(-10deg)' },
        config: { duration: 100 },
    });

    const iconStyle = {
        color: iconColor,
        cursor: 'pointer',
        transition: 'transform 0.3s ease, color 0.3s ease',
    };

    const hoverStyle = {
        transform: 'scale(1.2)',
        color: COLORS.BLUE,
    };

    const handleClick = () => {
        setShowDialog(true);
    };

    return (
        <>
            <animated.div style={animationStyles} onClick={handleClick}>
                <NotificationsIcon
                    style={iconStyle}
                    onMouseOver={(e) => (e.currentTarget.style.transform = hoverStyle.transform, e.currentTarget.style.color = hoverStyle.color)}
                    onMouseOut={(e) => (e.currentTarget.style.transform = '', e.currentTarget.style.color = iconColor)}
                />
            </animated.div>
            {showDialog && <NotificationDialog open={showDialog} onClose={() => setShowDialog(false)} />}
        </>
    );
};

export default NotificationIcon;
