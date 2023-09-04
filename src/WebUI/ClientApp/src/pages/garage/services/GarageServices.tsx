import React, { useState } from "react";
import {
    Box,
    Button,
    Container,
    Divider,
    IconButton,
    Tooltip,
    Typography,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    TextField,
    Card,
    CardHeader,
    CardContent,
    CardActions
} from "@mui/material";
import { useTranslation } from "react-i18next";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';

// own imports

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();

    // Sample data
    const testData = [
        { id: 1, title: "Service 1", description: "This is service 1 description.", duration: "10 min" },
        // ... add more sample data
    ];

    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState<boolean>(false);
    const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
    const [cartItems, setCartItems] = useState<any[]>([]);

    return (
        <>
            <Box pt={4}>
                <Typography variant="h4" gutterBottom>
                    {t("Services")}
                    <Tooltip title={t("Services.Description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
            </Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <div>
                    <IconButton onClick={() => { }}>
                        <AddIcon />
                    </IconButton>
                    <IconButton
                        disabled={!selectedItem}
                        onClick={() => setEditDialogOpen(true)}
                    >
                        <EditIcon />
                    </IconButton>
                    <IconButton
                        disabled={!selectedItem}
                        onClick={() => setConfirmDeleteOpen(true)}
                    >
                        <DeleteIcon />
                    </IconButton>
                </div>
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <Box>
                {testData.map((item) => (
                    <Card key={item.id} style={{ marginBottom: "20px" }}>
                        <CardHeader
                            action={
                                <IconButton onClick={() => setCartItems([...cartItems, item])}>
                                    <ShoppingCartIcon />
                                </IconButton>
                            }
                            title={item.title}
                        />
                        <CardContent>
                            <Typography variant="body1">{item.description}</Typography>
                            <Typography variant="body2" color="textSecondary">Status: Offline</Typography>
                        </CardContent>
                        <CardActions>
                            <Box display="flex" alignItems="center">
                                <AccessTimeIcon color="action" />
                                <Typography variant="body2" color="textSecondary" style={{ marginLeft: "8px" }}>
                                    {item.duration}
                                </Typography>
                            </Box>
                        </CardActions>
                    </Card>
                ))}
            </Box>

            <Dialog
                open={confirmDeleteOpen}
                onClose={() => setConfirmDeleteOpen(false)}
            >
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent>
                    <Typography>Are you sure you want to delete this item?</Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setConfirmDeleteOpen(false)} color="primary">
                        Cancel
                    </Button>
                    <Button
                        onClick={() => {
                            // Your delete logic here
                            setSelectedItem(null);
                            setConfirmDeleteOpen(false);
                        }}
                        color="primary"
                    >
                        Confirm
                    </Button>
                </DialogActions>
            </Dialog>

            <Dialog
                open={editDialogOpen}
                onClose={() => setEditDialogOpen(false)}
            >
                <DialogTitle>Edit Item</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Title"
                        defaultValue={selectedItem?.title}
                        fullWidth
                    />
                    <TextField
                        margin="dense"
                        label="Description"
                        defaultValue={selectedItem?.description}
                        fullWidth
                        multiline
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setEditDialogOpen(false)} color="primary">
                        Cancel
                    </Button>
                    <Button
                        onClick={() => {
                            // Your update logic here
                            setEditDialogOpen(false);
                        }}
                        color="primary"
                    >
                        Update
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}
