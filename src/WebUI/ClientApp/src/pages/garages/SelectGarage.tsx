import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { Box, Button, Container, Pagination, Paper, Skeleton, Typography } from "@mui/material";

// local
import useGarageSearch from "./useGarageSearch";
import { COLORS } from "../../constants/colors";
import GarageListItem from "./components/GarageListItem";
import GarageSearchField from "./components/GarageSearchField";


interface IProps {
}

const inMeterRange = 150000;
const pageSize = 10;

export default ({ }: IProps) => {
    const { t } = useTranslation();
    const { license_plate, lat, lng } = useParams();
    const [currentPage, setCurrentPage] = useState(1);
    const queryParams = new URLSearchParams(window.location.search);
    const { loading, garages } = useGarageSearch(license_plate!, Number(lat!), Number(lng!), inMeterRange, currentPage, pageSize, queryParams.get("input") || "", queryParams.get("filters")?.split(",") || []);

    const handlePageChange = (event:any, value:number) => {
        setCurrentPage(value);
    };

    return <>
        <Container maxWidth="lg" sx={{ minHeight: "70vh" }}>
            <Box sx={{ marginBottom: "75px" }}>
                <Box flexGrow={1}>
                    <Box sx={{ backgroundColor: COLORS.BLUE, borderBottomLeftRadius: "5px", borderBottomRightRadius: "5px", p: 1 }}>
                        <Box sx={{
                            height: "90px",
                            display: 'flex',     
                            alignItems: 'center',
                            justifyContent: 'center'
                        }}>
                            <Typography variant="h4" sx={{ color: "white", fontFamily: "'Nunito', sans-serif" }}>
                                {t("SelectGaragePage.Title")}
                            </Typography>
                        </Box>
                        <Paper
                            elevation={2}
                            sx={{ p: 1, width: "initial", position: "relative" }}
                        >
                            <GarageSearchField loading={loading} items={garages?.items || []}/>
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
