using Dapper;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Repository;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Harri.SchoolDempAPI.Tests.Common
{
    public class InMemorySQLiteTestDatabase : IDbConnectionFactory, IDisposable
    {
        private readonly SqliteConnection _connection;
        public InMemorySQLiteTestDatabase() {
            if (_connection is not null)
            {
                return;
            }

            using (var connection2 = GetConnection())
            {
                _connection = new SqliteConnection($"Data Source=InMemorySchoolDemoDatabase;Mode=Memory;Cache=Shared");
                _connection.Open();
                connection2.Execute(@"
ATTACH DATABASE 'main' As 'SchoolDemo';

DROP TABLE [SchoolDemo].Application;
DROP TABLE [SchoolDemo].Student;
DROP TABLE [SchoolDemo].School;


CREATE TABLE [SchoolDemo].Student (
    [sID] int PRIMARY KEY NOT NULL,
    [sName] varchar(255) NOT NULL,
    [GPA] decimal(5,2) NULL
);

INSERT INTO [SchoolDemo].Student VALUES 
(001, 'Amy', 3.9),
(002, 'Doris', 3.9),
(003, 'Fay', 3.8),
(123, 'Helen', 3.7),
(456, 'Tobius', 3.5),
(678, 'Amy', 3.9);

CREATE TABLE [SchoolDemo].School (
    [schoolID] int PRIMARY KEY NOT NULL,
    [schoolName] varchar(255) NOT NULL,
    [state] varchar(3) NOT NULL,
    [enrollment] int NULL
);

INSERT INTO [SchoolDemo].School (schoolID, schoolName, state, enrollment) VALUES
(1001, 'Sunshine Valley Academy', 'QLD', 3498),
(1002, 'Northern Ridge High School', 'NSW', 2587),
(1003, 'Hobart Collegiate Institute', 'TAS', 1034),
(1004, 'Melbourne Future School', 'VIC', 4500),
(1005, 'West Coast Education Centre', 'WA', 3975),
(1006, 'Southern Cross Grammar', 'SA', 2146),
(1007, 'Capital City Academy', 'ACT', 1289),
(1008, 'Outback Learning Hub', 'NT', 1720),
(1009, 'Riverdale Comprehensive School', 'QLD', 3458),
(1010, 'Eastern Heights College', 'NSW', 4671),
(1011, 'Tasman National School', 'TAS', 1893),
(1012, 'Victorian Tech School', 'VIC', 3039),
(1013, 'Perth Modern Institute', 'WA', 3555),
(1014, 'Adelaide Leadership Academy', 'SA', 2207),
(1015, 'Canberra Scholars College', 'ACT', 1422),
(1016, 'Darwin International School', 'NT', 1834),
(1017, 'Coastal View High School', 'QLD', 3127),
(1018, 'Sydney Progressive School', 'NSW', 4298),
(1019, 'Tasmania Science College', 'TAS', 2077),
(1020, 'Victoria Arts School', 'VIC', 4001),
(1021, 'Western Horizon University', 'WA', 4903),
(1022, 'South Australia Institute of Technology', 'SA', 2890),
(1023, 'Australian Capital Educational Centre', 'ACT', 1759),
(1024, 'Northern Territory Polytechnic', 'NT', 2615);

CREATE TABLE [SchoolDemo].[Application] (
    [applicationID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    [sID] int NOT NULL,
    [schoolID] int NOT NULL ,
    [major] VARCHAR(255) NULL,
    [decision] char(1) NOT NULL,
    FOREIGN KEY(sID) REFERENCES Student(sID),
    FOREIGN KEY(schoolID) REFERENCES School(schoolID)
);

INSERT INTO [SchoolDemo].Application (sId, SchoolId, major, decision)
VALUES (001, 1001, 'Computer Science', 'Y');
");
            }

        }

        public void Dispose()
        {
            _connection.Close();
        }

        //public string Name { get; } = "InMemorySchoolDemoDatabase";
        public IDbConnection GetConnection()
        {
            //TODO setup test data
            var sqliteConnection = new SqliteConnection($"Data Source=InMemorySchoolDemoDatabase;");

            if( _connection != null )
            {
                var testQuery = sqliteConnection.Query<Student>("SELECT * FROM [SchoolDemo].Student");
            }
            return sqliteConnection;
        }
    }
}
