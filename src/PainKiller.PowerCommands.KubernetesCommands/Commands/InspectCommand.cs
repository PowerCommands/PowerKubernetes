using System.Text.Json;
using PainKiller.PowerCommands.KubernetesCommands.DomainObjects;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Inspect the digest and signature of  your docker image",
                         options: ".!key",
                         example: "inspect <image-name> --key <key-name>")]
public class InspectCommand : CommandBase<PowerCommandsConfiguration>
{
    public InspectCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var imageName = Input.SingleArgument;
        var keyName = GetOptionValue("key");
        Inspect(imageName, keyName);
        return Ok();
    }

    public void Inspect(string imageName, string keyName)
    {
        WriteHeadLine("First step is to inspect the digest of the image. ");
        ShellService.Service.Execute("docker", $"manifest inspect --verbose {imageName}", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
        var manifest = JsonSerializer.Deserialize<ContainerManifest>(LastReadLine);
        var imageDigest = manifest?.Descriptor.digest;
        WriteCodeExample("Image digest     :", $"{imageDigest}");
        
        ShellService.Service.Execute("docker", $"trust inspect --pretty {imageName}", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
        
        ShellService.Service.Execute("docker", $"trust inspect --pretty --key <public-key> <image-name>", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
        WriteLine(LastReadLine);
    }
}