export interface CreateRatingFormData {
    Rating: number;
    Comment?: string
}
export interface CreateRatingData extends CreateRatingFormData {
    ConnectionId: string
}
