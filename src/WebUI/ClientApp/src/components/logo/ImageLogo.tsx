import { styled } from "@mui/material/styles";
import React from "react";
import { useNavigate } from "react-router-dom";
import { Hidden } from "@mui/material";

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

const AutoHelperImage: React.FC<IProps> = ({ small, large, very_large, className }) => {
    const navigate = useNavigate();
    const size: string = small ? "32px" : large ? "70px" : very_large ? "140px" : "60px";

    const onClick = () => {
        navigate("/");
    }

    return (
        <>
            <Hidden mdUp>
                <StyledImg
                    src={"/images/ic_blue.svg"}
                    height={size}
                    className={className || ''}
                    onClick={onClick}
                    alt="AutoHelper.nl"
                />
            </Hidden>
            <Hidden mdDown>
                <StyledImg
                    src={"/images/ic_blue_autohelper.svg"}
                    height={size}
                    className={className || ''}
                    onClick={onClick}
                    alt="AutoHelper.nl"
                />
            </Hidden>
        </>
    );
}

export default AutoHelperImage;
