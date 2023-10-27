import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, CircularProgress, Container, Divider, Pagination, Paper, Skeleton, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

// local
import ImageLogo from "../../components/logo/ImageLogo";
import useGarageSearch from "./useGarageSearch";
import { COLORS } from "../../constants/colors";
import GarageListItem from "./components/GarageListItem";
import { GarageLookupDto, PaginatedListOfGarageLookupBriefDto } from "../../app/web-api-client";
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

    const handlePageChange = (event:any, value:number) => {
        setCurrentPage(value);
        fetchGarages(license_plate!, Number(lat!), Number(lng!), inMeterRange, value, pageSize, null, null);
    };

    const handleSearchExecuted = (data: PaginatedListOfGarageLookupBriefDto) => {
        // Assuming setGaragesData is what you named the updater function
        setGaragesData(data);
    };

    // Batch locations that is faster search?

    return <>
        <Container maxWidth="lg" sx={{ minHeight: "70vh" }}>
            <Box sx={{ marginBottom: "75px" }}>
                <Box flexGrow={1}>
                    <Box sx={{ backgroundColor: COLORS.BLUE, borderBottomLeftRadius: "5px", borderBottomRightRadius: "5px", p:1 }}>
                        <Paper
                            elevation={2}
                            sx={{ p: 1, width: "initial", position: "relative", marginTop:"55px" }}
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
                    </Box>
                    <Box sx={{ minHeight: "70vh", ml: 1, mr: 1, mt:1 }}>
                        {loading ? 
                            <>
                                <Skeleton variant="rounded" height="100px" sx={{ mb: 1 }} />
                                <Skeleton variant="rounded" height="100px" sx={{ mb: 1 }} />
                                <Skeleton variant="rounded" height="100px" sx={{ mb: 1 }} />
                                <Skeleton variant="rounded" height="100px" sx={{ mb: 1 }} />
                                <Skeleton variant="rounded" height="100px" sx={{ mb: 1 }} />
                            </>
                            :
                            garages?.items?.map((item, index) => (
                                <GarageListItem
                                    key={`garageItem-${index}`}
                                    garage={item}
                                    licensePlate={license_plate!}
                                    lat={lat!}
                                    lng={lng!}
                                />
                            ))
                        }
                    </Box>
                    <Paper
                        elevation={2}
                        sx={{ p: 1, ml:1, mr:1 }}
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
