using PainKiller.PowerCommands.Core.Commands;
using PainKiller.PowerCommands.KubernetesCommands.DomainObjects;
using PainKiller.PowerCommands.KubernetesCommands.Managers;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Manage your Docker images",
                       arguments: "!<image-name>",
                         options: "search|build|delete|sign",
                         example: "//Build image, image file must exist in current working directory|image <image-name> --build")]
public class ImageCommand : CdCommand
{
    private readonly PowerCommandsConfiguration _configuration;
    public ImageCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => _configuration = configuration;

    public override RunResult Run()
    {
        var imageName = Input.SingleArgument;
        if(HasOption("build")) BuildImage(imageName);
        else if(HasOption("sign")) WriteSignature(imageName);
        else if(HasOption("search")) SearchImage(imageName);
        return Ok();
    }

    private void WriteSignature(string imageName)
    {
        var signatureManager = new SignatureManager(this, _configuration, WorkingDirectory);
        signatureManager.WriteSignature(imageName);
    }
    private void BuildImage(string imageName) => ShellService.Service.Execute("docker", $"build -t {_configuration.DockerHubRepo}/{imageName} .", WorkingDirectory, WriteLine, "", useShellExecute: true);

    private void SearchImage(string imageName)
    {
        ShellService.Service.Execute("docker", "images", WorkingDirectory, ReadLine, waitForExit: true);
        var rows = LastReadLine.Split('\n').Where(r => r.Contains(imageName)).Select(r => new DockerImagesTableRow{DockerImageRow = r});
        ConsoleTableService.RenderTable(rows, this);
    }
}