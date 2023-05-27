using PainKiller.PowerCommands.KubernetesCommands.Managers;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Sign and publish your docker image or inspect the integrity of an existing docker image",
                         options: "inspect",
                         example: "//Sign and publish your docker image|signature|//Verify that a docker image is signed and that the signature is ok.|//signature --verify <image-name>")]
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
        var repoName = DialogService.QuestionAnswerDialog("What is the name of your docker hub repo?");
        var dockerImageName = DialogService.QuestionAnswerDialog("What is the name of the image you want to sign?");
        var tagName = DialogService.QuestionAnswerDialog("What is the tag name of the image you want to publish?");

        Environment.SetEnvironmentVariable("DOCKER_CONTENT_TRUST", "1");
        ShellService.Service.Execute("docker", $"trust key generate {keyName}", directory, ReadLine, "", useShellExecute: true);

        var storeSecrets = DialogService.YesNoDialog("Do you want to store your secrets as an encrypted environment variable on this machine?\nSo that you can decrypt the secret key later?");
        if (storeSecrets)
        {
            var hasSecretInitialized = SecretManager.CheckEncryptConfiguration();
            if (!hasSecretInitialized)
            {
                WriteLine("You need to initialize the secrete feature that first with this command:");
                WriteCodeExample("secret", "--initialize\n");
                WriteLine("Then you can re run this command again.");
                return;
            }
            SecretManager.CreateSecret(Configuration, $"docker_key_pair_{keyName}");
        }
        ShellService.Service.Execute("docker", $"trust signer add {Configuration.DockerHubUserName} {repoName}/{dockerImageName} --key {keyName}.pub", directory, ReadLine, "");
        ShellService.Service.Execute("docker", $"trust sign {repoName}/{dockerImageName}:{tagName}", directory, ReadLine, "");
        if(storeSecrets) SecretManager.CreateSecret(Configuration, $"docker_repo_{keyName}");
        if(storeSecrets) SecretManager.CreateSecret(Configuration, $"docker_root_{keyName}");
        ShellService.Service.Execute("docker", $"docker push {repoName}:{tagName}", directory, ReadLine, "");
    }
}