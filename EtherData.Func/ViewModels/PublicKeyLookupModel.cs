namespace EtherData.Func.ViewModels
{
    public class PublicKeyLookupModel
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        public EnsModel Ens { get; set; }
    }

    public class EnsModel
    {
        public string Name { get; set; }

        public string Hash { get; set; }

        public string Resolver { get; set; }
    }
}
