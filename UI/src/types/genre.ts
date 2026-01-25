// Basic Genre type
export type Genre =
    | 'Action'
    | 'Adventure'
    | 'Animation'
    | 'Biography'
    | 'Comedy'
    | 'Crime'
    | 'Documentary'
    | 'Drama'
    | 'Family'
    | 'Fantasy'
    | 'Film-Noir'
    | 'History'
    | 'Horror'
    | 'Music'
    | 'Musical'
    | 'Mystery'
    | 'Romance'
    | 'Sci-Fi'
    | 'Sport'
    | 'Thriller'
    | 'War'
    | 'Western';

// Basic array of all genres
export const ALL_GENRES: Genre[] = [
    'Action',
    'Adventure',
    'Animation',
    'Biography',
    'Comedy',
    'Crime',
    'Documentary',
    'Drama',
    'Family',
    'Fantasy',
    'Film-Noir',
    'History',
    'Horror',
    'Music',
    'Musical',
    'Mystery',
    'Romance',
    'Sci-Fi',
    'Sport',
    'Thriller',
    'War',
    'Western'
];

// For dropdown/select component
export const GENRE_OPTIONS = ALL_GENRES.map(genre => ({
    value: genre,
    label: genre,
    description: getGenreDescription(genre)
}));

function getGenreDescription(genre: Genre): string {
    const descriptions: Record<Genre, string> = {
        'Action': 'High-energy films with physical stunts',
        'Adventure': 'Exciting journeys and explorations',
        'Animation': 'Animated films and cartoons',
        'Biography': 'Films based on real people\'s lives',
        'Comedy': 'Funny and humorous films',
        'Crime': 'Films about criminal activities',
        'Documentary': 'Non-fiction educational films',
        'Drama': 'Serious, emotional storylines',
        'Family': 'Films suitable for all ages',
        'Fantasy': 'Magical and supernatural elements',
        'Film-Noir': 'Stylish crime dramas from the 1940s-50s',
        'History': 'Historical events and periods',
        'Horror': 'Scary and frightening films',
        'Music': 'Music-centered storylines',
        'Musical': 'Films with song and dance numbers',
        'Mystery': 'Puzzles and investigations',
        'Romance': 'Love stories and relationships',
        'Sci-Fi': 'Science fiction and futuristic themes',
        'Sport': 'Sports-related stories',
        'Thriller': 'Suspenseful and exciting films',
        'War': 'Military conflicts and battles',
        'Western': 'American frontier stories'
    };
    return descriptions[genre];
}