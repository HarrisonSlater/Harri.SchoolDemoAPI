using Harri.SchoolDemoApi.Client;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using System.Runtime.CompilerServices;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentApiTests
    {
        private static HostedProvider _hostedProvider;
        private StudentApiClient _client;

        [OneTimeSetUp]
        public static async Task OneTimeSetup()
        {
            _hostedProvider = new HostedProvider();
            var t = new CancellationTokenSource();

            await _hostedProvider.StartAsync(t.Token);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            _hostedProvider.Dispose();
        }

        [SetUp]
        public async Task Setup()
        {
            _client = new StudentApiClient(_hostedProvider.ServerUri.AbsoluteUri);
        }

        // This test assumes a restored clean database
        [Test]
        public async Task GetStudent_ShouldReturnStudent()
        {
            var student = await _client.GetStudent(1);
            student.Should().NotBeNull();

            student.Name.Should().NotBeEmpty();
            student.GPA.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(4);
            student.SId.Should().Be(1);
        }

        [Test]
        public async Task AddStudent_ShouldAddNewStudent()
        {
            var newStudent = new NewStudentDto()
            {
                Name = "New Test Student",
                GPA = 3.81m
            };

            // Act 
            var id = await _client.AddStudent(newStudent);

            // Assert
            id.Should().NotBeNull();
            id.Should().BeGreaterThan(0);

            var student = await _client.GetStudent(id.Value);
            student.Should().NotBeNull();
            student.Name.Should().Be(newStudent.Name);
            student.GPA.Should().Be(newStudent.GPA);
        }
    }
}