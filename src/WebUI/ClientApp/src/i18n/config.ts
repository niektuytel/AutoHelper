import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import { EN, NL } from "./LanguageKeys";

i18n.use(initReactI18next).init({
    fallbackLng: EN,
    lng: NL,
    resources: {
        en: {
            translations: require('./locales/en/translations.json')
        },
        nl: {
            translations: require('./locales/nl/translations.json')
        }
    },
    ns: ['translations'],
    defaultNS: 'translations'
});

i18n.languages = [EN, NL];

export default i18n;