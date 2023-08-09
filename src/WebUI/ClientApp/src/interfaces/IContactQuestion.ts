export const emptyContactQuestion:IContactQuestion = {
    id: -1,
    askedAmount: 0,
    question: "",
    answer: ""
}

export default interface IContactQuestion {
    id: number;
    askedAmount: number;
    question: string;
    answer: string;
}
