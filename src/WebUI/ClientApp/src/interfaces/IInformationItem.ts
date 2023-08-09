

export const emptyNewsItem:IInformationItem = {
    id: 0,
    title: "",
    content: "",
    image: "",
    createdAt: ""
}

export default interface IInformationItem {
    id:number;
    title:string;
    content: string;
    image: string;
    createdAt: string;
}