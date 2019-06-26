namespace Yei3.PersonalEvaluation.Core
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    public class BizConst
    {
        public const string OperationsArea = "Operaciones";
        public const string StoreManagerJobDescription = "Gerente de tienda";
        public const string DistrictManagerJobDescription = "Gerente de Distrito";
        public const string ZoneManagerJobDescription = "Gerente de Zona";
        public const string RegionManagerJobDescription = "Director Regional";

        public static readonly ReadOnlyCollection<string> SalesManJobDescriptions = new ReadOnlyCollection<string>(new List<string>
        {
            StoreManagerJobDescription, DistrictManagerJobDescription, ZoneManagerJobDescription, RegionManagerJobDescription
        });
    }
}