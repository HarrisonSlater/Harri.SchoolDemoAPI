using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.ApplicationDemoAPI.Repository
{
    public class ApplicationRepository
    {
        public ApplicationRepository() { }
        public int AddApplication(NewApplicationDto newApplication) {
            return 1234;
        }

        public ApplicationDto GetApplication(int applicationId)
        {
            return new ApplicationDto() { Major = "Test Major"};
        }


        public void UpdateApplication(ApplicationDto application) { 
            
        }
        public void DeleteApplication(int applicationId)
        {

        }
    }
}
