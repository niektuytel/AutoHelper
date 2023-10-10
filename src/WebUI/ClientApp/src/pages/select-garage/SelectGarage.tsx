import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, CircularProgress, Container, Divider, Pagination, Paper, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import useGarageSearch from "./useGarageSearch";
import { COLORS } from "../../constants/colors";
import GarageListItem from "./components/GarageListItem";
import { GarageLookupDto, PaginatedListOfGarageLookupDto } from "../../app/web-api-client";
import GarageSearchField from "./components/GarageSearchField";
import { useQueryClient } from "react-query";

interface IProps {
}

const inMeterRange = 150000;
const pageSize = 10;

export default ({ }: IProps) => {
    const { license_plate, lat, lng } = useParams();
    const [currentPage, setCurrentPage] = useState(1);
    const { loading, garages, fetchGarages, setGaragesData } = useGarageSearch(license_plate!, Number(lat!), Number(lng!), inMeterRange, currentPage, pageSize);

    // TODO: const { loading, services, fetchServices } = useVehicleRelatedServices(license_plate!);

    const queryClient = useQueryClient();

    const handlePageChange = (event:any, value:number) => {
        setCurrentPage(value);
        fetchGarages(license_plate!, Number(lat!), Number(lng!), inMeterRange, value, pageSize, null);
    };

    const handleSearchExecuted = (data: PaginatedListOfGarageLookupDto) => {
        // Assuming setGaragesData is what you named the updater function
        setGaragesData(data);
    };

    return <>
        <Container maxWidth="lg" sx={{ minHeight: "70vh" }}>
            <Box sx={{ marginBottom: "75px" }}>
                <Box flexGrow={1}>
                    <Box sx={{ height: "70px", backgroundColor: COLORS.BLUE }}>
                        {/*// TODO: Space between search inputbar and header set motivational text, 'wij zorgen voor je onderhouds boekje', 'wij vragen offertes op en houden je op de hoogte', 'Wij be(zorgen) voor je auto'*/}
                    </Box>
                    <Paper
                        elevation={5}
                        sx={{ mb: 1, p: 1, width: "100%", position: "relative", top: "-8px" }}
                    >
                        <GarageSearchField
                            license_plate={license_plate!}
                            latitude={Number(lat!)}
                            longitude={Number(lng!)}
                            in_km_range={inMeterRange}
                            page_size={pageSize}
                            onSearchExecuted={handleSearchExecuted}
                        />
                    </Paper>
                    <Box sx={{ minHeight:"70vh", mt:1}}>
                        {garages?.items?.map((item, index) => (
                            <GarageListItem key={`garageItem-${index}`} garage={item}/>
                        ))}
                    </Box>
                    <Paper
                        elevation={5}
                        sx={{ mb: 1, p: 1 }}
                        style={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: "flex-end",
                            alignItems: "center"
                        }}
                    >
                        <Pagination
                            count={garages?.totalPages}
                            shape="rounded"
                            page={currentPage}
                            onChange={handlePageChange}
                        />
                    </Paper>
                </Box>
            </Box>
        </Container>
    </>;
}
