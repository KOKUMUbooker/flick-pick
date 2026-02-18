import type { Group, MovieNightEvent } from "../types";

export const mockGroups: Group[] = [
    {
        id: 1,
        name: 'Friday Film Club',
        description: 'Weekly movie nights with close friends',
        createdById: 1,
        members: [
            { id: 1, name: 'Alex Johnson', isAdmin: true, avatar: '', joinedAt: '2024-01-15' },
            { id: 2, name: 'Sam Taylor', isAdmin: false, avatar: '', joinedAt: '2024-01-15' },
            { id: 3, name: 'Jordan Lee', isAdmin: false, avatar: '', joinedAt: '2024-01-20' },
            { id: 4, name: 'Taylor Smith', isAdmin: false, avatar: '', joinedAt: '2024-02-01' },
            { id: 5, name: 'Morgan Wells', isAdmin: false, avatar: '', joinedAt: '2024-02-10' }
        ],
        unreadCount: 3,
        upcomingEventsCount: 2,
        lastActivity: '10 min ago'
    },
    {
        id: 2,
        name: 'Horror Nights',
        description: 'Monthly scary movie sessions',
        createdById: 1,
        members: [
            { id: 1, name: 'Alex Johnson', isAdmin: true, avatar: '', joinedAt: '2024-01-15' },
            { id: 6, name: 'Casey Kim', isAdmin: false, avatar: '', joinedAt: '2024-02-05' },
            { id: 7, name: 'Riley Chen', isAdmin: false, avatar: '', joinedAt: '2024-02-05' }
        ],
        unreadCount: 0,
        upcomingEventsCount: 0,
        lastActivity: '2 days ago'
    },
    {
        id: 3,
        name: 'Family Movie Night',
        description: 'Kid-friendly weekend movies',
        createdById: 1,
        members: [
            { id: 1, name: 'Alex Johnson', isAdmin: true, avatar: '', joinedAt: '2024-01-15' },
            { id: 8, name: 'Jamie Wilson', isAdmin: false, avatar: '', joinedAt: '2024-02-10' },
            { id: 9, name: 'Taylor Jr', isAdmin: false, avatar: '', joinedAt: '2024-02-10' },
            { id: 10, name: 'Morgan Jr', isAdmin: false, avatar: '', joinedAt: '2024-02-10' }
        ],
        unreadCount: 1,
        upcomingEventsCount: 1,
        lastActivity: '1 hour ago'
    },
    {
        id: 4,
        name: 'Oscar Watch Party',
        description: 'Annual Academy Awards viewing',
        createdById: 1,
        members: [
            { id: 1, name: 'Alex Johnson', isAdmin: true, avatar: '', joinedAt: '2024-01-15' },
            { id: 2, name: 'Sam Taylor', isAdmin: false, avatar: '', joinedAt: '2024-01-15' },
            { id: 3, name: 'Jordan Lee', isAdmin: false, avatar: '', joinedAt: '2024-01-20' },
            { id: 11, name: 'Drew Patel', isAdmin: false, avatar: '', joinedAt: '2024-02-15' }
        ],
        unreadCount: 0,
        upcomingEventsCount: 0,
        lastActivity: '3 weeks ago'
    }
];

