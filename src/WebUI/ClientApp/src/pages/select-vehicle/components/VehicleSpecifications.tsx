import React, { useEffect, useState } from "react";
import { Box, Card, CircularProgress, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import { CSSProperties } from "react";
import { styled } from '@mui/material/styles';
import ArrowForwardIosSharpIcon from '@mui/icons-material/ArrowForwardIosSharp';
import MuiAccordion, { AccordionProps } from '@mui/material/Accordion';
import MuiAccordionSummary, {
    AccordionSummaryProps,
} from '@mui/material/AccordionSummary';
import MuiAccordionDetails from '@mui/material/AccordionDetails';
import useVehicleInformation from "../useVehicleInformation";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleInfo } = useVehicleInformation(license_plate);
    const [expanded, setExpanded] = useState<string[]>([]);

    const handleChange = (panel: string) => (event: React.SyntheticEvent, newExpanded: boolean) => {
        if (newExpanded) {
            setExpanded((prev) => [...prev, panel]);
        } else {
            setExpanded((prev) => prev.filter((item) => item !== panel));
        }
    };

    return <>
        {loading ?
            Array.from({ length: 10 }).map((_, index) => (
                <Accordion key={index}>
                    <AccordionSummary>
                        <Skeleton sx={{ width: "100%" }} />
                    </AccordionSummary>
                </Accordion>
            ))
        : vehicleInfo?.data?.map((section, index) => (
            <Accordion expanded={expanded.includes(`panel${index}`)} onChange={handleChange(`panel${index}`)}>
                <AccordionSummary aria-controls={`panel${index}d-content`} id={`panel${index}d-header`}>
                    <Typography>{section.title}</Typography>
                </AccordionSummary>
                <AccordionDetails sx={{ padding: "0" }}>
                    <Table>
                        <TableBody>
                            {section.values!.map((line, rowIndex) => (
                                <TableRow
                                    key={rowIndex}
                                    sx={{
                                        backgroundColor: rowIndex % 2 === 0 ? 'grey.100' : 'white'
                                    }}
                                >
                                    {line!.map((value, cellIndex) => (

                                        <TableCell
                                            style={{ width: `${(line.length / 100)}%`, textAlign: 'left' }}
                                        >
                                            {value}
                                        </TableCell>
                                    ))}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </AccordionDetails>
            </Accordion>
        ))}
    </>
}

const Accordion = styled(({ ...props }: AccordionProps) => (
    <MuiAccordion elevation={0} square {...props} />
))(({ theme }) => ({
    border: `1px solid ${theme.palette.divider}`,
    marginTop: "0",
    '&:before': {
        display: 'none',
    },
}));


const AccordionSummary = styled((props: AccordionSummaryProps) => (
    <MuiAccordionSummary
        expandIcon={<ArrowForwardIosSharpIcon sx={{ fontSize: '0.9rem' }} />}
        {...props}
    />
))(({ theme }) => ({
    backgroundColor:
        theme.palette.mode === 'dark'
            ? 'rgba(255, 255, 255, .05)'
            : 'rgba(0, 0, 0, .03)',
    flexDirection: 'row-reverse',
    '& .MuiAccordionSummary-expandIconWrapper.Mui-expanded': {
        transform: 'rotate(90deg)',
        marginRight: theme.spacing(1),
    },
    '& .MuiAccordionSummary-content': {
        marginLeft: theme.spacing(1),
    },
}));

const AccordionDetails = styled(MuiAccordionDetails)(({ theme }) => ({
    padding: theme.spacing(2),
    borderTop: '1px solid rgba(0, 0, 0, .125)',
}));