using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace restservice
{
    [ServiceContract]
    public interface IService
    {
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "version")]
        [return: MessageParameter(Name = "data")]
        GETVERSION Version();

        [WebInvoke(Method = "POST",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "sale")]
        [return: MessageParameter(Name = "data")]
        SENDSALE sendSale(double ammount);
    }

    
    public class GETVERSION
    {
        public string version { get; set; }
    }

    public class SENDSALE
    {
        public double ammount { get; set; }
        public string transactionNumber { get; set; }
        public string status { get; set; }
    }
}
