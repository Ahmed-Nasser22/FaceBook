using api.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUsserName() , Context.ConnectionId);
            await Clients.Others.SendAsync("user online", Context.User.GetUsserName());

             var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUsserName(), Context.ConnectionId);
            await Clients.Others.SendAsync("user offline", Context.User.GetUsserName());

            var currentUsers = _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);



            await base.OnDisconnectedAsync(exception);
        }
    }
}
