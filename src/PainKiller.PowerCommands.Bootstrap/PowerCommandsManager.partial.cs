using PainKiller.PowerCommands.KubernetesCommands.Commands;
using PainKiller.PowerCommands.Shared.Contracts;

namespace PainKiller.PowerCommands.Bootstrap;
public partial class PowerCommandsManager
{
    private void RunCustomCode()
    {
        var kCommand = (KCommand)IPowerCommandsRuntime.DefaultInstance!.Commands.First(c => c.Identifier == "k");
        kCommand.InitializeCodeCompletion();
    }
}