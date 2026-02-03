import type { Movie } from "../types/movie";
import type { Genre } from '../types/genre';

export const stats = {
    totalMovies: 0,
    avgRating: 0,
    recentAdditions: 0
};

export const dummyMovies: Movie[] = [
    {
        Id: 1,
        Title: "Hereditary",
        Minutes: 127,
        Description: "A grieving family is haunted by tragic and disturbing occurrences following the death of their secretive grandmother.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/lHV8HHlhwNup2VbPiDCLq3XovTm.jpg",
        ReleaseDate: new Date("2018-06-08"),
        Rating: 7.3,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=V6wWKNij_1M",
        CreatedAt: new Date("2024-01-15"),
        Verified: true
    },
    {
        Id: 2,
        Title: "A Quiet Place",
        Minutes: 90,
        Description: "In a post-apocalyptic world, a family is forced to live in silence while hiding from monsters with ultra-sensitive hearing.",
        AgeRating: "PG-13",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/nAU74GmpUk7t5iklEp3bufwDq4n.jpg",
        ReleaseDate: new Date("2018-04-06"),
        Rating: 7.5,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=WR7cc5t7tv8",
        CreatedAt: new Date("2024-01-16"),
        Verified: true
    },
    {
        Id: 3,
        Title: "Rec",
        Minutes: 78,
        Description: "A television reporter and cameraman follow emergency workers into a dark apartment building and are quickly locked inside with something terrifying.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/nAU74GmpUk7t5iklEp3bufwDq4n.jpg",
        ReleaseDate: new Date("2007-11-23"),
        Rating: 7.4,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=0Y4Lxh7nYBE",
        CreatedAt: new Date("2024-01-17"),
        Verified: true
    },
    {
        Id: 4,
        Title: "The Substance",
        Minutes: 140,
        Description: "A fading celebrity decides to use a black market drug, a cell-replicating substance that temporarily creates a younger, better version of herself.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/2tGTTSCxHQGgGQbK9R6JLpQ6Wsi.jpg",
        ReleaseDate: new Date("2024-09-07"), // Cannes premiere
        Rating: 8.2,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=9BvF3YlZ0_o",
        CreatedAt: new Date("2024-01-18"),
        Verified: true
    },
    {
        Id: 5,
        Title: "Smile",
        Minutes: 115,
        Description: "After witnessing a bizarre, traumatic incident involving a patient, a psychiatrist becomes increasingly convinced she is being threatened by an uncanny entity.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/67Myda9zANAnlS54rRjQF4dHNNG.jpg",
        ReleaseDate: new Date("2022-09-30"),
        Rating: 6.7,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=BcDK7kTzz9w",
        CreatedAt: new Date("2024-01-19"),
        Verified: true
    },
    {
        Id: 6,
        Title: "Smile 2",
        Minutes: 118,
        Description: "A global pop star is forced to face her past when she begins experiencing increasingly terrifying and inexplicable events.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/rULWuutDcN5NvtiZi4FRPzRYWSh.jpg",
        ReleaseDate: new Date("2024-10-16"),
        Rating: 6.9,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=T8GbycK2pXk",
        CreatedAt: new Date("2024-01-20"),
        Verified: true
    },
    {
        Id: 7,
        Title: "Evil Dead",
        Minutes: 91,
        Description: "Five friends head to a remote cabin, where the discovery of a Book of the Dead leads them to unwittingly summon up demons living in the nearby woods.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/wM7DURS7vU1z8LhWUeKqKtHpH6X.jpg",
        ReleaseDate: new Date("2013-04-05"),
        Rating: 6.5,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=Q-CpalU4xys",
        CreatedAt: new Date("2024-01-21"),
        Verified: true
    },
    {
        Id: 8,
        Title: "Evil Dead Rise",
        Minutes: 96,
        Description: "A twisted tale of two estranged sisters whose reunion is cut short by the rise of flesh-possessing demons, thrusting them into a primal battle for survival.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/5ik4ATKmNtmJU6AYD0bLm56BCVM.jpg",
        ReleaseDate: new Date("2023-04-21"),
        Rating: 6.5,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=BqQNO7BzN08",
        CreatedAt: new Date("2024-01-22"),
        Verified: true
    },
    {
        Id: 9,
        Title: "Infinity Pool",
        Minutes: 117,
        Description: "While staying at an isolated island resort, James and Em are enjoying a perfect vacation of pristine beaches, exceptional staff, and soaking up the sun.",
        AgeRating: "R",
        Genre: "Horror",
        ImgUrl: "https://image.tmdb.org/t/p/w500/gzpp3xF6uh8SfZQf9yf0m2Syi5R.jpg",
        ReleaseDate: new Date("2023-01-21"),
        Rating: 6.1,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=VCk2gZ14TAk",
        CreatedAt: new Date("2024-01-23"),
        Verified: true
    },
    {
        Id: 10,
        Title: "Interstellar",
        Minutes: 169,
        Description: "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
        AgeRating: "PG-13",
        Genre: "Sci-Fi",
        ImgUrl: "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
        ReleaseDate: new Date("2014-11-07"),
        Rating: 8.6,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=zSWdZVtXT7E",
        CreatedAt: new Date("2024-01-24"),
        Verified: true
    },
    {
        Id: 11,
        Title: "X-Men: Days of Future Past",
        Minutes: 132,
        Description: "The X-Men send Wolverine to the past in a desperate effort to change history and prevent an event that results in doom for both humans and mutants.",
        AgeRating: "PG-13",
        Genre: "Action",
        ImgUrl: "https://image.tmdb.org/t/p/w500/tYfijzolzgoMOtegh2K2FmKus5p.jpg",
        ReleaseDate: new Date("2014-05-23"),
        Rating: 7.9,
        AddedBy: "admin",
        TrailerUrl: "https://www.youtube.com/watch?v=pK2zYHWDZKo",
        CreatedAt: new Date("2024-01-25"),
        Verified: true
    },
    {
        Id: 12,
        Title: "The Dark Knight",
        Minutes: 152,
        Description: "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
        AgeRating: "PG-13",
        Genre: "Action",
        ImgUrl: "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
        ReleaseDate: new Date("2008-07-18"),
        Rating: 9.0,
        AddedBy: "movie_fan",
        TrailerUrl: "https://www.youtube.com/watch?v=EXeTwQWrcwY",
        CreatedAt: new Date("2024-01-26"),
        Verified: true
    },
    {
        Id: 13,
        Title: "Inception",
        Minutes: 148,
        Description: "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
        AgeRating: "PG-13",
        Genre: "Sci-Fi",
        ImgUrl: "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
        ReleaseDate: new Date("2010-07-16"),
        Rating: 8.8,
        AddedBy: "movie_fan",
        TrailerUrl: "https://www.youtube.com/watch?v=YoHD9XEInc0",
        CreatedAt: new Date("2024-01-27"),
        Verified: true
    },
    {
        Id: 14,
        Title: "Parasite",
        Minutes: 132,
        Description: "Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.",
        AgeRating: "R",
        Genre: "Comedy",
        ImgUrl: "https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
        ReleaseDate: new Date("2019-05-30"),
        Rating: 8.6,
        AddedBy: "critic_user",
        TrailerUrl: "https://www.youtube.com/watch?v=5xH0HfJHsaY",
        CreatedAt: new Date("2024-01-28"),
        Verified: true
    },
    {
        Id: 15,
        Title: "The Shawshank Redemption",
        Minutes: 142,
        Description: "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
        AgeRating: "R",
        Genre: "Drama",
        ImgUrl: "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
        ReleaseDate: new Date("1994-09-23"),
        Rating: 9.3,
        AddedBy: "classic_cinema",
        TrailerUrl: "https://www.youtube.com/watch?v=6hB3S9bIaco",
        CreatedAt: new Date("2024-01-29"),
        Verified: true
    },
    {
        Id: 16,
        Title: "Pulp Fiction",
        Minutes: 154,
        Description: "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
        AgeRating: "R",
        Genre: "Crime",
        ImgUrl: "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
        ReleaseDate: new Date("1994-10-14"),
        Rating: 8.9,
        AddedBy: "classic_cinema",
        TrailerUrl: "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
        CreatedAt: new Date("2024-01-30"),
        Verified: true
    },
    {
        Id: 17,
        Title: "The Matrix",
        Minutes: 136,
        Description: "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
        AgeRating: "R",
        Genre: "Sci-Fi",
        ImgUrl: "https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg",
        ReleaseDate: new Date("1999-03-31"),
        Rating: 8.7,
        AddedBy: "scifi_fan",
        TrailerUrl: "https://www.youtube.com/watch?v=vKQi3bBA1y8",
        CreatedAt: new Date("2024-01-31"),
        Verified: true
    },
    {
        Id: 18,
        Title: "Spirited Away",
        Minutes: 125,
        Description: "During her family's move to the suburbs, a sullen 10-year-old girl wanders into a world ruled by gods, witches, and spirits.",
        AgeRating: "PG",
        Genre: "Animation",
        ImgUrl: "https://image.tmdb.org/t/p/w500/39wmItIWsg5sZMyRUHLkWBcuVCM.jpg",
        ReleaseDate: new Date("2001-07-20"),
        Rating: 8.6,
        AddedBy: "anime_lover",
        TrailerUrl: "https://www.youtube.com/watch?v=ByXuk9QqQkk",
        CreatedAt: new Date("2024-02-01"),
        Verified: true
    },
    {
        Id: 19,
        Title: "The Lord of the Rings: The Return of the King",
        Minutes: 201,
        Description: "Gandalf and Aragorn lead the World of Men against Sauron's army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.",
        AgeRating: "PG-13",
        Genre: "Fantasy",
        ImgUrl: "https://image.tmdb.org/t/p/w500/rCzpDGLbOoPwLjy3OAm5NUPOTrC.jpg",
        ReleaseDate: new Date("2003-12-17"),
        Rating: 8.9,
        AddedBy: "fantasy_fan",
        TrailerUrl: "https://www.youtube.com/watch?v=r5X-hFf6Bwo",
        CreatedAt: new Date("2024-02-02"),
        Verified: true
    },
    {
        Id: 20,
        Title: "La La Land",
        Minutes: 128,
        Description: "While navigating their careers in Los Angeles, a pianist and an actress fall in love while attempting to reconcile their aspirations for the future.",
        AgeRating: "PG-13",
        Genre: "Musical",
        ImgUrl: "https://image.tmdb.org/t/p/w500/uDO8zWDhfWwoFdKS4fzkUJt0Rf0.jpg",
        ReleaseDate: new Date("2016-12-09"),
        Rating: 8.0,
        AddedBy: "musical_fan",
        TrailerUrl: "https://www.youtube.com/watch?v=0pdqf4P9MB8",
        CreatedAt: new Date("2024-02-03"),
        Verified: true
    }
];

