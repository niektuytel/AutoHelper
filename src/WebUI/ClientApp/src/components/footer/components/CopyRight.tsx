import { Box } from "@mui/material";
import React from "react";

const CopyRight = () => {
    return <>
        <Box sx={{textAlign:"center"}}>
            <small>
                2023-{new Date().getFullYear()} <a href={window.location.href}>{window.location.host}</a>
            </small>
        </Box>
    </>
}

export default CopyRight;
