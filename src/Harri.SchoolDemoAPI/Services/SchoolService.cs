using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Services
{
    public class SchoolService
    {
        public SchoolService() { }
        public int AddSchool(NewSchool newSchool) {
            return 1234;
        }

        public School GetSchool(int schoolId)
        {
            return new School() { SchoolName = "Test School"};
        }


        public void UpdateSchool(School school) { 
            
        }
        public void DeleteSchool(int schoolId)
        {

        }
    }
}
