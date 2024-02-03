
export const getLicenseFromPath = (path: string): string | undefined => {
    const patterns = [
        /[A-Z]{2}-\d{3}-[A-Z]/,
        /[A-Z]-\d{3}-[A-Z]{2}/,
        /[A-Z]{3}-\d{2}-[A-Z]/,
        /\d-[A-Z]{2}-\d{3}/,
        /[A-Z]{2}-\d{2}-[A-Z]{2}/,
        /\d{2}-[A-Z]{3}-\d/,
        /[A-Z]{2}-\d{2}-\d{2}/,
        /\d{2}-\d{2}-[A-Z]{2}/,
        /[A-Z]{2}-[A-Z]{2}-\d{2}/
    ];

    for (const pattern of patterns) {
        const match = path.match(pattern);
        if (match) {
            return match[0];
        }
    }
    return undefined;
}

export const getFormatedLicense = (license: string): string => {

    switch (license.length) {
        case 6:
            return /^[A-Z]{2}\d{3}[A-Z]$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 5)}-${license.slice(5)}`
                :
                /^[A-Z]\d{3}[A-Z]{2}$/.test(license) ? `${license.slice(0, 1)}-${license.slice(1, 4)}-${license.slice(4)}`
                    :
                    /^[A-Z]{3}\d{2}[A-Z]$/.test(license) ? `${license.slice(0, 3)}-${license.slice(3, 5)}-${license.slice(5)}`
                        :
                        /^\d[A-Z]{2}\d{3}$/.test(license) ? `${license.slice(0, 1)}-${license.slice(1, 3)}-${license.slice(3)}`
                            :
                            /^[A-Z]{2}\d{2}[A-Z]{2}$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 4)}-${license.slice(4)}`
                                :
                                /^\d{2}[A-Z]{3}\d$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 5)}-${license.slice(5)}`
                                    :
                                    /^[A-Z]{2}\d{2}\d{2}$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 4)}-${license.slice(4, 6)}`
                                        :
                                        /^\d{2}\d{2}[A-Z]{2}$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 4)}-${license.slice(4, 6)}`
                                            :
                                            /^[A-Z]{2}[A-Z]{2}\d{2}$/.test(license) ? `${license.slice(0, 2)}-${license.slice(2, 4)}-${license.slice(4, 6)}`
                                                :
                                                license;
        case 7:
            return `${license.slice(0, 3)}-${license.slice(3, 5)}-${license.slice(5)}`;
    }

    return license;
}