using blazorServer.Models;
using MongoDB.Driver;
namespace blazorServer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;
        public EmployeeService(IEmployeeStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _employees = database.GetCollection<Employee>(settings.EmployeesCollectionName);
        }
        public Employee Create(Employee employee)
        {
            _employees.InsertOne(employee);
            return employee;
        }
        public List<Employee> Get() { return _employees.Find(employee => true).ToList(); }
        public Employee Get(string id) { return _employees.Find(employee => employee.Id == id).FirstOrDefault(); }
        public void Remove(string id) { _employees.DeleteOne(employee => employee.Id == id); }
        public void Update(string id, Employee employee) { _employees.ReplaceOne(student => student.Id == id, employee); }
    }
}
