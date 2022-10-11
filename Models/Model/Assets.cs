using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Assets
    {
        public int AssetId { get; set; }
        public double? AfAsset { get; set; }
        public double? PkAsset { get; set; }
        public double? UsAsset { get; set; }
        public int? Dates { get; set; }
        public string Desc { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
