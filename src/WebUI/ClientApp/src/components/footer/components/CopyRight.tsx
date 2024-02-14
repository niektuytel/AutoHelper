import React from "react";
import { useNavigate } from "react-router";
import { Box, Link } from "@mui/material";
import { ROUTES } from "../../../constants/routes";

const CopyRight = () => {
    const navigate = useNavigate();

    const handlePolicyClick = () => {
        navigate(`${ROUTES.POLICY}`);
    };

    return (
        <Box sx={{ textAlign: "center" }}>
            <small>
                2023-{new Date().getFullYear()} <a href={window.location.href}>{window.location.host}</a> &copy; All rights reserved (
                <Link onClick={handlePolicyClick} style={{ cursor: 'pointer' }}>
                    policy
                </Link>)
            </small>
        </Box>
    );
}

export default CopyRight;
