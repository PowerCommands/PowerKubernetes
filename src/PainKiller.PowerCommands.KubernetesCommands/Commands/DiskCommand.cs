﻿using PainKiller.PowerCommands.KubernetesCommands.DomainObjects;

namespace PainKiller.PowerCommands.KubernetesCommands.Commands;

[PowerCommandTest(tests:" ")]
[PowerCommandDesign( description: "List PersistentVolumes sorted by capacity",
    arguments: "",
    example: "storage")]
public class DiskCommand : CommandBase<PowerCommandsConfiguration>
{
    public DiskCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        WriteCodeExample("kubectl",$"get pv --sort-by=.spec.capacity.storage");
        ShellService.Service.Execute("kubectl",$"get pv --sort-by=.spec.capacity.storage","", ReadLine,"", waitForExit: true);
        var response = LastReadLine;
        var rows = response.Split('\n').Skip(1).Reverse().ToList();
        if (rows.Count <= 0) return Ok();

        var details = rows.Select(r => new StorageDetailItem(r)).Where(s => !string.IsNullOrEmpty(s.Name)).ToList();
        ConsoleTableService.RenderTable(details, this);
        var total = (int)details.Sum(d => d.CapacityMegabytes) / 1024;
        WriteCodeExample("Total", $"{total} GB");
        return Ok();
    }
}