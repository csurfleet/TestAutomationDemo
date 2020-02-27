using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestAutomationDemo.IntegrationTests.WithDeployedSut
{
    /// <summary>
    /// This shows the code required to get a simple integration test running against a deployed version of the website project.
    /// This will actually start up the TestAutomationDemo.Website, and if you pause the test while it is running you will be able to navigate to
    /// http://localhost:5001 and use the site.
    /// The benefit here is that we can use things like selenium to run our tests, the downside is that this makes things more brittle, and that it is significantly slower.
    /// </summary>
    public class WithDeployedSutTests : IDisposable
    {
        private Process _process;
        private IWebDriver _webDriver;

        public WithDeployedSutTests()
        {
            var projectName = "TestAutomationDemo.Website";
            var projectPath = GetProjectPath("TestAutomationDemo", projectName);

            _process = new Process
            {
                StartInfo = new ProcessStartInfo("dotnet.exe", $"run {projectPath}\\{projectName}.csproj --urls=http://localhost:5001")
                {
                    UseShellExecute = false,
                    WorkingDirectory = projectPath,
                }
            };
            var success = _process.Start();

            if (!success)
            {
                var errorMessage = _process.StandardOutput.ReadToEnd();
                throw new Exception(errorMessage);
            }
                
        }

        public void Dispose()
        {
            if (_process != null)
            {
                if (!_process.HasExited)
                    _process.Kill();
                _process.Dispose();
            }
        }

        [Fact]
        public async Task HomePage_SaysHello()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:5001");

            response.IsSuccessStatusCode.Should().BeTrue();

            // This is where things get ikky as we are dealing with the content directly, rather than though a browser
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("<h1 class=\"display-4\">Welcome</h1>");
        }

        private string GetProjectPath(string solutionFolderName, string projectName)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            var dir = new System.IO.DirectoryInfo(path);
            var counter = 0;

            while(dir.Name != solutionFolderName)
            {
                dir = dir.Parent;
                counter++;

                if (counter > 5)
                    throw new ArgumentException("Could not locate solution folder", nameof(solutionFolderName));
            }


            var target = dir.GetDirectories().FirstOrDefault(d => d.Name == projectName);
            if (target == null)
                throw new ArgumentException($"no project '{projectName}' found", nameof(projectName));

            return target.FullName;
        }
    }
}
