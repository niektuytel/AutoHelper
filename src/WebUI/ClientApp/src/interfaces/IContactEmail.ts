
export const emptyContactEmail:IContactEmail = {
    email: "",
    subject: "",
    content: ""
}

export default interface IContactEmail {
    email: string;
    subject: string;
    content: string;
}