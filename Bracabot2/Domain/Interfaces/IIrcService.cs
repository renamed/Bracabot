namespace Bracabot2.Domain.Interfaces
{
    public interface IIrcService
    {
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }

        Task ConnectAsync();
        Task<string> GetMessageAsync();
        Task SendMessageAsync(string message);
    }
}
