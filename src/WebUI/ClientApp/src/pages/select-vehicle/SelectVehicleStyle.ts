import { CSSProperties } from "react"
import { colorOnIndex } from "../../i18n/ColorValues"

// import bgImage from "../../assets/images/bg-presentation.jpg";

export const BGPanelStyle: CSSProperties =
{
    margin: "0 auto",
    marginTop: "-75px",
    width: "95%",
    padding: "20px",
    textAlign: "center",
    backgroundColor: "#ffffff",
    backdropFilter: "saturate(200%) blur(30px)"
}

export const BackgroundStyle: CSSProperties = {
    height: "90vh",
    // width: "200%",
    // backgroundImage: `url(${bgImage})`,
    backgroundSize: "cover",
    backgroundPosition: "top",
    display: "grid",
    placeItems: "center",
    cursor: "pointer"
}

export const GotoProductsParentStyle: CSSProperties = {
    zIndex: 1,
    // position: "relative",
    cursor: "pointer",
    width: "100%"
}

export const GotoProductsStyle: CSSProperties = {
    color: "black",
    backgroundColor: "#ffffff61",
    display: "flex",
    height: "100%",
    justifyContent: "center",
    marginTop: "30vh",
    fontSize: "min(10vw, 35px)",

    border: "2px solid black",
    width: "fit-content",
    borderRadius: "5px",
    paddingLeft:"5px",
    paddingRight:"5px",
    borderColor: "#123123"
}

export const GotoProductsIconStyle: CSSProperties = {
    fontSize: "min(10vw, 60px)"//,
    // paddingBottom:"10px"
}
