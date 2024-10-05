using System.Threading;
using System.Threading.Tasks;

interface ICommand {
    Task Execute(string[] args, CancellationToken cancellationToken);
}
