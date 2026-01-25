import type { Genre } from "./genre";

// export interface Movie {
//     id: number;
//     title: string;
//     genre: string;
//     imgUrl: string;
//     releaseDate: Date;
//     rating: number;
// }

export interface Movie {
    Id: number;
    Title: string;
    Minutes: number;
    Description: string;
    AgeRating: string;
    Genre: Genre;
    ImgUrl: string;
    ReleaseDate: Date;
    trailerUrl: string;
    Rating: number;
    AddedBy: string;
    CreatedAt: Date;
    verified: boolean
}