using System;
using System.Collections.Generic;
using System.ServiceModel; //interfaz
using WinPOSPointLib;
using System.IO;
using Newtonsoft.Json.Linq;

namespace restservice
{
    public partial class Service : IService
    {
        public WinPOSPoint posi = null;
        string filepath = @"C:\WinPosPoint\config.json";
        string logpath = @"C:\WinPosPoint\Log.json";
        string result = string.Empty;

        [return: MessageParameter(Name = "data")]
        public SENDSALE sendSale(double ammount)
        {
            isTransactionPending(true, false);
            IsAbortPending(true, false);
            var test = isTransactionPending(false, false);
            var test1 = IsAbortPending(false, false);
            try
            {
                
                var config = getConfigProperties();
                var jobj = JObject.Parse(config);
                var merchantId = jobj.SelectToken("MerchantId").ToString();
                var ip = jobj.SelectToken("IP").ToString();
                var terminalNumber = jobj.SelectToken("TerminalNumber").ToString();
                
                posi = new WinPOSPoint();
                SENDSALE data = new SENDSALE();

                data.ammount = ammount;
                posi.ClearData();
                posi.TransactionType = 0;
                posi.MerchantID = merchantId;
                posi.TerminalNumber = int.Parse(terminalNumber);
                posi.Amount = ammount * 100.0;
                posi.ClientApplicationName = "restservice";
                posi.ClientApplicationVersion = "3.0";
                posi.ServerHostName = ip;
                posi.ServerPortNumber = short.Parse("10676");
                posi.EcrCardTransactionReferenceNr = String.Format("{0:00}", DateTime.Now.Hour)
                                                           + String.Format("{0:00}", DateTime.Now.Minute)
                                                           + String.Format("{0:00}", DateTime.Now.Second);
                posi.ConfirmWaitTimeout = 5;
                posi.ProcessTransaction();
                while (true)
                {

                    if (!isTransactionPending(false, false)) { isTransactionPending(true, true); }
                    
                    if (IsAbortPending(false, false))
                    {
                        posi.PostCancelTransaction();
                        if (!IsAbortPending(false, false)) { IsAbortPending(true, false); }
                    }

                    if (posi.PollProcess() != -1)
                    {
                        isTransactionPending(true, false);
                        break;
                    }
                }

                if (posi.ResponseCode == "0")
                {
                    if (posi.MustConfirmTransaction != 0)
                    {
                        isTransactionPending(true, false);
                        IsAbortPending(true, false);
                        ConfirmTransaction();
                    }

                    data.transactionNumber = posi.TransactionNumber.ToString();
                    data.status = "success";
                    log("Operation status: " + data.status + " - Transaction Number: " + data.transactionNumber + " - Amount: " + data.ammount + " Timestamp " + DateTime.Now.ToString());
                    isTransactionPending(true, false);
                    IsAbortPending(true, false);
                    return data;
                }
                else
                {
                    isTransactionPending(true, false);
                    IsAbortPending(true, false);
                    data.status = "failed";
                    log("Operation status: " + data.status + " - Amount: " + data.ammount + " Timestamp " + DateTime.Now.ToString());
                    return data;
                }

            }
            catch (Exception ex)
            {
                isTransactionPending(true, false);
                IsAbortPending(true, false);
                posi.PostCancelTransaction();
                log(ex.ToString());
                return null;
            }
        }

