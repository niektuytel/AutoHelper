import React, { useEffect, useState } from "react";
import { Box, Button, Container, Pagination, Paper, Skeleton } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import ArrowBackIosNewRoundedIcon from '@mui/icons-material/ArrowBackIosNewRounded';

// local
import useGarageSearch from "./useGarageSearch";
import { COLORS } from "../../constants/colors";
import GarageListItem from "./components/GarageListItem";
import { PaginatedListOfGarageLookupBriefDto } from "../../app/web-api-client";
import GarageSearchField from "./components/GarageSearchField";

interface IProps {
}

const inMeterRange = 150000;
const pageSize = 10;

export default ({ }: IProps) => {
    const { t } = useTranslation();
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

    return <>
        <Container maxWidth="lg" sx={{ minHeight: "70vh" }}>
            <Box sx={{ marginBottom: "75px" }}>
                <Box flexGrow={1}>
                    <Box sx={{ backgroundColor: COLORS.BLUE, borderBottomLeftRadius: "5px", borderBottomRightRadius: "5px", p: 1 }}>
                        <Box sx={{ height: "90px"}}>
                        </Box>
                        <Paper
                            elevation={2}
                            sx={{ p: 1, width: "initial", position: "relative" }}
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
