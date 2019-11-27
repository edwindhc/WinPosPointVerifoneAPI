using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Topshelf;
using WinPOSPointLib;
using System.Net.Http;

namespace restservice
{
    class Program
    {
        static void Main(string[] args)
        {
            /*WebServiceHost hostWeb = new WebServiceHost(typeof(restservice.Service));
            ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(restservice.IService), new WebHttpBinding(), "");
            ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            hostWeb.Open();
            WinPOSPoint wpp = new WinPOSPoint();

            Console.WriteLine("WinPOSPoint Version " + wpp.ProgramVersion + " started on Host http://localhost:8081 " + DateTime.Now.ToString());

            Console.Read();*/
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ServerWinPOS>(s =>
                {
                    s.ConstructUsing(server => new ServerWinPOS());
                    s.WhenStarted(server => server.Start());
                    s.WhenStopped(server => server.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("WinPosPointService");
                x.SetDisplayName("WinPosPoint API");
                x.SetDescription("This is an api application for verifone pos point");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
