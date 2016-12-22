using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TraffkPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            Services.CachingServices.Initialize(host.Services);
            Services.CachingServices.Instance.Register(RevolutionaryStuff.Azure.DocumentDb.DdbContext.Flusher, Services.CachingServices.FlushPeriods.Long);

            host.Run();
        }
    }
}
