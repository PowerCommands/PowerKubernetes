namespace PainKiller.PowerCommands.KubernetesCommands.Managers;

public class SignatureManager
{
    private readonly IConsoleWriter _writer;
    private readonly PowerCommandsConfiguration _configuration;

    public SignatureManager(IConsoleWriter writer, PowerCommandsConfiguration configuration)
    {
        _writer = writer;
        _configuration = configuration;
    }

    public void WriteSignature(string dockerImageName)
    {
        _writer.WriteHeadLine("Follow this steps to sign a docker images");
        _writer.WriteLine($"1. If you don´t have created a root key earlier, the first step is to create a new one..");
        _writer.WriteLine($"2. Login, a docker login will be the first thing when you are starting the signature process.");
        _writer.WriteLine($"3. Make sure that you have the right DockerHubUserName set in the {nameof(PowerCommandsConfiguration)}.yaml file. (it will be used as the trusted signer)");
        _writer.WriteLine($"4. Login, a docker login will be the first thing when you are starting the signature process.");
        var continueResponse = DialogService.YesNoDialog("Do you want to continue?");
        if(!continueResponse) return;

        var directory = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "powerkubernetes\\docker");
        if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        var createRootKey = DialogService.YesNoDialog("Do you want to create a new root key?");
        if (createRootKey)
        {
            var rootKeyName = DialogService.QuestionAnswerDialog("Name your root key:");
            ShellService.Service.Execute("docker", $"trust key generate {rootKeyName}", directory, _writer.WriteLine, "", useShellExecute: true);
        }
        var keyName = DialogService.QuestionAnswerDialog("Name your signer key:");

        var createSigner = DialogService.YesNoDialog("Do you want to create a new root key?");

        var storeSecrets = DialogService.YesNoDialog("Do you want to store your secrets as an encrypted environment variable on this machine?\nSo that you can decrypt the secret key later?");
        if (createSigner)
        {
            if (storeSecrets)
            {
                var hasSecretInitialized = SecretManager.CheckEncryptConfiguration();
                if (!hasSecretInitialized)
                {
                    _writer.WriteLine("You need to initialize the secrete feature that first with this command:");
                    _writer.WriteLine( "secret --initialize\n");
                    _writer.WriteLine("Then you can re run this command again.");
                    return;
                }
                SecretManager.CreateSecret(_configuration, $"docker_key_pair_{keyName}");
            }
        }
        Environment.SetEnvironmentVariable("DOCKER_CONTENT_TRUST", "1");
        ShellService.Service.Execute("docker", $"trust key generate {keyName} --dir \"{directory}\"", directory, _writer.WriteLine, "", useShellExecute: true);

        
        
        ShellService.Service.Execute("docker", $"trust signer add {_configuration.DockerHubRepo} {_configuration.DockerHubRepo}/{dockerImageName} --key {keyName}.pub", directory, _writer.WriteLine, "");
        
        var tagName = DialogService.QuestionAnswerDialog("What is the tag name of the image you want to publish?");
        ShellService.Service.Execute("docker", $"trust sign {_configuration.DockerHubRepo}/{dockerImageName}:{tagName}", directory, _writer.WriteLine, "", useShellExecute: true);
        if(storeSecrets) SecretManager.CreateSecret(_configuration, $"docker_repo_{keyName}");
        if(storeSecrets) SecretManager.CreateSecret(_configuration, $"docker_root_{keyName}");
        ShellService.Service.Execute("docker", $"docker push {_configuration.DockerHubRepo}:{tagName}", directory, _writer.WriteLine, "");
    }
}