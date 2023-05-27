using System.Text.Json;
using PainKiller.PowerCommands.KubernetesCommands.DomainObjects;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Inspect the digest and signature of  your docker image",
                       arguments: "!image-name",
                         options: "!KEY|show-raw-data",
                         example: "inspect <image-name> --key <key-name>")]
public class InspectCommand : CommandBase<PowerCommandsConfiguration>
{
    public InspectCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var imageName = Input.SingleArgument;
        var keyName = GetOptionValue("key");
        var showRawData = HasOption("show-raw-data");
        Inspect(imageName, keyName, showRawData);
        return Ok();
    }

    public void Inspect(string imageName, string keyName, bool showRawData)
    {
        WriteHeadLine("First step is to inspect the digest of the image. ");
        ShellService.Service.Execute("docker", $"manifest inspect --verbose {imageName}", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
        var manifest = JsonSerializer.Deserialize<ContainerManifest>(LastReadLine);
        var imageDigest = manifest?.Descriptor.digest;
        WriteCodeExample("Image digest     :", $"{imageDigest}");
        if(showRawData) WriteLine(LastReadLine);
        
        ShellService.Service.Execute("docker", $"trust key load {keyName}.pub", Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "powerkubernetes\\docker"), ReadLine, "", waitForExit: true);
        //ShellService.Service.Execute("docker", $"trust key load {keyName}.key", Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "powerkubernetes\\docker"), ReadLine, "", waitForExit: true);
        WriteLine(LastReadLine);
        ShellService.Service.Execute("docker", $"trust inspect {imageName}", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
        var jsonData = $"{{ \"Signatures\": {LastReadLine}}}";
        var imageKeysMetadata = JsonSerializer.Deserialize<ImageKeysMetadata>(jsonData) ?? new ImageKeysMetadata();
        foreach (var signature in imageKeysMetadata.Signatures)
        {
            WriteLine($"Name: {signature.Name}");
            WriteHeadLine("Signed tags:");
            foreach (var signedTag in signature.SignedTags)
            {
                WriteLine($"Signed tag: {signedTag.SignedTag}");
                WriteLine($"Digest: {signedTag.Digest}");
            }
            WriteHeadLine("Signers:");
            foreach (var signer in signature.Signers) WriteLine($"Signed tag: {signer.Name}");
        }
        if(showRawData) WriteLine(LastReadLine);
    }
}