﻿using Microsoft.AspNetCore.SignalR.Protocol;

namespace api.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new();

        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;

        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) { return Task.CompletedTask; }


                OnlineUsers[username].Remove(connectionId);


                if (OnlineUsers[username].Count == 0) { OnlineUsers.Remove(username); }

            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineusers;
            lock(OnlineUsers)
            {
                onlineusers = OnlineUsers.OrderBy( k =>k.Key).Select(k=>k.Key).ToArray();
            }

            return Task.FromResult(onlineusers);
        }


        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }

    }
}
