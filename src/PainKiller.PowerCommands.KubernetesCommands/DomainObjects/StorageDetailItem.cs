namespace PainKiller.PowerCommands.KubernetesCommands.DomainObjects;
public class StorageDetailItem
{
    public StorageDetailItem(){}
    public StorageDetailItem(string row)
    {
        var cols = row.Split(' ').Where(r => !string.IsNullOrEmpty(r)).ToArray();
        if(cols.Length == 0) return;
        Name = cols[0];
        Capacity = cols[1];
        Status = cols[4];
        Claim = cols[5];
    }
    public string Name { get; set; }
    public string Capacity { get; set; }
    public decimal CapacityMegabytes => Capacity.Trim().EndsWith("Gi") ? string.IsNullOrEmpty(Capacity) ? 0 : Convert.ToDecimal(Capacity.Replace("Gi", "").Trim())*1024 : string.IsNullOrEmpty(Capacity) ? 0 : Convert.ToDecimal(Capacity.Replace("Mi", "").Trim());
    public string Claim { get; set; }
    public string Status { get; set; }
}