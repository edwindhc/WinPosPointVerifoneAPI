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
             UriTemplate = "config")]
        [return: MessageParameter(Name = "data")]
        GETCONFIG GetConfig();

        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "cancelTransaction")]
        [return: MessageParameter(Name = "data")]
        CANCELTRANSACTION CancelTransaction();

        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "log")]
        [return: MessageParameter(Name = "data")]
        GETLOG GetLog();

        [WebInvoke(Method = "POST",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "sale")]
        [return: MessageParameter(Name = "data")]
        SENDSALE sendSale(double ammount);

        [WebInvoke(Method = "POST",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "config")]
        [return: MessageParameter(Name = "data")]
        CONFIG config(string merchantId, string ip, string terminalNumber);
    }

    
    public class GETCONFIG
    {
        public string merchantId { get; set; }
        public string ip { get; set; }
        public string terminalNumber { get; set; }

    }

    public class GETLOG
    {
        public string data { get; set; }

    }


    public class CANCELTRANSACTION
    {
        public bool status { get; set; }
    }

    public class SENDSALE
    {
        public double ammount { get; set; }
        public string transactionNumber { get; set; }
        public string status { get; set; }
    }

    public class CONFIG
    {
        public string merchantId { get; set; }
        public string ip { get; set; }
        public string terminalNumber { get; set; }
    }
}
