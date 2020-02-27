using System;
using System.Linq;

namespace TestAutomationDemo.IntegrationTests
{
    internal static class Helpers
    {
        internal static string GetProjectPath(string solutionFolderName, string projectName)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            var dir = new System.IO.DirectoryInfo(path);
            var counter = 0;

            while (dir.Name != solutionFolderName)
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
