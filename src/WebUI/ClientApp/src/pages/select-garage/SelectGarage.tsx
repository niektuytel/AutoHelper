import React from "react";
import { Box, Container, Divider, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import useGarageSearch from "./useGarageSearch";
import { COLORS } from "../../constants/colors";
import GarageListItem from "./components/GarageListItem";

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
    
    return <>
        <Box display="flex" sx={{ marginBottom: "75px", borderBottom: `1px solid ${COLORS.BORDER_GRAY}` }}>
            <Box flexGrow={1}>
                <Container maxWidth="md" sx={{ minHeight: "100vh" }} >
                    <Divider style={{ marginBottom: "20px" }} />
                    {garages?.items?.map((item, index) => (
                        <GarageListItem key={`garageItem-${index}`} garage={item}/>
                    ))}
                </Container>
            </Box>
        </Box>
    </>;
}
