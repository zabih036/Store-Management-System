﻿using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class HalfContract
    {
        public HalfContract()
        {
            HalfContractItems = new HashSet<HalfContractItems>();
        }

        public int HalfContractId { get; set; }
        public int? DealerId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public int? CurrencyId { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public DateTime? Date { get; set; }
        public string Finished { get; set; }
        public int? DepartmentId { get; set; }
        public string ContractType { get; set; }
        public string Desc { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<HalfContractItems> HalfContractItems { get; set; }
    }
}
