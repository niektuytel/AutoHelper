import React, { CSSProperties } from "react";
import { ClickAwayListener, IconButton, Theme, Tooltip, Typography, withStyles } from "@material-ui/core";
import InfoOutlinedIcon from '@material-ui/icons/InfoOutlined';

const HtmlTooltip = withStyles((theme: Theme) => ({
    tooltip: {
        backgroundColor: '#f5f5f9',
        color: 'rgba(0, 0, 0, 0.87)',
        maxWidth: 220,
        fontSize: theme.typography.pxToRem(12),
        border: '1px solid #dadde9',
    },
}))(Tooltip);

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