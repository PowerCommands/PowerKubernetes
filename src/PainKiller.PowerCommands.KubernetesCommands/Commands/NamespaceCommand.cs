namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandDesign( description: "Change the namespace for the current context",
                       arguments: "!<namespace name>",
                         options: "delete",
                         example: "namespace my-namespace-name|namespace my-namespace-name --delete")]
public class NamespaceCommand : CommandBase<PowerCommandsConfiguration>
{
    public NamespaceCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        var nSpaceName = Input.SingleArgument;
        if(HasOption("delete")) Delete(nSpaceName);
        else
        {
            WriteCodeExample("kubectl",$"config set-context --current --namespace={nSpaceName}");
            ShellService.Service.Execute("kubectl",$"config set-context --current --namespace={nSpaceName}","", WriteLine,"", waitForExit: true);
        }
        return Ok();
    }
    private void Delete(string namespaceName)
    {
        if (namespaceName.ToLower() == "default")
        {
            WriteError("You can not delete default namespace");
            return;
        }
        ShellService.Service.Execute("kubectl",$"config set-context --current --namespace={namespaceName}","", WriteLine,"", waitForExit: true);
        ShellService.Service.Execute("kubectl",$"get all","", WriteLine,"", waitForExit: true);
        var areYouSure = DialogService.YesNoDialog("Are you sure you want to delete the namespace with all resources within it?");
        if (!areYouSure) return;
        WriteCodeExample("kubectl", $"delete namespaces {namespaceName}");
        ShellService.Service.Execute("kubectl", $"delete namespaces {namespaceName}", "", WriteLine, "", waitForExit: true);
    }
}