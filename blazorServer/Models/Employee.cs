using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.RegularExpressions;
using blazorServer.Shared;

namespace blazorServer.Models
{
    [BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set
            {
                string newId = value ?? string.Empty;
                if (newId.Length == 24) { _id = newId; }
                else
                {
                    _id = Guid.NewGuid().ToString();
                    _id = Regex.Replace(_id, "[^0-9]", string.Empty, RegexOptions.None);
                    _id = Regex.Match(_id, "[0-9]{24}").Value;
                }
            }
        }

        [BsonElement("employeeid")]
        public int EmployeeId { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [BsonElement("gender")]
        public Gender Gender { get; set; }

        [BsonElement("departmentId")]
        public int DepartmentId { get; set; }

        [BsonElement("photoPath")]
        public string PhotoPath { get; set; } = string.Empty;

        //[BsonElement("department")]
        //public Department? Department { get; set; } = null;
    }
}
