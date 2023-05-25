namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Description of your command...",
                         example: "demo")]
public class SignatureCommand : CommandBase<PowerCommandsConfiguration>
{
    public SignatureCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        WriteSignature();
        return Ok();
    }

    public void WriteSignature()
    {
        WriteHeadLine("Follow this steps to sign a docker images");
        WriteCodeExample("1.",$"Login, a docker login will be the first thing when you are starting the signature process.");
        WriteCodeExample("2.",$"Make sure that you have the right DockerHubUserName set in the {nameof(PowerCommandsConfiguration)}.yaml file. (it will be used as the trusted signer)");
        WriteCodeExample("3.",$"In this process 3 different secret keys will be provided by you, you can store them afterwards if you want. I call dem sign key, repo key and root key, they are created in that order.");
        var continueResponse = DialogService.YesNoDialog("Do you want to continue?");
        if(!continueResponse) return;

        var directory = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "powerkubernetes\\docker");
        if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        var keyName = DialogService.QuestionAnswerDialog("Name your key");

        Environment.SetEnvironmentVariable("DOCKER_CONTENT_TRUST", "1");
        ShellService.Service.Execute("docker", $"trust key generate {keyName}", directory, ReadLine, "", useShellExecute: true, disableOutputLogging: true);
    }
}