using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class ExpenceType
    {
        public ExpenceType()
        {
            Expence = new HashSet<Expence>();
        }

        public int ExpenceTypeId { get; set; }
        public string ExpenceType1 { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Expence> Expence { get; set; }
    }
}
