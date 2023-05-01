using PainKiller.PowerCommands.Core.Commands;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Run kubeseal commands",
                         options: "seal|output-certificate|initialize-control-plane|sleep-time",
              overrideHelpOption: true,
                         example: "kubeseal")]
public class KubesealCommand : MasterCommando
{
    private static string _lastCertificate = "";
    public KubesealCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        if (HasOption("initialize-control-plane"))
        {
            ShellService.Service.Execute("kubectl", "apply -f SealedSecretController.yaml", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
            WriteLine($"{LastReadLine}");
            return Ok();
        }
        if (HasOption("seal"))
        {
            var sleepTime = Input.OptionToInt("sleep-time", 1000);

            ShellService.Service.Execute("kubeseal", "--fetch-cert > public-key-cert.pem", AppContext.BaseDirectory, ReadLine, "", waitForExit: true);
            if(HasOption("output-certificate")) WriteLine($"{LastReadLine}");
            _lastCertificate = LastReadLine;

            var fileName = Path.Combine(WorkingDirectory, GetOptionValue("seal"));
            if (!File.Exists(fileName)) return BadParameterError($"{fileName} does not exists.");


            var outputFileName = $"{fileName.ToLower().Replace(".yaml", "")}_sealed.yaml";

            var certFileName = Path.Combine(WorkingDirectory, "public-key-cert.pem");
            File.WriteAllText(certFileName, _lastCertificate);
            var batchFileName = Path.Combine(WorkingDirectory, "command.bat");
            File.WriteAllText(batchFileName, $"kubeseal --format=yaml --cert={certFileName} < {fileName} > {outputFileName}");
            
            ShellService.Service.OpenWithDefaultProgram(batchFileName, WorkingDirectory);

            Thread.Sleep(sleepTime);

            File.Delete(certFileName);
            WriteProcessLog("delete file", $"File {certFileName} deleted");
            File.Delete(batchFileName);
            WriteProcessLog("delete file", $"File {batchFileName} deleted");
            WriteSuccessLine($"File [{outputFileName}] saved to disk.");
            return Ok();
        }
        
        return base.Run();
    }
}