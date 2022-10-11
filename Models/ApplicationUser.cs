using Microsoft.AspNetCore.Identity;

namespace GeneralSalesDb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string EmployeeName { get; set; }
        public string ImagePath { get; set; }
        public int DepartmentId { get; set; }
        //public ApplicationUser()
        //{
        //    Employee = new HashSet<Employee>();

        //}
        //public ICollection<Employee> Employee { get; set; }

    }
}
