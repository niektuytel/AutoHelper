import React from "react";
import PropTypes from 'prop-types';
import { Slide, useScrollTrigger } from "@mui/material";


function HideOnScroll(props:any) {
    const { children, window, show } = props;
    // Note that you normally won't need to set the window ref as useScrollTrigger
    // will default to window.
    // This is only being set here because the demo is in an iframe.
    const trigger = useScrollTrigger({ target: window ? window() : undefined });
  
    return <>
        <Slide appear={false} direction="down" in={!trigger || show}>
            {children}
        </Slide>
    </>
}
  
HideOnScroll.propTypes = {
    children: PropTypes.element.isRequired,
    /**
     * Injected by the documentation to work in an iframe.
     * You won't need it on your project.
     */
    window: PropTypes.func,
    show:PropTypes.bool
};

export default HideOnScroll;