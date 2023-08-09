

export const colorOnStatus = (status:string):string => {
    if(status === "paid")
    {
        return "#f4a305";
    }
    else if(status === "delivered")
    {
        return "#23ea0f";
    }

    return "#00000000"
}

export const colorOnRow = (index:number):string => {
    if((index % 2) == 0)
    {
        return "rgb(176 190 197 / 13%)";
    }

    return "#00000000"
}

export const colorOnIndex = (index:number) : string =>
{
    return ColorValues[index % ColorValues.length]
}

export const ColorValues = [
    "#65B243",
    "#E40B5E",
    "#6E2EA9",
    "#FEE133",
    "#FE8E01",
    "#FD0801"
]