using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Document
    {
        public int DocumentId { get; set; }
        public string DocumentDetails { get; set; }
        public string DocumentPicture { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
