using Microsoft.AspNetCore.SignalR;
using SignalR.Chat.Data;
using SignalR.Chat.Models;

namespace SignalR.Chat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetNickName(string nickName)
        {
            Client client = new Client(Context.ConnectionId, nickName);
            ClientSource.Clients.Add(client);

            await Clients.Others.SendAsync("clientJoined", nickName);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            Client senderClient = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            if (clientName == "To Everybody")
                await Clients.Others.SendAsync("receiveMessage", message, senderClient.Nickname);

            else
            {
                Client client = ClientSource.Clients.FirstOrDefault(c => c.Nickname == clientName);
                await Clients.Clients(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.Nickname);
            }
        }

        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(connectionId: Context.ConnectionId, groupName: groupName);

            Group group = new Group(groupName);
            group.Clients.Add(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));

            GroupSource.Groups.Add(group);

            await Clients.All.SendAsync("groups", GroupSource.Groups);
        }

        public async Task AddClientToGroup(IEnumerable<string> groupNames)
        {
            Client client = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            foreach (var group in groupNames)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == group);
                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);

                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(connectionId: Context.ConnectionId, groupName: group);
                }
            }
        }

        public async Task GetClientToGroup(string groupName)
        {
            Group group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == groupName);

            await Clients.Caller.SendAsync("clients", groupName == "-1" ? ClientSource.Clients : group.Clients);
        }

        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            Group group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == groupName);

            await Clients.Group(groupName).SendAsync("receiveMessage", message, ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId).Nickname);
        }
    }
}
