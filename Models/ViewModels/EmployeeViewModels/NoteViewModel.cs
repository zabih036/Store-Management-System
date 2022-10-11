using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class NoteViewModel
    {
        public int NoteId { get; set; }

        [Required(ErrorMessage = " لطفآ خپل یاد داښت ولیکئ!!")]
        [Display(Name = "یاد داښت:")]
        public string Note { get; set; }

        /////////////////////////////////////////////////////////////
        [Required(ErrorMessage = "د یاد داښت لپاره نیټه وټاکئ!!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "نیټه:")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }
        ///////////////////////////////////////////////////////////
        [Required(ErrorMessage = "د یاد داښت د وریادولو نیټه وټاکئ!!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "د یادولو نیټه:")]
        [DataType(DataType.Date)]
        public DateTime RemindDate { get; set; }
    }
}
