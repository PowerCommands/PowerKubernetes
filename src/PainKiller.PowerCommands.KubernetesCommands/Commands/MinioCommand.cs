using PainKiller.PowerCommands.Core.Commands;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Setup a new instance of MinIO with Docker Desktop",
                         options: "remove",
                         example: "minio")]
public class MinioCommand : CommandBase<PowerCommandsConfiguration>
{
    public MinioCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var containerName = "minio1";
        if(HasOption("remove")) Remove(containerName);
        else
        {
            var path = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "Docker", "data");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            ShellService.Service.Execute("docker",$"run -p 9000:9000 -p 9090:9090 --name {containerName} -v {path}:/data -e \"MINIO_ROOT_USER=minioadmin\" -e \"MINIO_ROOT_PASSWORD=minioadmin\" quay.io/minio/minio:latest server /data --console-address \":9090",CdCommand.WorkingDirectory, WriteLine,"", waitForExit: false);
            WriteSuccessLine("MinIO container spinning up...");
            Thread.Sleep(5000);
            ShellService.Service.OpenWithDefaultProgram("http://localhost:9000");
        }
        return Ok();
    }

    private void Remove(string containerName)
    {
        ShellService.Service.Execute("docker","stop minio1",CdCommand.WorkingDirectory, WriteLine,"", waitForExit: false);
        WriteSuccessLine("MinIO container minio1 stopped.");
        ShellService.Service.Execute("docker","rm minio1",CdCommand.WorkingDirectory, WriteLine,"", waitForExit: false);
        WriteSuccessLine("MinIO container minio1 removed.");

    }
}