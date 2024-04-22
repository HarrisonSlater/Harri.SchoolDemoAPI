using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Repository
{
    public class SchoolRepository
    {
        public SchoolRepository() { }
        public int AddSchool(NewSchoolDto newSchool) {
            return 1234;
        }

        public SchoolDto GetSchool(int schoolId)
        {
            return new SchoolDto() { SchoolName = "Test School"};
        }


        public void UpdateSchool(SchoolDto school) { 
            
        }
        public void DeleteSchool(int schoolId)
        {

        }
    }
}