        private string SetConfig (string value, string name){
            try
            {
                using (StreamReader r = new StreamReader(filepath))
                {
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json);
                    var IP = jobj.SelectToken("IP").ToString();
                    var MerchantId = jobj.SelectToken("MerchantId").ToString();
                    var TerminalNumber = jobj.SelectToken("TerminalNumber").ToString();
                    var isAbortPending = jobj.SelectToken("isAbortPending").Value<bool>();
                    var isTransactionPending = jobj.SelectToken("isTransactionPending").Value<bool>();

                    if(name == "MerchantId")
                    {
                        jobj["MerchantId"] = value;
                    }
                    else if(name == "IP")
                    {
                        jobj["IP"] = value;
                    }
                    else if(name == "TerminalNumber")
                    {
                        jobj["TerminalNumber"] = value;
                    }

                    result = jobj.ToString();
                    r.Close();
                    File.WriteAllText(filepath, result);
                    return result;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


        private string SetStates(bool value, string name)
        {
            try
            {
                using (StreamReader r = new StreamReader(filepath))
                {
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json);
                    var isAbortPending = jobj.SelectToken("isAbortPending").Value<bool>();
                    var isTransactionPending = jobj.SelectToken("isTransactionPending").Value<bool>();
                    if(name == "isAbortPending")
                    {
                        jobj["isAbortPending"] = value;
                    }
                    if(name == "isTransactionPending")
                    {
                        jobj["isTransactionPending"] = value;
                    }
                    result = jobj.ToString();
                    r.Close();
                    File.WriteAllText(filepath, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private string getConfigProperties()
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                var jobj = JObject.Parse(json);
                result = jobj.ToString();
                r.Close();
                return result;
            }
        }

        [return: MessageParameter(Name = "data")]
        public GETCONFIG GetConfig()
        {
            try
            {
                logJson("Success", 100, DateTime.Now.ToString());
                GETCONFIG data = new GETCONFIG();
                var config = getConfigProperties();
                var jobj = JObject.Parse(config);
                data.merchantId = jobj.SelectToken("MerchantId").ToString();
                data.ip = jobj.SelectToken("IP").ToString();
                data.terminalNumber = jobj.SelectToken("TerminalNumber").ToString();
                return data;
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
        }
        

        private bool IsAbortPending(bool setter, bool status)
        {
            if (!setter)
            {
                var config = getConfigProperties();
                var jobj = JObject.Parse(config);
                return jobj.SelectToken("isAbortPending").Value<bool>();
            }

            var res = SetStates(status, "isAbortPending");
            return true;
        }

        private bool isTransactionPending(bool setter, bool status)
        {
            if (!setter)
            {
                var config = getConfigProperties();
                var jobj = JObject.Parse(config);
                return jobj.SelectToken("isTransactionPending").Value<bool>();
            }
            var res = SetStates(status, "isTransactionPending");
            return true;
        }

        public CANCELTRANSACTION CancelTransaction()
        {
            try
            {
                CANCELTRANSACTION data = new CANCELTRANSACTION();
                if (isTransactionPending(false,false))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        r.Close();
                    }
                    IsAbortPending(true,true);
                    isTransactionPending(true, true);
                    data.status = true;
                } else
                {
                    data.status = false;
                }
                
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                log(ex.ToString());
                return null;
            }
        }

        [return: MessageParameter(Name = "data")]
        public CONFIG config(string merchantId, string ip, string terminalNumber)
        {
            try
            {
                var config = "";
                CONFIG data = new CONFIG();
                if (!string.IsNullOrWhiteSpace(merchantId))
                {
                    config = SetConfig(merchantId, "MerchantId");
                }
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    config = SetConfig(ip, "IP");
                }
                if (!string.IsNullOrWhiteSpace(terminalNumber))
                {
                    config = SetConfig(terminalNumber, "TerminalNumber");
                }
                var jobj = JObject.Parse(config);
                data.merchantId = jobj.SelectToken("MerchantId").ToString();
                data.ip = jobj.SelectToken("IP").ToString();
                data.terminalNumber = jobj.SelectToken("TerminalNumber").ToString();
                return data;
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
        }

        [return: MessageParameter(Name = "data")]
        public GETLOG GetLog()
        {
            try
            {
                GETLOG data = new GETLOG();
                using (StreamReader r = new StreamReader(logpath))
                {
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json);
                    JArray items = (JArray)jobj["data"];
                    Console.WriteLine(items.ToString());
                    data.data = jobj.ToString();
                    r.Close();
                }
                return data;
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
        }

        private void ConfirmTransaction()
        {
            if (posi.MustConfirmTransaction != 0)
            {
                posi.TransactionType = 901;
                posi.ProcessTransaction();
            }
        }

        private void log(string info)
        {
            string[] lines = new string[] { info };
            File.AppendAllLines(@"C:\WinPosPoint\Log.txt", lines);
        }

        private void logJson(string status, double amount, string timestamp)
        {
            try
            {
                using (StreamReader r = new StreamReader(logpath))
                {
                    JObject create = new JObject();
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json);
                    JArray items = (JArray)jobj["data"];
                    int id = items.Count + 1;
                    create["id"] = id;
                    create["status"] = status;
                    create["amount"] = amount;
                    create["timestamp"] = timestamp;
                    items.Add(create);
                    var result = jobj.ToString();
                    r.Close();
                    File.WriteAllText(logpath, result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SendAbortIfPending()
        {
            if (IsAbortPending(true, false))
            {
                posi.PostCancelTransaction();
                isTransactionPending(true, false);
            }
        }

    }
}
