using Microsoft.WindowsAzure.Storage.Table;

namespace EtherData.Data.Entities
{
    public class StateEntity : TableEntity
    {
        public string Value { get; set; }
    }
}
