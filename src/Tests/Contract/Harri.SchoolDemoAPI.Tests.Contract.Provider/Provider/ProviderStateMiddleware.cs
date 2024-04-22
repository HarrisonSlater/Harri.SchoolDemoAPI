using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    /// <summary>
    /// Middleware for handling provider state requests
    /// base file from: https://github.com/pact-foundation/pact-net/blob/master/samples/OrdersApi/Provider.Tests/ProviderStateMiddleware.cs
    /// </summary>
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> providerStates;
        private readonly RequestDelegate _next;
        //private readonly IStudentRepository _studentRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="ProviderStateMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="orders">Orders repository for actioning provider state requests</param>
        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            //_studentRepository = studentRepository;

            this.providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["a student with sId {sId} exists"] = this.EnsureStudentExists,
                ["a student with sId {sId} does not exist"] = this.EnsureStudentDoesNotExist,
                ["a student with sId {sIdNew} will be created"] = this.EnsureStudentWillBeCreated,
                ["a student with sId {sId} exists and will be updated"] = this.EnsureStudentWillBeUpdated,
                ["a student with sId {sId} will be updated"] = this.EnsureStudentWillBeUpdated,
                ["no student will be updated"] = this.EnsureNoStudentWillBeUpdated,
                ["no student will be deleted"] = this.EnsureNoStudentWillBeDeleted,
                ["a student with sId {sId} exists and will be deleted"] = this.EnsureStudentWillBeDeleted,
                ["a student with sId {sId} does not exist and will not be deleted"] = this.EnsureStudentDoesNotExistAndWillBeNotDeleted,
                ["a student with sId {sId} exists but can not be deleted"] = this.EnsureStudentHasConflictAndCanNotNotDeleted,
                //["a student with sId {sId} will be patched"] = this.EnsureStudentWillBePatched

            };
        }

        /// <summary>
        /// Ensure an event exists
        /// </summary>
        /// <param name="parameters">Event parameters</param>
        /// <returns>Awaitable</returns>
        private async Task EnsureStudentExists(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            var studentToMock = new Models.StudentDto()
            {
                SId = sId?.GetInt32(),
                Name = name?.GetString(),
                GPA = gpa?.GetDecimal()
            };

            TestStartup.MockStudentRepo.Setup<Task<Models.StudentDto>>(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult<Models.StudentDto>((Models.StudentDto?)studentToMock));
        }

        private async Task EnsureStudentDoesNotExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup<Task<Models.StudentDto>>(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult<Models.StudentDto>((Models.StudentDto?)null));
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<Models.StudentDto>())).Returns(Task.FromResult(false));
        }

        private async Task EnsureNoStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<Models.StudentDto>())).Throws(new Exception("UpdateStudent should not be called"));
        }

        private async Task EnsureNoStudentWillBeDeleted(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>())).Throws(new Exception("DeleteStudent should not be called"));
        }

        private async Task EnsureStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<Models.StudentDto>()))
                .Returns(Task.FromResult(true))
                .Callback<Models.StudentDto>(ns =>
                {
                    ns.SId.Should().Be(sId?.GetInt32());
                    ns.Name.Should().Be(name?.ToString());
                    ns.GPA.Should().Be(gpa?.GetDecimal());
                });
        }

        private async Task EnsureStudentWillBeCreated(IDictionary<string, object> parameters)
        {
            var sIdNew = (JsonElement?)parameters["sIdNew"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            TestStartup.MockStudentRepo.Setup(s => s.AddStudent(It.IsAny<NewStudentDto>()))
                .Returns(Task.FromResult(sIdNew.Value.GetInt32()))
                .Callback<NewStudentDto>(ns =>
                {
                    ns.Name.Should().Be(name.ToString());
                    ns.GPA.Should().Be(gpa?.GetDecimal());
                });

        }

        private async Task EnsureStudentWillBeDeleted(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)true))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });
        }

        private async Task EnsureStudentDoesNotExistAndWillBeNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)false))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });
        }

        private async Task EnsureStudentHasConflictAndCanNotNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)null))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });
        }


        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">Request context</param>
        /// <returns>Awaitable</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                try
                {
                    ProviderState providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);
                    
                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        await this.providerStates[providerState.State].Invoke(providerState.Params);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                    await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }
    }
}
