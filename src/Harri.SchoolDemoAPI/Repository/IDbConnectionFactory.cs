using System.Data;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}