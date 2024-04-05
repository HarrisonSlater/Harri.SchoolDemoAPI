using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.ApplicationDemoAPI.Services
{
    public class ApplicationService
    {
        public ApplicationService() { }
        public int AddApplication(NewApplication newApplication) {
            return 1234;
        }

        public Application GetApplication(int applicationId)
        {
            return new Application() { Major = "Test Major"};
        }


        public void UpdateApplication(Application application) { 
            
        }
        public void DeleteApplication(int applicationId)
        {

        }
    }
}
