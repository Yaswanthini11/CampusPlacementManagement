
namespace CampusPlacementManagement.Constants
{
    public static class UserRoles
    {
        public const string SuperAdmin = "SuperAdmin";

        public const string PlacementOfficer = "PlacementOfficer";

        public const string CompanyHR = "CompanyHR";

        public const string Student = "Student";

        public static readonly string[] All =
        {
            SuperAdmin,
            PlacementOfficer,
            CompanyHR,
            Student
        };
    }
}
