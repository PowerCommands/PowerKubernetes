using PainKiller.PowerCommands.KubernetesCommands.DomainObjects;
using System.Text;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Publish your kubernetes application(s)",
                         options: "!name|!namespace",
                         example: "publish --name bootcamp")]
public class PublishCommand : CommandBase<PowerCommandsConfiguration>
{
    public PublishCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var name = GetOptionValue("name");
        var path = Path.Combine(Configuration.KubernetesDeploymentFilesRoot, name);
        var nspace = GetOptionValue("namespace");
        var dirInfo = new DirectoryInfo(path);

        //var babar = new ProcessMetadata { Name = "\"{{babar}}\"" };
        //StorageService<ProcessMetadata>.Service.StoreObject(babar);

        if (string.IsNullOrEmpty(name) && dirInfo.Exists)
        {
            WriteHeadLine("Kubernetes projects");
            foreach (var directoryInfo in dirInfo.GetDirectories())
            {
                WriteLine(directoryInfo.Name);
            }
            return Ok();
        }

        var yamlFiles = Directory.GetFiles(path, "*.yaml").OrderBy(f => f).ToList();
        foreach (var fileName in yamlFiles)
        {
            var nmnSpace = nspace;
            if (!string.IsNullOrEmpty(nmnSpace) && !fileName.ToLower().Contains("namespace")) nmnSpace = $"-n {nspace}";
            else nmnSpace = "";
            var fileInfo = new FileInfo(fileName);
            ShellService.Service.Execute("kubectl",$"apply {nmnSpace} -f {fileInfo.FullName}",dirInfo.FullName, WriteLine,"", waitForExit: true);
            WriteSuccessLine($"{fileInfo.Name} applied OK");
        }

        if (!string.IsNullOrEmpty(nspace))
        {
            ShellService.Service.Execute("kubectl",$"config set-context --current --namespace={nspace}","", WriteLine,"", waitForExit: true);
            nspace = $"-n {nspace}";
        }
        ShellService.Service.Execute("kubectl", $"get services {nspace}", dirInfo.FullName, WriteLine, "", waitForExit: true);

        var jsonFiles = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToList();
        foreach (var fileName in jsonFiles)
        {
            var fileInfo = new FileInfo(fileName);
            var processMetadata = StorageService<ProcessMetadata>.Service.GetObject(fileName);
            if (string.IsNullOrEmpty(processMetadata.Url))
            {
                var applicationName = processMetadata.Name.Replace(".exe", "").Replace(".bat", "").Replace(".cmd", "");
                if (processMetadata.UseReadline)
                {
                    ShellService.Service.Execute(applicationName, processMetadata.Args, dirInfo.FullName, ReadLine, "", waitForExit: processMetadata.WaitForExit, useShellExecute: processMetadata.UseShellExecute, disableOutputLogging: processMetadata.DisableOutputLogging);
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(LastReadLine));
                    Console.WriteLine(decoded);
                }
                else ShellService.Service.Execute(applicationName, processMetadata.Args, dirInfo.FullName, WriteLine, "", waitForExit: processMetadata.WaitForExit, useShellExecute: processMetadata.UseShellExecute, disableOutputLogging: processMetadata.DisableOutputLogging);
            }
            else
            {
                var url = processMetadata.Url.Replace(".exe", "").Replace(".bat", "").Replace(".cmd", "");
                ShellService.Service.OpenWithDefaultProgram(url);
            }
            WriteLine(processMetadata.Description);
            WriteSuccessLine($"{fileInfo.Name} executed OK");
        }
        return Ok();
    }
}