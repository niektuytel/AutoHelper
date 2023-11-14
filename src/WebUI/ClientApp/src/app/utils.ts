
export const enumToKeyValueArray = (enumObject: any) => {
    return Object.keys(enumObject)
        .filter(key => isNaN(Number(key))) // Filter out numeric keys
        .map(key => ({
            key: enumObject[key as keyof typeof enumObject],
            value: key,
        }));
}