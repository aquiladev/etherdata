using Microsoft.WindowsAzure.Storage.Table;

namespace EtherData.Data.Entities
{
    public class StatEntity : TableEntity
    {
        public long Count { get; set; }
    }
}
