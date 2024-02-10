import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import { EN, NL } from "./LanguageKeys";

i18n.use(initReactI18next).init({
    fallbackLng: EN,
    lng: NL,
    resources: {
        en: {
            translations: require('./locales/en/translations.json'),
            serviceTypes: require('./locales/en/serviceTypes.json'),
            serviceFuelTypes: require('./locales/en/serviceFuelTypes.json')
        },
        nl: {
            translations: require('./locales/nl/translations.json'),
            serviceTypes: require('./locales/nl/serviceTypes.json'),
            serviceFuelTypes: require('./locales/nl/serviceFuelTypes.json')
        }
    },
    ns: ['translations', 'serviceTypes', 'serviceFuelTypes'],
    defaultNS: 'translations'
});

i18n.languages = [EN, NL];

export default i18n;