type EventItem = {
    upcoming: MovieNightEvent[];
    past: MovieNightEvent[];
}
type StatsItem = {
    moviesWatched: number;
    totalVotes: number;
    averageRating: number;
    streak: number;
}
interface LoadGroupDataInput {
    groupId: number;
    eventCb: (data: EventItem) => void;
    statsCb: (data: StatsItem) => void;
}
export function loadGroupData(data: LoadGroupDataInput) {
    let events: EventItem = { upcoming: [], past: [] };
    let stats: StatsItem = { moviesWatched: 0, totalVotes: 0, averageRating: 0, streak: 0 };

    if (data.groupId === 1) {
        const now = new Date();
        const tomorrow = new Date(now);
        tomorrow.setDate(tomorrow.getDate() + 1);
        const saturday = new Date(now);
        saturday.setDate(saturday.getDate() + (6 - now.getDay()));

        events = {
            upcoming: [
                {
                    id: 101,
                    groupId: 1,
                    scheduledAt: tomorrow.toISOString(),
                    isLocked: false,
                    title: 'Sci-Fi Night',
                    suggestions: [
                        {
                            id: 1001,
                            tmdbId: 693134,
                            movieDetails: {
                                tmdbId: 693134,
                                title: 'Dune: Part Two',
                                overview: 'Follow the mythic journey of Paul Atreides...',
                                posterPath: '/1pdfLvkbY9ohJlCjQH2CZjjYVvJ.jpg',
                                runtime: 166,
                                releaseDate: '2024-02-28'
                            },
                            suggestedBy: { id: 1, name: 'Alex' },
                            votes: [
                                { id: 1, userId: 1, user: { id: 1, name: 'Alex' }, voteType: 'Upvote' },
                                { id: 2, userId: 2, user: { id: 2, name: 'Sam' }, voteType: 'Upvote' },
                                { id: 3, userId: 3, user: { id: 3, name: 'Jordan' }, voteType: 'Downvote' }
                            ],
                            isDisqualified: false,
                            score: 1
                        },
                        {
                            id: 1002,
                            tmdbId: 157336,
                            movieDetails: {
                                tmdbId: 157336,
                                title: 'Interstellar',
                                overview: 'A team of explorers travel through a wormhole...',
                                posterPath: '/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg',
                                runtime: 169,
                                releaseDate: '2014-11-05'
                            },
                            suggestedBy: { id: 2, name: 'Sam' },
                            votes: [
                                { id: 4, userId: 2, user: { id: 2, name: 'Sam' }, voteType: 'Upvote' },
                                { id: 5, userId: 4, user: { id: 4, name: 'Taylor' }, voteType: 'Upvote' }
                            ],
                            isDisqualified: false,
                            score: 2
                        },
                        {
                            id: 1003,
                            tmdbId: 329865,
                            movieDetails: {
                                tmdbId: 329865,
                                title: 'Arrival',
                                overview: 'A linguist works with the military to communicate...',
                                posterPath: '/x2FJsf1ElRukx0iPdXitrBrGBlq.jpg',
                                runtime: 116,
                                releaseDate: '2016-11-10'
                            },
                            suggestedBy: { id: 3, name: 'Jordan' },
                            votes: [{ id: 6, userId: 5, user: { id: 5, name: 'Morgan' }, voteType: 'Veto' }],
                            isDisqualified: true,
                            score: -1000
                        }
                    ],
                    chatMessages: [
                        {
                            id: 1,
                            userId: 1,
                            user: { id: 1, name: 'Alex', avatar: '' },
                            message: "I'm really excited for Dune part 2! Anyone else?",
                            createdAt: '2 hours ago'
                        },
                        {
                            id: 2,
                            userId: 2,
                            user: { id: 2, name: 'Sam', avatar: '' },
                            message: 'Yes! The first one was amazing. Hope it wins the vote.',
                            createdAt: '1 hour ago'
                        },
                        {
                            id: 3,
                            userId: 3,
                            user: { id: 3, name: 'Jordan', avatar: '' },
                            message: 'Just a heads up - Arrival is 2 hours, hope that works for everyone',
                            createdAt: '45 min ago'
                        }
                    ],
                    ratings: [],
                    participants: 4
                },
                {
                    id: 102,
                    groupId: 1,
                    scheduledAt: saturday.toISOString(),
                    isLocked: false,
                    title: 'Comedy Special',
                    suggestions: [
                        {
                            id: 1004,
                            tmdbId: 153987,
                            movieDetails: {
                                tmdbId: 153987,
                                title: 'The Holdovers',
                                overview: 'A cranky history teacher at a prep school...',
                                posterPath: '/aIQF7H44uCLw0eQxHLR6Sxg3k5l.jpg',
                                runtime: 133,
                                releaseDate: '2023-10-27'
                            },
                            suggestedBy: { id: 1, name: 'Alex' },
                            votes: [],
                            isDisqualified: false,
                            score: 0
                        }
                    ],
                    chatMessages: [],
                    ratings: [],
                    participants: 5
                }
            ],
            past: [
                {
                    id: 103,
                    groupId: 1,
                    scheduledAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
                    isLocked: true,
                    selectedMovieTmdbId: 4935,
                    selectedMovie: {
                        tmdbId: 4935,
                        title: 'Hereditary',
                        overview: 'A grieving family is haunted...',
                        posterPath: '/lHV3oVwM4rCQh7ajK7MZvL3gk3j.jpg',
                        runtime: 127,
                        releaseDate: '2018-06-07'
                    },
                    title: 'Horror Night',
                    suggestions: [],
                    chatMessages: [
                        {
                            id: 10,
                            userId: 1,
                            user: { id: 1, name: 'Alex', avatar: '' },
                            message: 'That was intense! What did everyone think?',
                            createdAt: '1 week ago'
                        },
                        {
                            id: 11,
                            userId: 3,
                            user: { id: 3, name: 'Jordan', avatar: '' },
                            message: 'Still processing it... 😱',
                            createdAt: '1 week ago'
                        }
                    ],
                    ratings: [
                        {
                            id: 1,
                            userId: 1,
                            user: { id: 1, name: 'Alex' },
                            rating: 5,
                            comment: 'Masterpiece'
                        },
                        {
                            id: 2,
                            userId: 2,
                            user: { id: 2, name: 'Sam' },
                            rating: 4,
                            comment: 'Too scary for me!'
                        },
                        { id: 3, userId: 3, user: { id: 3, name: 'Jordan' }, rating: 5 }
                    ],
                    participants: 3
                }
            ]
        };

        stats = {
            moviesWatched: 8,
            totalVotes: 42,
            averageRating: 4.2,
            streak: 3
        };
    }
    data.eventCb(events);
    data.statsCb(stats);
}