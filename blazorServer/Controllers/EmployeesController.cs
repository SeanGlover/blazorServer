using Microsoft.AspNetCore.Mvc;
using blazorServer.Models;
using blazorServer.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace blazorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        public EmployeesController(IEmployeeService service) { employeeService = service; }
        
        // GET: api/<EmployeesController>
        [HttpGet]
        public ActionResult<List<Employee>> Get() { return employeeService.Get(); }

        // GET api/<EmployeesController>/5
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(string id)
        {
            Employee employee = employeeService.Get(id);
            return employee == null ? NotFound($"Employee with Id = {id} not found") : employee;
        }

        // POST api/<EmployeesController>
        [HttpPost]
        public ActionResult<Employee> Post([FromBody] Employee employee)
        {
            employeeService.Create(employee);
            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        // PUT api/<EmployeesController>/
        [HttpPut("{id}")]
        public ActionResult<Employee> Put(string id, [FromBody] Employee employee)
        {
            var existingEmployee = employeeService.Get(id);
            if (existingEmployee == null) { return NotFound($"Employee with Id = {id} not found"); }
            else { employeeService.Update(id, existingEmployee); return NoContent(); }
        }

        // DELETE api/<EmployeesController>/
        [HttpDelete("{id}")]
        public ActionResult<Employee> Delete(string id)
        {
            var existingEmployee = employeeService.Get(id);
            if (existingEmployee == null) { return NotFound($"Employee with Id = {id} not found"); }
            else { employeeService.Remove(id); return Ok($"Employee with Id = {id} deleted"); }
        }
    }
}
