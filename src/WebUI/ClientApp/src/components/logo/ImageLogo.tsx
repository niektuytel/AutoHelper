import { styled } from "@mui/material/styles";
import React, { useEffect, useState } from "react";
import ArrowBackIosNewRoundedIcon from '@mui/icons-material/ArrowBackIosNewRounded';
import { useLocation, useNavigate } from "react-router-dom";
import { Button, Hidden } from "@mui/material";

// custom imports
import { ROUTES } from "../../constants/routes";

interface IProps {
    small?: boolean;
    large?: boolean;
    very_large?: boolean;
    className?: string;
}

const StyledImg = styled("img")({
    "&:hover": {
        cursor: "pointer"
    }
});

export default ({ small, large, very_large, className }: IProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const [pathHistory, setPathHistory] = useState<string[]>([]);
    const size: string = small ? "32px" : large ? "70px" : very_large ? "140px" : "60px";

    useEffect(() => {
        // Add the current path to the path history, if it's not the same as the last one
        if (pathHistory[pathHistory.length - 1] !== location.pathname) {
            setPathHistory([...pathHistory, location.pathname]);
        }
    }, [location.pathname]);

    const onClickBack = () => {
        if (window.location.hash) {
            // Find the most recent path that's different from the current one
            const previousPath = pathHistory.slice(0, -1).reverse().find(path => path !== location.pathname);
            if (previousPath) {
                navigate(previousPath);
            } else {
                navigate(-1); // Fallback to regular back navigation
            }
        } else {
            navigate(-1); // Regular back navigation
        }
    };

    return (
        <>
            <Button
                disableRipple 
                variant={undefined}
                startIcon={location.pathname !== ROUTES.SELECT_VEHICLE && <ArrowBackIosNewRoundedIcon />}
                onClick={() => location.pathname !== ROUTES.SELECT_VEHICLE && onClickBack()}
            >
                <Hidden mdUp>
                    <StyledImg
                        src={"/images/ic_blue.svg"}
                        height={size}
                        className={className || ''}
                        alt="AutoHelper.nl"
                    />
                </Hidden>
                <Hidden mdDown>
                    <StyledImg
                        src={"/images/ic_blue_autohelper.svg"}
                        height={size}
                        className={className || ''}
                        alt="AutoHelper.nl"
                    />
                </Hidden>
            </Button>
        </>
    );
}
