import React from "react";
import { Box, Container, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import useGarageSearch from "./useGarageSearch";

interface IProps {
}

const inKmRange = 100;
const pageIndex = 1;
const pageSize = 10;

export default ({ }: IProps) => {
    const { license_plate, lat, lng } = useParams();
    const { loading, garages } = useGarageSearch(license_plate!, Number(lat!), Number(lng!), inKmRange, pageIndex, pageSize);


    // splittedPath looks like "[lat,lng, licence_plate, ...]"
    const splittedPath = window.location.pathname.split('/').filter(x => x.length > 0).slice(1);
    
    return (
        <Box
            style={{
                position: "relative",
                marginLeft: "10px",
                marginRight: "10px"
            }}
        >
            <Container
                maxWidth="lg"
                style={{
                    padding: "0",
                    textAlign: "center"
                }}
            >
                {garages?.items?.map((item, index) => (
                    <Box
                        key={index}
                        style={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "space-between",
                            alignItems: "center",
                            padding: "10px",
                            borderBottom: "1px solid #ccc"
                        }}
                    >
                        <Box
                            style={{
                                display: "flex",
                                flexDirection: "row",
                                alignItems: "center"
                            }}
                        >
                            {/*<ImageLogo*/}
                            {/*    src={garage.logo}*/}
                            {/*    style={{*/}
                            {/*        width: "50px",*/}
                            {/*        height: "50px",*/}
                            {/*        marginRight: "10px"*/}
                            {/*    }}*/}
                            {/*/>*/}
                            <Box
                                style={{
                                    display: "flex",
                                    flexDirection: "column",
                                    alignItems: "flex-start"
                                }}
                            >
                                <Typography
                                    variant="h6"
                                    style={{
                                        color: colorOnIndex(index)
                                    }}
                                >
                                    {item.name}
                                </Typography>
                                <Typography
                                    variant="body1"
                                    style={{
                                        color: colorOnIndex(index)
                                    }}
                                >
                                    {/*{item.location?.address}*/}
                                </Typography>
                            </Box>
                        </Box>
                        <Box
                            style={{
                                display: "flex",
                                flexDirection: "column",
                                alignItems: "flex-end"
                            }}
                        >
                            {/*<Typography*/}
                            {/*    variant="body1"*/}
                            {/*    style={{*/}
                            {/*        color: colorOnIndex(index)*/}
                            {/*    }}*/}
                            {/*>*/}
                            {/*    {garage.distance} km*/}
                            {/*</Typography>*/}
                            {/*<Typography*/}
                            {/*    variant="body1"*/}
                            {/*    style={{*/}
                            {/*        color: colorOnIndex(index)*/}
                            {/*    }}*/}
                            {/*>*/}
                            {/*    {garage.price} €*/}
                            {/*</Typography>*/}
                        </Box>
                    </Box>
                ))}
            </Container>
        </Box>
    );
}
