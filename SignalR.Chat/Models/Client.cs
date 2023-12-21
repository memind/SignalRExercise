namespace SignalR.Chat.Models
{
    public class Client
    {
        public Client(string connectionId, string nickname)
        {
            ConnectionId = connectionId;
            Nickname = nickname;
        }

        public string ConnectionId { get; set; }
        public string Nickname { get; set; }
    }
}
