using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel; //interfaz
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WinPOSPointLib;
using Topshelf;
using System.IO;

namespace restservice
{
    public class Service : IService
    {
        private WinPOSPoint posi = null;

        [return: MessageParameter(Name = "data")]
        public SENDSALE sendSale(double ammount)
        {
            try { 
                posi = new WinPOSPoint();
                SENDSALE data = new SENDSALE();

                data.ammount = ammount;
                posi.ClearData();
                posi.TransactionType = 0;
                posi.MerchantID = "CBE01";
                posi.TerminalNumber = int.Parse("1");
                posi.Amount = ammount * 100.0;
                posi.ClientApplicationName = "restservice";
                posi.ClientApplicationVersion = "3.0";
                posi.ServerHostName = "192.168.0.20";
                posi.ServerPortNumber = short.Parse("10676");
                posi.EcrCardTransactionReferenceNr = String.Format("{0:00}", DateTime.Now.Hour)
                                                           + String.Format("{0:00}", DateTime.Now.Minute)
                                                           + String.Format("{0:00}", DateTime.Now.Second);

                posi.ProcessTransaction();
                while (true)
                {
                    if (posi.PollProcess() != -1)
                    {
                        break;
                    }
                }

                if (posi.ResponseCode == "0")
                {
                    if (posi.MustConfirmTransaction != 0)
                    {
                        ConfirmTransaction();
                    }

                    data.transactionNumber = posi.TransactionNumber.ToString();
                    data.status = "success";
                    log("Operation status: " + data.status + " - Transaction Number: " + data.transactionNumber + " - Amount: " + data.ammount + " Timestamp " + DateTime.Now.ToString());
                    return data;
                }
                else
                {
                    data.status = "failed";
                    log("Operation status: " + data.status + " - Amount: " + data.ammount + " Timestamp " + DateTime.Now.ToString());
                    return data;
                }

            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
        }

        [return: MessageParameter(Name = "data")]
        public GETVERSION Version()
        {
            try
            {
                GETVERSION data = new GETVERSION();
                data.version = posi.ProgramVersion;
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

    }
}
