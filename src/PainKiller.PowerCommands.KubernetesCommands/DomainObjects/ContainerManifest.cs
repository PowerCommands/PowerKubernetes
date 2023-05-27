namespace PainKiller.PowerCommands.KubernetesCommands.DomainObjects;
public class ContainerManifest
{
    public Descriptor Descriptor { get; set; }
}
public class Descriptor
{
    public string mediaType { get; set; }
    public string digest { get; set; }
    public int size { get; set; }
    public Platform platform { get; set; }
}
public class Platform
{
    public string architecture { get; set; }
    public string os { get; set; }
}