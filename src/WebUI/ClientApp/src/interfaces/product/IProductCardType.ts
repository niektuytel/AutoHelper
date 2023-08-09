
export const emptyProductCardType:ICardProductType = {
    id: 0,
    title: "",
    image: "",
    price: 0.00,
}

export default interface ICardProductType {
    id: number;
    title: string,
    image: string,
    price: number,
}