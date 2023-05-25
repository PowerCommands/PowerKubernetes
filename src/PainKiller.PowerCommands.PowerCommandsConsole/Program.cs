using PainKiller.PowerCommands.Core.Services;

ConsoleService.Service.WriteLine(nameof(Program), @"__________                             ____  __.    ___.                               __                 
\______   \______  _  __ ___________  |    |/ _|__ _\_ |__   ___________  ____   _____/  |_  ____   ______
 |     ___/  _ \ \/ \/ // __ \_  __ \ |      < |  |  \ __ \_/ __ \_  __ \/    \_/ __ \   __\/ __ \ /  ___/
 |    |  (  <_> )     /\  ___/|  | \/ |    |  \|  |  / \_\ \  ___/|  | \/   |  \  ___/|  | \  ___/ \___ \ 
 |____|   \____/ \/\_/  \___  >__|    |____|__ \____/|___  /\___  >__|  |___|  /\___  >__|  \___  >____  >
                            \/                \/         \/     \/           \/     \/          \/     \/ ", ConsoleColor.Cyan);
ConsoleService.Service.WriteHeaderLine(nameof(Program),"Power Kubernetes Version 1.0");
PainKiller.PowerCommands.Bootstrap.Startup.ConfigureServices().Run(args);