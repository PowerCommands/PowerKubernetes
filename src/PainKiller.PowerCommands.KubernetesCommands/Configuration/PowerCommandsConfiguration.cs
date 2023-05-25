namespace PainKiller.PowerCommands.KubernetesCommands.Configuration;
public class PowerCommandsConfiguration : CommandsConfiguration
{
    public string DefaultGitRepositoryPath { get; set; } = "C:\\repo";
    public string PathToDockerDesktop { get; set; } = "";
    public string DockerHubUserName { get; set; } = string.Empty;
    public string LoginShellCommand { get; set; } = "";
}