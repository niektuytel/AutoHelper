import React, { useEffect, useState } from "react";
import { Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import { styled } from '@mui/material/styles';
import ArrowForwardIosSharpIcon from '@mui/icons-material/ArrowForwardIosSharp';
import MuiAccordion, { AccordionProps } from '@mui/material/Accordion';
import MuiAccordionSummary, { AccordionSummaryProps } from '@mui/material/AccordionSummary';
import MuiAccordionDetails from '@mui/material/AccordionDetails';

// custom imports
import useVehicleInformation from "../../useVehicleSpecifications";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleInfo } = useVehicleInformation(license_plate);
    const [expanded, setExpanded] = useState<string | undefined>(undefined);

    const handleChange = (panel: string) => (event: React.SyntheticEvent, newExpanded: boolean) => {
        setExpanded(newExpanded ? panel : undefined);
    };

    return (
        <>
            {loading ?
                Array.from({ length: 10 }).map((_, index) => (
                    <Accordion expanded={false} key={`Accordion-${index}`}>
                        <AccordionSummary>
                            <Skeleton sx={{ width: "100%" }} />
                        </AccordionSummary>
                    </Accordion>
                ))
                : vehicleInfo?.data?.map((section, index) => (
                    <Accordion key={`Accordion-${index}`} expanded={expanded === `panel${index}`} onChange={handleChange(`panel${index}`)}>
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
                                                <TableCell key={`specifications-cell-${cellIndex}`}
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
    );
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