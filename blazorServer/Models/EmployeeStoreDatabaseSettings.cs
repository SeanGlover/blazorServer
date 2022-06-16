namespace blazorServer.Models
{
    public interface IEmployeeStoreDatabaseSettings
    {
        string EmployeesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
    public class EmployeeStoreDatabaseSettings : IEmployeeStoreDatabaseSettings
    {
        public string EmployeesCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
