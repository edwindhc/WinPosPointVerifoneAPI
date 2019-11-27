using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WinPOSPointLib;

namespace restservice
{
    public class ServerWinPOS
    {
        public void Start()
        {
            WebServiceHost hostWeb = new WebServiceHost(typeof(restservice.Service));
            ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(restservice.IService), new WebHttpBinding(), "");
            ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            hostWeb.Open();
            WinPOSPoint wpp = new WinPOSPoint();

            Console.WriteLine("WinPOSPoint Version " + wpp.ProgramVersion + " started on Host http://localhost:8081 " + DateTime.Now.ToString());

            Console.Read();
        }

        public void Stop()
        {
            WebServiceHost hostWeb = new WebServiceHost(typeof(restservice.Service));
            ServiceEndpoint ep = hostWeb.AddServiceEndpoint(typeof(restservice.IService), new WebHttpBinding(), "");
            ServiceDebugBehavior stp = hostWeb.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            hostWeb.Open();
            WinPOSPoint wpp = new WinPOSPoint();

            Console.WriteLine("WinPOSPoint Version " + wpp.ProgramVersion + " started on Host http://localhost:8081 " + DateTime.Now.ToString());

            Console.Read();
        }
    }
}
