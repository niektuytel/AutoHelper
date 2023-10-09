





export const enumToStringArray = (enumObject: any) => {
    return Object.keys(enumObject)
        .filter(key => isNaN(Number(key))) // Filter out numeric keys
        .map(key => enumObject[key as keyof typeof enumObject]);
}

export const enumToKeyArray = (enumObject: any) => {
    return Object.keys(enumObject)
        .filter(key => isNaN(Number(key))); // Filter out numeric keys
}

export const enumToKeyValueArray = (enumObject: any) => {
    return Object.keys(enumObject)
        .filter(key => isNaN(Number(key))) // Filter out numeric keys
        .map(key => ({
            key: enumObject[key as keyof typeof enumObject],
            value: key,
        }));
}