// Utility functions
export const movieUtils = {
    // Get movie by ID
    getMovieById: (id: number): Movie | undefined => {
        return dummyMovies.find(movie => movie.Id === id);
    },

    // Get movies by genre
    getMoviesByGenre: (genre: Genre): Movie[] => {
        return dummyMovies.filter(movie => movie.Genre === genre);
    },

    // Get recent movies (last 30 days)
    getRecentMovies: (): Movie[] => {
        const thirtyDaysAgo = new Date();
        thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
        return dummyMovies.filter(movie => movie.CreatedAt >= thirtyDaysAgo);
    },

    // Get top rated movies
    getTopRatedMovies: (limit: number = 10): Movie[] => {
        return [...dummyMovies]
            .sort((a, b) => b.Rating - a.Rating)
            .slice(0, limit);
    },

    // Get movies by year
    getMoviesByYear: (year: number): Movie[] => {
        return dummyMovies.filter(movie => movie.ReleaseDate.getFullYear() === year);
    },

    // Search movies by title or description
    searchMovies: (query: string): Movie[] => {
        const lowerQuery = query.toLowerCase();
        return dummyMovies.filter(movie =>
            movie.Title.toLowerCase().includes(lowerQuery) ||
            movie.Description.toLowerCase().includes(lowerQuery) ||
            movie.Genre.toLowerCase().includes(lowerQuery)
        );
    },

    // Get random movie
    getRandomMovie: (): Movie => {
        return dummyMovies[Math.floor(Math.random() * dummyMovies.length)];
    },

    // Get statistics
    getStats: () => {
        const genres = new Set(dummyMovies.map(movie => movie.Genre));
        const totalMinutes = dummyMovies.reduce((sum, movie) => sum + movie.Minutes, 0);
        const avgRating = dummyMovies.reduce((sum, movie) => sum + movie.Rating, 0) / dummyMovies.length;

        return {
            totalMovies: dummyMovies.length,
            totalGenres: genres.size,
            totalWatchTime: `${Math.floor(totalMinutes / 60)}h ${totalMinutes % 60}m`,
            averageRating: avgRating.toFixed(1),
            verifiedMovies: dummyMovies.filter(m => m.Verified).length,
            recentAdditions: dummyMovies.filter(m => {
                const thirtyDaysAgo = new Date();
                thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
                return m.CreatedAt >= thirtyDaysAgo;
            }).length
        };
    }
};

export type { Genre };