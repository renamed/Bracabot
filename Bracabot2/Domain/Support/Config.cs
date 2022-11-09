using System;
using System.IO;
using System.Linq;

namespace Bracabot2.Domain.Support
{
    public static class Config
    {
        public static void AddEnvironmentVariables()
        {
            SetFileToEnv("params.env");
        }

        private static void SetFileToEnv(string path)
        {
            foreach (var line in File.ReadAllLines(path))
            {
                var parsed = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
                var key = parsed[0];
                var value = string.Join("=", parsed.Skip(1));
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
