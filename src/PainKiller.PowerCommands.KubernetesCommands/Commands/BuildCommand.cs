using PainKiller.PowerCommands.Core.Commands;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Command to build your docker image",
                       arguments: "!image-name",
                         example: "build <image-name>")]
public class BuildCommand : CdCommand
{
    private readonly PowerCommandsConfiguration _configuration;

    public BuildCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => _configuration = configuration;

    public override RunResult Run()
    {
        var imageName = Input.SingleArgument;
        ShellService.Service.Execute("docker", $"build -t {_configuration.DockerHubRepo}/{imageName} .", WorkingDirectory, WriteLine, "", useShellExecute: true);
        return Ok();
    }
}