import React, { CSSProperties } from "react";
import ClickAwayListener from '@mui/material/ClickAwayListener';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { styled } from '@mui/material/styles';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { Theme } from '@mui/system';

const pxToRem = (px: number) => `${px / 16}rem`;

const HtmlTooltip = styled(Tooltip)(({ theme }: { theme: Theme }) => ({
    '& .MuiTooltip-tooltip': {
        backgroundColor: '#f5f5f9',
        color: 'rgba(0, 0, 0, 0.87)',
        maxWidth: 220,
        fontSize: pxToRem(12),
        border: '1px solid #dadde9',
    },
}));

interface IProps {
    text:string;
}

export default ({text}:IProps) => {
    const [open, setOpen] = React.useState(false);
    
    return <>
        <ClickAwayListener onClickAway={() => setOpen(false)}>
            <HtmlTooltip 
                PopperProps={{
                    disablePortal: true,
                }}
                onClose={() => setOpen(false)}
                open={open}
                disableFocusListener
                disableHoverListener
                disableTouchListener
                title={
                    <Typography color="inherit">
                        {text}
                    </Typography>
                }
            >
                <IconButton onClick={() => setOpen(true)} aria-label="info">
                    <InfoOutlinedIcon />
                </IconButton>
            </HtmlTooltip>
        </ClickAwayListener>
    </>
}