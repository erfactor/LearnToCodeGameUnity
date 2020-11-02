using Models;

namespace Commands
{
    public interface ICommand
    {
        int Execute(Board board, Bot bot);
        int NextCommandId { get; }
    }
}