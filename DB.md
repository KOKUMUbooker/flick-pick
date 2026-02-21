# Entities

1. User
2. Role
3. RefreshToken
4. Group
5. UserGroup (joint table for many-many relationship for Users & Groups)
6. Vote
7. MovieSuggestion
8. MovieNightRating
9. MovieNightEvent
10. MoviePreference
11. ChatMessage

## User stories

### Auth Features

- As user I want to be able to :
  - create a user account
  - log in to the app with a verified email account
  - reset my password
  - log out of the app

### Main Features

- As user I want to be able to :
  - create a group
  - create a movie night event (if I'm the group admin)
  - provide movie suggestions in a movie night event
  - vote (upvote/downvote/veto) a movie suggestion made for a movie night event
  - rate the group MovieNightEvent once its complete
  - set my personal genre preferences
  - chat in the app
