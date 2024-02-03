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
    navigateGoto?: () => void | undefined;
}

const StyledImg = styled("img")({
    "&:hover": {
        cursor: "pointer"
    }
});

export default ({ small, large, very_large, className, navigateGoto }: IProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const size: string = small ? "32px" : large ? "70px" : very_large ? "140px" : "60px";

    return <>
        <Button
            disableRipple
            variant={undefined}
            startIcon={navigateGoto ? <ArrowBackIosNewRoundedIcon /> : undefined}
            onClick={() => navigateGoto ? navigateGoto() : navigate(ROUTES.VEHICLE, { state: { from: location } })}
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
    </>;
}
