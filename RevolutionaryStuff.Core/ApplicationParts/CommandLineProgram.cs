using System;
using System.Diagnostics;
using RevolutionaryStuff.Core.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;

namespace RevolutionaryStuff.Core.ApplicationParts
{
    public abstract class CommandLineProgram : BaseDisposable
    {
        #region Command Line Args

        protected CommandLineInfo Cli { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        #endregion

        private void Go() => OnGoAsync().ExecuteSynchronously();

        protected abstract Task OnGoAsync();

        protected virtual void OnBuildConfiguration(IConfigurationBuilder builder)
        { }

        private void BuildConfiguration()
        {
            var appEnvironment = PlatformServices.Default.Application;

            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnvironment.ApplicationBasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            OnBuildConfiguration(builder);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        protected virtual void OnPostProcessCommandLineArgs()
        { }

        private void ProcessCommandLineArgs(string[] args)
        {
            var cli = new CommandLineInfo(Configuration, args);
            Cli = cli;
            CommandLineSwitchAttribute.SetArgs(Cli, this);
            OnPostProcessCommandLineArgs();
        }

        protected virtual void OnBuildServices(IServiceCollection services)
        { }

        private void BuildServices()
        {
            var services = new ServiceCollection();
            services.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
            services.AddOptions();
            OnBuildServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        protected CommandLineProgram()
        { }

        public static void PrintUsage(Type t)
        {
            var usage = CommandLineInfo.GetUsage(t);
            Trace.WriteLine(usage);
        }

        public static void Main(Type t, string appConfigFilename, string[] args)
        {
            Requires.NonNull(t, nameof(t));
            Requires.IsType(t, typeof(CommandLineProgram));
            var configuration = ConfigurationHelpers.CreateConfigurationFromFilename(appConfigFilename);

            RevolutionaryStuffCoreOptions.Initialize(configuration);

//            Trace.Listeners.Add(new ConsoleTraceListener(true));
            using (var tracing = new Tracer())
            {
                CommandLineProgram p = null;
                bool programInOperation = false;
                try
                {
                    var ci = t.GetTypeInfo().GetConstructor(Empty.TypeArray);
                    p = (CommandLineProgram)ci.Invoke(Empty.ObjectArray);
                    programInOperation = true;
                    p.BuildConfiguration();
                    p.ProcessCommandLineArgs(args);
                    p.BuildServices();
                    p.Go();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is CommmandLineInfoException)
                    {
                        Trace.WriteLine(ex.InnerException.Message);
                    }
                    else if (ex is CommmandLineInfoException)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                    else
                    {
                        Trace.WriteLine(ex);
                    }
                    if (!programInOperation)
                    {
                        PrintUsage(t);
                    }
                }
                finally
                {
                    Stuff.Dispose(p);
                }
            }
        }
    }
}
