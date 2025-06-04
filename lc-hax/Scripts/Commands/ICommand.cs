using System.Threading;
using System.Threading.Tasks;

interface ICommand {
    Task Execute(Arguments args, CancellationToken cancellationToken);
}
