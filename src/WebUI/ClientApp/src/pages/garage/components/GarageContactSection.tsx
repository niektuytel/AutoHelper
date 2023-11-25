import {
    IconButton,
    Paper,
    Skeleton,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Tooltip,
    Typography,
    useMediaQuery,
    useTheme
} from '@mui/material';
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import { DAYSINWEEK } from '../../../constants/days';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import PhoneIcon from '@mui/icons-material/Phone';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import WhatsAppIcon from '@mui/icons-material/WhatsApp';
import { useTranslation } from 'react-i18next';
import { useState } from 'react';
import { GarageLookupDtoItem } from '../../../app/web-api-client';
import GarageDailySchedule from './GarageDailySchedule';

interface IProps
{
    loading: boolean;
    garageLookup: GarageLookupDtoItem | undefined;
}

export default ({ loading, garageLookup }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [hoveredContact, setHoveredContact] = useState<string | null>(null);

    const copyToClipboard = (data: string) => {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(data);
            // You can display a toast message or some feedback here
        }
    };

    return <>
        <Typography variant="h4" gutterBottom display="flex" alignItems="center">
            {t("Contact")}
            <Tooltip title={t("Contact.Description")}>
                <IconButton size="small">
                    <InfoOutlinedIcon fontSize="inherit" />
                </IconButton>
            </Tooltip>
        </Typography>
        <Typography
            variant="subtitle1"
            display="flex"
            alignItems="center"
            onMouseEnter={() => setHoveredContact('address')}
            onMouseLeave={() => setHoveredContact(null)}
                        
        >
            <LocationOnIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
            {loading ?
                <Skeleton width="50%" />
                :
                <>
                    {garageLookup?.address}, {garageLookup?.city}
                    {hoveredContact === 'address' && !isMobile &&
                        <IconButton onClick={() => copyToClipboard(`${garageLookup?.address}, ${garageLookup?.city}`)}>
                            <ContentCopyIcon fontSize="small" />
                        </IconButton>
                    }
                </>
            }
        </Typography>
        <Typography
            variant="subtitle1"
            display="flex"
            alignItems="center"
            onMouseEnter={() => setHoveredContact('email')}
            onMouseLeave={() => setHoveredContact(null)}
        >
            <MailOutlineIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
            {loading ?
                <Skeleton width="50%" />
                :
                <>
                    <Link to={`mailto:${garageLookup?.emailAddress}`}>
                        {garageLookup?.emailAddress}
                    </Link>
                    {hoveredContact === 'email' &&
                        <IconButton onClick={() => copyToClipboard(garageLookup?.emailAddress || '')}>
                            <ContentCopyIcon fontSize="small" />
                        </IconButton>
                    }
                </>
            }
        </Typography>
        <Typography
            variant="subtitle1"
            display="flex"
            alignItems="center"
            onMouseEnter={() => setHoveredContact('phone')}
            onMouseLeave={() => setHoveredContact(null)}
        >
            <PhoneIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
            {loading ?
                <Skeleton width="50%" />
                :
                <>
                    <Link to={`tel:${garageLookup?.phoneNumber}`}>
                        {garageLookup?.phoneNumber}
                    </Link>
                    {hoveredContact === 'phone' &&
                        <IconButton onClick={() => copyToClipboard(garageLookup?.phoneNumber || '')}>
                            <ContentCopyIcon fontSize="small" />
                        </IconButton>
                    }
                </>
            }
        </Typography>
        { garageLookup?.whatsappNumber &&
            <Typography
                variant="subtitle1"
                display="flex"
                alignItems="center"
                onMouseEnter={() => setHoveredContact('whatsapp')}
                onMouseLeave={() => setHoveredContact(null)}
            >
                <WhatsAppIcon style={{ marginRight: '8px', marginTop: "8px", marginBottom: "8px" }} fontSize='small' />
                <Link to={`https://wa.me/${garageLookup?.whatsappNumber}`}>
                    {garageLookup?.whatsappNumber}
                </Link>
                {hoveredContact === 'whatsapp' &&
                    <IconButton onClick={() => copyToClipboard(garageLookup?.whatsappNumber || '')}>
                        <ContentCopyIcon fontSize="small" />
                    </IconButton>
                }
            </Typography>
        }
        <GarageDailySchedule openDaysOfWeek={garageLookup?.daysOfWeek} />
    </>

};