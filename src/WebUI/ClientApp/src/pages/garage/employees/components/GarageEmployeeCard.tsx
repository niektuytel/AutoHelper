import React, { useState } from "react";
import {
    Box,
    Button,
    Divider,
    IconButton,
    Tooltip,
    Typography,
    TextField,
    Card,
    CardHeader,
    CardContent,
    CardActions,
    CircularProgress,
    useTheme,
    useMediaQuery,
    Drawer,
    ButtonGroup,
    InputAdornment,
    Select,
    MenuItem,
} from "@mui/material";
import { useTranslation } from "react-i18next";
import AddIcon from '@mui/icons-material/Add';
import PersonOffIcon from '@mui/icons-material/PersonOff';
import PersonIcon from '@mui/icons-material/Person';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import EuroIcon from '@mui/icons-material/Euro';
import { GarageServiceType, GarageEmployeeItemDto } from "../../../../app/web-api-client";
import { COLORS } from "../../../../constants/colors";
import { getTitleForServiceType } from "../../defaultGarageService";
import { DAYSINWEEKSHORT } from "../../../../constants/days";

// own imports

const DAYS = [ '' ]

interface IProps {
    employee: GarageEmployeeItemDto;
    selectedItem: GarageEmployeeItemDto;
    setSelectedItem: (service: GarageEmployeeItemDto) => void;
}

export default ({ employee, selectedItem, setSelectedItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();

    return (
        <Card
            style={{
                marginBottom: "10px",
                padding: "8px",
                cursor: "pointer",
                border: selectedItem === employee ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
            }}
            onClick={() => setSelectedItem(employee)}
        >
            <CardHeader
                title={employee.contact?.fullName}
                titleTypographyProps={{ variant: "body1" }}
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
            <CardActions style={{ padding: "0", justifyContent: "space-between" }}>
                <Box display="flex" alignItems="center">
                    <AccessTimeIcon color="action" fontSize="small" />
                    <Typography variant="caption" color="textSecondary" style={{ marginLeft: "8px" }}>
                        {(employee.workSchema && employee.workSchema.length != 0) ?
                            employee.workSchema.map(item => DAYSINWEEKSHORT[item.dayOfWeek]).join(', ')
                            :
                            `${t('has no work schema')}`
                        }
                    </Typography>
                </Box>
                <Box display="flex" alignItems="center" style={{ marginRight: "10px" }} >
                    {employee.isActive
                        ? <PersonIcon style={{ color: 'green' }} fontSize="small" />
                        : <PersonOffIcon style={{ color: 'red' }} fontSize="small" />
                    }
                </Box>
            </CardActions>
        </Card>
    );
}
