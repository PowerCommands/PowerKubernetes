namespace PainKiller.PowerCommands.KubernetesCommands.DomainObjects;

public class ImageKeysMetadata
{
    public Signatures[] Signatures { get; set; }
}
public class Signatures
{
    public string Name { get; set; }
    public Signedtag[] SignedTags { get; set; }
    public Signer[] Signers { get; set; }
    public Administrativekey[] AdministrativeKeys { get; set; }
}

public class Signedtag
{
    public string SignedTag { get; set; }
    public string Digest { get; set; }
    public string[] Signers { get; set; }
}

public class Signer
{
    public string Name { get; set; }
    public Key[] Keys { get; set; }
}

public class Key
{
    public string ID { get; set; }
}

public class Administrativekey
{
    public string Name { get; set; }
    public Key1[] Keys { get; set; }
}

public class Key1
{
    public string ID { get; set; }
}
