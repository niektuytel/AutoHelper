import { Box } from "@mui/material";
import React from "react";

const CopyRight = () => {
    return <>
        <Box sx={{textAlign:"center"}}>
            <small>
                ©{new Date().getFullYear()} Copyright: <a href={window.location.href}>{window.location.host}</a> (v1)
            </small>
        </Box>
    </>
}

export default CopyRight;
