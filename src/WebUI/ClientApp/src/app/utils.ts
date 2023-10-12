




// Use like: enumToKeyValueArray(GarageServiceType).slice(1).map(({ key, value }) => // Display all enum items except the first one (None)
export const enumToKeyValueArray = (enumObject: any) => {
    return Object.keys(enumObject)
        .filter(key => isNaN(Number(key))) // Filter out numeric keys
        .map(key => ({
            key: enumObject[key as keyof typeof enumObject],
            value: key,
        }));
}