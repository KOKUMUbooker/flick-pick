import type { MovieSuggestion } from "../types/movie";
import { VoteType } from "../types/movie";

// Helper function to generate random IDs
const generateId = () => Math.random().toString(36).substring(2, 15);

// Sample users
const users = [
    { id: 'user1', fullName: 'Alex Chen', email: 'alex.chen@email.com' },
    { id: 'user2', fullName: 'Sarah Johnson', email: 'sarah.j@email.com' },
    { id: 'user3', fullName: 'Mike Rodriguez', email: 'mike.r@email.com' },
    { id: 'user4', fullName: 'Emma Watson', email: 'emma.w@email.com' },
    { id: 'user5', fullName: 'David Kim', email: 'david.kim@email.com' },
    { id: 'user6', fullName: 'Lisa Patel', email: 'lisa.p@email.com' },
    { id: 'user7', fullName: 'James Wilson', email: 'james.w@email.com' },
    { id: 'user8', fullName: 'Nina Gupta', email: 'nina.g@email.com' }
];

// Dummy movie suggestions
export const movieSuggestions: MovieSuggestion[] = [
    {
        id: generateId(),
        tmdbId: 157336,
        movieDetails: {
            tmdbId: 157336,
            title: 'Interstellar',
            overview: 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity\'s survival.',
            posterPath: '/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg',
            runtime: 169,
            releaseDate: '2014-11-05'
        },
        suggestedBy: { fullName: 'Alex Chen', email: 'alex.chen@email.com' },
        votes: [
            {
                userId: 'user2',
                movieSuggestionId: '1',
                voteType: VoteType.Upvote,
                user: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' }
            },
            {
                userId: 'user3',
                movieSuggestionId: '1',
                voteType: VoteType.Upvote,
                user: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' }
            },
            {
                userId: 'user4',
                movieSuggestionId: '1',
                voteType: VoteType.Upvote,
                user: { fullName: 'Emma Watson', email: 'emma.w@email.com' }
            },
            {
                userId: 'user5',
                movieSuggestionId: '1',
                voteType: VoteType.Downvote,
                user: { fullName: 'David Kim', email: 'david.kim@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 27205,
        movieDetails: {
            tmdbId: 27205,
            title: 'Inception',
            overview: 'A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.',
            posterPath: '/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg',
            runtime: 148,
            releaseDate: '2010-07-15'
        },
        suggestedBy: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' },
        votes: [
            {
                userId: 'user1',
                movieSuggestionId: '2',
                voteType: VoteType.Upvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            },
            {
                userId: 'user6',
                movieSuggestionId: '2',
                voteType: VoteType.Upvote,
                user: { fullName: 'Lisa Patel', email: 'lisa.p@email.com' }
            },
            {
                userId: 'user7',
                movieSuggestionId: '2',
                voteType: VoteType.Upvote,
                user: { fullName: 'James Wilson', email: 'james.w@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 603,
        movieDetails: {
            tmdbId: 603,
            title: 'The Matrix',
            overview: 'A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.',
            posterPath: '/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg',
            runtime: 136,
            releaseDate: '1999-03-30'
        },
        suggestedBy: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' },
        votes: [
            {
                userId: 'user1',
                movieSuggestionId: '3',
                voteType: VoteType.Upvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            },
            {
                userId: 'user2',
                movieSuggestionId: '3',
                voteType: VoteType.Upvote,
                user: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' }
            },
            {
                userId: 'user5',
                movieSuggestionId: '3',
                voteType: VoteType.Veto,
                user: { fullName: 'David Kim', email: 'david.kim@email.com' }
            }
        ],
        isDisqualified: true
    },
    {
        id: generateId(),
        tmdbId: 155,
        movieDetails: {
            tmdbId: 155,
            title: 'The Dark Knight',
            overview: 'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.',
            posterPath: '/qJ2tW6WMUDux911r6m7haRef0WH.jpg',
            runtime: 152,
            releaseDate: '2008-07-16'
        },
        suggestedBy: { fullName: 'Emma Watson', email: 'emma.w@email.com' },
        votes: [
            {
                userId: 'user3',
                movieSuggestionId: '4',
                voteType: VoteType.Upvote,
                user: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' }
            },
            {
                userId: 'user6',
                movieSuggestionId: '4',
                voteType: VoteType.Upvote,
                user: { fullName: 'Lisa Patel', email: 'lisa.p@email.com' }
            },
            {
                userId: 'user7',
                movieSuggestionId: '4',
                voteType: VoteType.Upvote,
                user: { fullName: 'James Wilson', email: 'james.w@email.com' }
            },
            {
                userId: 'user8',
                movieSuggestionId: '4',
                voteType: VoteType.Upvote,
                user: { fullName: 'Nina Gupta', email: 'nina.g@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 24428,
        movieDetails: {
            tmdbId: 24428,
            title: 'The Avengers',
            overview: 'Earth\'s mightiest heroes must come together and learn to fight as a team if they are to stop the mischievous Loki and his alien army from enslaving humanity.',
            posterPath: '/RYMX2wcKCBAr24UyPD7xwmjaTn.jpg',
            runtime: 143,
            releaseDate: '2012-04-25'
        },
        suggestedBy: { fullName: 'David Kim', email: 'david.kim@email.com' },
        votes: [
            {
                userId: 'user1',
                movieSuggestionId: '5',
                voteType: VoteType.Downvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            },
            {
                userId: 'user2',
                movieSuggestionId: '5',
                voteType: VoteType.Downvote,
                user: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' }
            },
            {
                userId: 'user4',
                movieSuggestionId: '5',
                voteType: VoteType.Upvote,
                user: { fullName: 'Emma Watson', email: 'emma.w@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 680,
        movieDetails: {
            tmdbId: 680,
            title: 'Pulp Fiction',
            overview: 'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.',
            posterPath: '/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg',
            runtime: 154,
            releaseDate: '1994-09-10'
        },
        suggestedBy: { fullName: 'Lisa Patel', email: 'lisa.p@email.com' },
        votes: [
            {
                userId: 'user3',
                movieSuggestionId: '6',
                voteType: VoteType.Upvote,
                user: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' }
            },
            {
                userId: 'user5',
                movieSuggestionId: '6',
                voteType: VoteType.Upvote,
                user: { fullName: 'David Kim', email: 'david.kim@email.com' }
            },
            {
                userId: 'user7',
                movieSuggestionId: '6',
                voteType: VoteType.Downvote,
                user: { fullName: 'James Wilson', email: 'james.w@email.com' }
            },
            {
                userId: 'user8',
                movieSuggestionId: '6',
                voteType: VoteType.Veto,
                user: { fullName: 'Nina Gupta', email: 'nina.g@email.com' }
            }
        ],
        isDisqualified: true
    },
    {
        id: generateId(),
        tmdbId: 13,
        movieDetails: {
            tmdbId: 13,
            title: 'Forrest Gump',
            overview: 'The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man with an IQ of 75, whose only desire is to be reunited with his childhood sweetheart.',
            posterPath: '/arw2vcBve4WxIZvR4N39An3PEDN.jpg',
            runtime: 142,
            releaseDate: '1994-06-23'
        },
        suggestedBy: { fullName: 'James Wilson', email: 'james.w@email.com' },
        votes: [
            {
                userId: 'user2',
                movieSuggestionId: '7',
                voteType: VoteType.Upvote,
                user: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' }
            },
            {
                userId: 'user4',
                movieSuggestionId: '7',
                voteType: VoteType.Upvote,
                user: { fullName: 'Emma Watson', email: 'emma.w@email.com' }
            },
            {
                userId: 'user6',
                movieSuggestionId: '7',
                voteType: VoteType.Upvote,
                user: { fullName: 'Lisa Patel', email: 'lisa.p@email.com' }
            },
            {
                userId: 'user1',
                movieSuggestionId: '7',
                voteType: VoteType.Downvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 78,
        movieDetails: {
            tmdbId: 78,
            title: 'Blade Runner 2049',
            overview: 'Young Blade Runner K\'s discovery of a long-buried secret leads him to track down former Blade Runner Rick Deckard, who\'s been missing for thirty years.',
            posterPath: '/gajva2L0rPYkEWjzgFlBXCAVBE5.jpg',
            runtime: 164,
            releaseDate: '2017-10-04'
        },
        suggestedBy: { fullName: 'Nina Gupta', email: 'nina.g@email.com' },
        votes: [
            {
                userId: 'user1',
                movieSuggestionId: '8',
                voteType: VoteType.Upvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            },
            {
                userId: 'user3',
                movieSuggestionId: '8',
                voteType: VoteType.Upvote,
                user: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' }
            },
            {
                userId: 'user5',
                movieSuggestionId: '8',
                voteType: VoteType.Upvote,
                user: { fullName: 'David Kim', email: 'david.kim@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 122,
        movieDetails: {
            tmdbId: 122,
            title: 'The Lord of the Rings: The Return of the King',
            overview: 'Gandalf and Aragorn lead the World of Men against Sauron\'s army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.',
            posterPath: '/rCzpDGLbOoPwLjy3OAm5NUPOTrC.jpg',
            runtime: 201,
            releaseDate: '2003-12-01'
        },
        suggestedBy: { fullName: 'Alex Chen', email: 'alex.chen@email.com' },
        votes: [
            {
                userId: 'user2',
                movieSuggestionId: '9',
                voteType: VoteType.Upvote,
                user: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' }
            },
            {
                userId: 'user4',
                movieSuggestionId: '9',
                voteType: VoteType.Upvote,
                user: { fullName: 'Emma Watson', email: 'emma.w@email.com' }
            },
            {
                userId: 'user6',
                movieSuggestionId: '9',
                voteType: VoteType.Upvote,
                user: { fullName: 'Lisa Patel', email: 'lisa.p@email.com' }
            },
            {
                userId: 'user8',
                movieSuggestionId: '9',
                voteType: VoteType.Upvote,
                user: { fullName: 'Nina Gupta', email: 'nina.g@email.com' }
            }
        ],
        isDisqualified: false
    },
    {
        id: generateId(),
        tmdbId: 581,
        movieDetails: {
            tmdbId: 581,
            title: 'Dune: Part Two',
            overview: 'Follow the mythic journey of Paul Atreides as he unites with Chani and the Fremen while on a warpath of revenge against the conspirators who destroyed his family.',
            posterPath: '/1bn8n9ZgoT0GEXpfsiQ8VkHdsVq.jpg',
            runtime: 166,
            releaseDate: '2024-02-28'
        },
        suggestedBy: { fullName: 'Sarah Johnson', email: 'sarah.j@email.com' },
        votes: [
            {
                userId: 'user1',
                movieSuggestionId: '10',
                voteType: VoteType.Upvote,
                user: { fullName: 'Alex Chen', email: 'alex.chen@email.com' }
            },
            {
                userId: 'user3',
                movieSuggestionId: '10',
                voteType: VoteType.Upvote,
                user: { fullName: 'Mike Rodriguez', email: 'mike.r@email.com' }
            },
            {
                userId: 'user5',
                movieSuggestionId: '10',
                voteType: VoteType.Upvote,
                user: { fullName: 'David Kim', email: 'david.kim@email.com' }
            },
            {
                userId: 'user7',
                movieSuggestionId: '10',
                voteType: VoteType.Upvote,
                user: { fullName: 'James Wilson', email: 'james.w@email.com' }
            }
        ],
        isDisqualified: false
    }
];

// Helper function to get suggestions by group/filter
export const getSuggestionsByStatus = {
    active: () => movieSuggestions.filter(s => !s.isDisqualified),
    disqualified: () => movieSuggestions.filter(s => s.isDisqualified),
    withMostVotes: () => [...movieSuggestions].sort((a, b) => b.votes.length - a.votes.length),
    withMostUpvotes: () => [...movieSuggestions].sort((a, b) => {
        const aUpvotes = a.votes.filter(v => v.voteType === VoteType.Upvote).length;
        const bUpvotes = b.votes.filter(v => v.voteType === VoteType.Upvote).length;
        return bUpvotes - aUpvotes;
    })
};

