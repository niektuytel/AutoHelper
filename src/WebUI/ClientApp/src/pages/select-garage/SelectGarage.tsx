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
import GarageInviteListItem from "./components/GarageInviteListItem";
import { GarageItemSearchDto, PaginatedListOfGarageItemSearchDto } from "../../app/web-api-client";
import GarageSearchField from "./components/GarageSearchField";
import { useQueryClient } from "react-query";

interface IProps {
}

const inKmRange = 1000;
const pageSize = 10;

export default ({ }: IProps) => {
    const { license_plate, lat, lng } = useParams();
    const [currentPage, setCurrentPage] = useState(1);
    const { loading, garages, fetchGarages, setGaragesData } = useGarageSearch(license_plate!, Number(lat!), Number(lng!), inKmRange, currentPage, pageSize);


    const queryClient = useQueryClient();

    const handlePageChange = (event:any, value:number) => {
        setCurrentPage(value);
        fetchGarages(license_plate!, Number(lat!), Number(lng!), inKmRange, value, pageSize, null);
    };

    const handleSearchExecuted = (data: PaginatedListOfGarageItemSearchDto) => {
        // Assuming setGaragesData is what you named the updater function
        setGaragesData(data);
    };

    return <>
        <Container maxWidth="lg" sx={{ minHeight: "70vh" }}>
            <Box display="flex" sx={{ marginBottom: "75px" }}>
                <Box flexGrow={1}>
                    <Paper
                        elevation={5}
                        sx={{ mb: 1, p: 1, width:"100%", mt:"60px" }}
                    >
                        <GarageSearchField
                            license_plate={license_plate!}
                            latitude={Number(lat!)}
                            longitude={Number(lng!)}
                            in_km_range={inKmRange}
                            page_size={pageSize}
                            onSearchExecuted={handleSearchExecuted}
                        />
                    </Paper>
                    <Box sx={{ minHeight:"70vh", mt:1}}>
                        {garages?.items?.map((item, index) => (
                            <GarageListItem key={`garageItem-${index}`} garage={item}/>
                        ))}
                        {garages?.items && garages?.items?.length < pageSize && <GarageInviteListItem />}
                    </Box>
                    <Paper
                        elevation={5}
                        sx={{ mb: 1, p: 1 }}
                        style={{
                            display: "flex",
                            flexDirection: "row",
                            justifyContent: garages?.items && garages?.items?.length === pageSize ? "space-between" : "flex-end",
                            alignItems: "center"
                        }}
                    >
                        {garages?.items && garages?.items?.length === pageSize &&
                            <Button variant="outlined" sx={{ backgroundColor: "#fff" }}>
                                Invite Garage
                            </Button>
                        }
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
