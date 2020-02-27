using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace TestAutomationDemo.IntegrationTests.WithDeployedSutAndSelenium
{
    public class WithDeployedSutAndSelenium : IDisposable
    {
        private Process _process;
        private IWebDriver _webDriver;

        public WithDeployedSutAndSelenium()
        {
            var projectName = "TestAutomationDemo.Website";
            var projectPath = Helpers.GetProjectPath("TestAutomationDemo", projectName);

            _process = new Process
            {
                StartInfo = new ProcessStartInfo("dotnet.exe", $"run {projectPath}\\{projectName}.csproj --urls=http://localhost:5002")
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

            _webDriver = new ChromeDriver();
        }

        public void Dispose()
        {
            if (_webDriver != null)
            {
                _webDriver.Close();
                _webDriver.Dispose();
            }

            if (_process != null)
            {
                if (!_process.HasExited)
                    _process.Kill();
                _process.Dispose();
            }
        }

        [Fact]
        public void HomePage_SaysHello()
        {
            _webDriver.Url = "http://localhost:5002";

            var items = _webDriver.FindElements(By.XPath("//h1[@class=\"display-4\"]"));

            items.Should().Contain(i => i.Text == "Welcome");
        }
    }
}
