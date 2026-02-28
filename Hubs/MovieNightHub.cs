namespace WatchHive.Hubs;

using Microsoft.AspNetCore.SignalR;

public class MovieNightHub : Hub 
{ 
    public async Task JoinMovieNight(string movieNightId) 
    { 
        await Groups.AddToGroupAsync(Context.ConnectionId, movieNightId);
    } 
    
    public async Task LeaveMovieNight(string movieNightId) 
    { 
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, movieNightId); 
    } 
}