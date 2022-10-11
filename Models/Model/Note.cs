using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Note
    {
        public int NoteId { get; set; }
        public string UserId { get; set; }
        public string Note1 { get; set; }
        public string NoteStatus { get; set; }
        public DateTime? RemindDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
