using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FlightCalculator.Host
{
    public class Program
    {
        /// <summary>
        /// Start point of the host.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Define startup and build the host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
