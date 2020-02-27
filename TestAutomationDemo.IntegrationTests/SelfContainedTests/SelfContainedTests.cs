using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestAutomationDemo.IntegrationTests.SelfContainedTests
{
    /// <summary>
    /// This test class shows the minimum required code to get an integration test running against the Website project.
    /// The tests here use the TestServer imlementation, which allows us to paralellise massively without cross-effects, and to speed up individual test execution hugely.
    /// HOWEVER, we cannot use Selenium for these tests, meaning they need more skills from the developer. They should be used for tests which run often, and which do not
    /// rely on lots of interaction with the UI.
    /// </summary>
    public class SelfContainedTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        // Runs once before any tests in the file
        public SelfContainedTests()
        {
            // This is the minimum we need to get our test site up and running, with all its default settings etc.
            // We have the power to inject alternative config files here, or even to inject our own versions of code into the IoC system
            // to override any functionality in the system (great for injecting mocked external services, faking dates/times etc)
            _server = new TestServer(new WebHostBuilder().UseStartup<Website.Startup>());
            _client = _server.CreateClient();
        }

        // Runs once after the tests all run
        public void Dispose()
        {
            if (_server != null)
                _server.Dispose();
        }

        [Fact]
        public async Task HomePage_SaysHello()
        {
            var response = await _client.GetAsync("");

            response.IsSuccessStatusCode.Should().BeTrue();

            // This is where things get ikky as we are dealing with the content directly, rather than though a browser
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("<h1 class=\"display - 4\">Welcome</h1>");

            // We can't connect selenium to this as we have the single HttpClient rather than the headless browser
            // Instead I'd use one of the ten bazillion HTML parsing tools to create a page model allowing the assertion to be more like:
            // var page = await response.Content.ToPage();
            // page.MainContent.Should()
                // .Contain()
                // .H1().WithText("Welcome");
        }


    }
}
