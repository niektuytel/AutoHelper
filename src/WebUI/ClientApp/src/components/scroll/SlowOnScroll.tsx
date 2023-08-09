import React from "react";
import PropTypes from "prop-types";

const SlowOnScroll = (props:any) =>
{
    const { children, src } = props;

    let windowScrollTop;
    if (window.innerWidth >= 768)
    {
        windowScrollTop = window.pageYOffset / 3;
    }
    else
    {
        windowScrollTop = 0;
    }

    const [transform, setTransform] = React.useState(
        "translate3d(0," + windowScrollTop + "px,0)"
    );

    const resetTransform = () => {
        var windowScrollTop = window.pageYOffset / 3;
        setTransform("translate3d(0," + windowScrollTop + "px,0)");
    };

    React.useEffect(() => {
        if (window.innerWidth >= 768)
        {
            window.addEventListener("scroll", resetTransform);
        }
        return function cleanup() {
            if (window.innerWidth >= 768)
            {
                window.removeEventListener("scroll", resetTransform);
            }
        };
    }, []);

    return <div style={{ transform: transform }}>
        {children}
    </div>
}

export default SlowOnScroll;