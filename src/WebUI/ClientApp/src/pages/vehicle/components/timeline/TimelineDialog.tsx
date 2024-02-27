import React from 'react';
import { Dialog, DialogActions, DialogContent, DialogTitle, Button, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { VehicleTimelineDtoItem } from '../../../../app/web-api-client';

interface QuestionDialogProps {
    open: boolean;
    onClose: () => void;
    timeline: VehicleTimelineDtoItem;
}

export default function QuestionDialog({ open, onClose, timeline }: QuestionDialogProps) {
    const { t } = useTranslation();

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
            <DialogTitle>{timeline.description}</DialogTitle>
            <DialogContent>
                <Typography variant="body2" paragraph>
                    {t("TimelineDialog.Description")}
                    {/*<a href="https://locationOfMakingitprivate.nl">*/}
                    {/*    Hier kunt u de informatie onzichtbaar maken voor anderen.*/}
                    {/*</a>*/}
                </Typography>
                <TableContainer component={Paper}>
                    <Table aria-label="simple table">
                        <TableBody>
                            {timeline.extraData?.map((item: any, index: number) => (
                                <TableRow key={index}>
                                    <TableCell component="th" scope="row">
                                        {item.item1}
                                    </TableCell>
                                    <TableCell align="right">{item.item2}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} style={{ textTransform: 'capitalize' }}>
                    {t("Cancel")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
