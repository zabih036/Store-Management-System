namespace GeneralSalesDb.Models.ViewModels
{
    public class ClaimViewModel
    {
        //public ClaimViewModel()
        //{
        //    Claims = new List<UserClaim>();
        //}
        public int UserId { get; set; }
        public string Email { get; set; }

        public bool None { get; set; }
        public bool View { get; set; }
        public bool Insert { get; set; }
        public bool Edit { get; set; }
        public bool Delet { get; set; }
    }
}
