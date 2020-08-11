using System;
namespace DotNetCoreWebApiDemo.Models
{
    public interface IStudentDbSettings {
        public string Collection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }

    public class StudentDbSettings : IStudentDbSettings
    {
        public string Collection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
