namespace SignalR.Chat.Models
{
    public class Group
    {
        public Group(string name) => GroupName = name; 
        public string GroupName { get; set; }
        public List<Client> Clients { get; } = new List<Client>();
    }
}
