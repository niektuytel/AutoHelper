import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Button, Paper, IconButton, Typography } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import CloseIcon from '@mui/icons-material/Close';
import { COLORS } from '../../../../constants/colors';
import { GarageServiceItemDto } from '../../../../app/web-api-client';
import { getTitleForServiceType } from '../../defaultGarageService';

interface IProps {
    items: GarageServiceItemDto[];
    setItems: (items: GarageServiceItemDto[]) => void;
}

export default ({ items, setItems }: IProps) => {
    const { t } = useTranslation();

    // Calculate the total price of all services
    const totalPrice = items.reduce((sum, item) => sum + (item.price || 0), 0).toFixed(2);

    const handleDelete = (itemToDelete: GarageServiceItemDto) => {
        const updatedItems = items.filter((item) =>
            item.id !== itemToDelete.id &&
            item.description !== itemToDelete.description
        );

        setItems(updatedItems);
    };

    return (
        <Box position="fixed" bottom={0} right={0} zIndex={999} maxWidth="100%">
            <Paper elevation={3} sx={{
                borderRadius: 2,
                border: '1px solid #ccc',
                padding: 2,
                display: 'flex',
                flexDirection: 'column', 
                alignItems: 'stretch',    
                margin: "25px"
            }}>
                <Typography variant="h6" mb={2}>
                    {t("Selected Services")} (€{totalPrice})
                </Typography>
                <Box alignItems="center" width="100%" sx={{ overflowX: "auto", marginBottom: "10px" }}>
                    {items.map((item) => (
                        <Box
                            display="flex"
                            alignItems="center"
                            my={0.5} 
                            border={1}  
                            borderColor={COLORS.BORDER_GRAY}  
                            borderRadius={2}
                            p={1}  
                            key={item.type}
                            title={item.description}
                        >
                            <Box flexGrow={1}>
                                {getTitleForServiceType(t, item.type!, item.description)}
                            </Box>
                            <Box ml={2}>
                                €{Number(item.price).toFixed(2)}
                            </Box>
                            <IconButton
                                edge="end"
                                size="small"
                                onClick={() => handleDelete(item)}
                            >
                                <CloseIcon />
                            </IconButton>
                        </Box>
                    ))}

                </Box>

                {/* Confirm Button */}
                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => {
                        
                    }}
                >
                    {t("Confirm")}
                </Button>
            </Paper>
        </Box>
    );
}
