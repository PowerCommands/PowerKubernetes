namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Run command to change or view current cluster",
                         example: "//Show clusters|cluster|//Set cluster <demo> as current|cluster demo")]
public class ClusterCommand : CommandBase<PowerCommandsConfiguration>
{
    public ClusterCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var clusterName = Input.SingleArgument;
        if (string.IsNullOrEmpty(clusterName))
        {
            ShowClusters();
            return Ok();
        }
        ShellService.Service.Execute("kubectl",$"config use-context {clusterName}","", WriteLine,"", waitForExit: true);
        WriteHeadLine("Nodes");
        ShellService.Service.Execute("kubectl","get nodes","", WriteLine,"", waitForExit: true);
        WriteHeadLine("Pods");
        ShellService.Service.Execute("kubectl","get pods","", WriteLine,"", waitForExit: true);
        return Ok();
    }
    public void ShowClusters()
    {
        WriteCodeExample("kubectl",$"config get-contexts");
        ShellService.Service.Execute("kubectl",$"config get-contexts","", WriteLine,"", waitForExit: true);
    }
}