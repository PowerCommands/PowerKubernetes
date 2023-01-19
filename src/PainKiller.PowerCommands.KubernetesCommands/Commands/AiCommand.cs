namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Run go firefly AI ac queries using optimized OpenAI API queries",
                          quotes: "<Query>",
                         options: "pull-image",
                         example: "//Docker needs to pull the image before you start using it|aiac --pull-image|aiac \"get dockerfile for nodejs with comments\"")]
public class AiCommand : CommandBase<PowerCommandsConfiguration>
{
    public AiCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        if (HasOption("pull-image")) PullAndSpinupDockerImage();
        RunQuery(Input.SingleQuote);
        return Ok();
    }

    private void RunQuery(string query)
    {
        ShellService.Service.Execute("docker", arguments: $"run -it -e OPENAI_API_KEY={Configuration.Environment.GetValue("aiac")} {Configuration.GoFireflyioAiacDockerImage} {query}", workingDirectory: "", WriteLine, fileExtension: "", useShellExecute: true);
    }
    private void PullAndSpinupDockerImage()
    {
        var fullFileName = Path.Combine(Configuration.PathToDockerDesktop, "Docker Desktop.exe");
        ShellService.Service.Execute(fullFileName, arguments: "", workingDirectory: "", WriteLine, fileExtension: "");
        WriteSuccessLine("Docker Desktop started");
        WriteCodeExample("docker",$"Docker Desktop needs to pull image {Configuration.GoFireflyioAiacDockerImage}");
        ShellService.Service.Execute("docker", arguments: $"pull {Configuration.GoFireflyioAiacDockerImage}", workingDirectory: "", WriteLine, fileExtension: "", waitForExit: true);
        for (int i = 0; i < 10; i++)
        {
            OverwritePreviousLine($"Docker Desktop needs to spin up the image, waiting a {10-i} seconds, before executing your query ...");
            Thread.Sleep(1000);
        }

    }
}