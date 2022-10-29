using System;
using System.IO;

namespace Bracabot2.Domain.Support
{
    public static class Config
    {
        public static void AddEnvironmentVariables()
        {
            foreach (var line in File.ReadAllLines("params.env"))
            {
                var parsed = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
                Environment.SetEnvironmentVariable(parsed[0], parsed[1]);
            }
        }
    }
}
