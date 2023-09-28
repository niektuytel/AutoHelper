import React from 'react';
import { Box, Hidden } from "@mui/material";

interface GradientBoxProps {
    children?: React.ReactNode;
}

const GradientBox: React.FC<GradientBoxProps> = ({ children }) => {
    const leftColor = "#1C94F3";
    const rightColor = "transparent";
    const style = {
        position: "relative",
        height: "100%",
        padding: "10px"
    };

    return (
        <>
            <Hidden mdDown>
                <Box
                    sx={{
                        ...style,
                        background: `linear-gradient(80deg, ${leftColor} 50%, ${rightColor} 50.1%)`
                    }}
                >
                    {children}
                </Box>
            </Hidden>
            <Hidden mdUp>
                <Box
                    sx={{
                        ...style,
                        background: `linear-gradient(171deg, ${leftColor} 57%, ${rightColor} 57.1%)`
                    }}
                >
                    {children}
                </Box>
            </Hidden>
        </>
    );
}

export default GradientBox;
