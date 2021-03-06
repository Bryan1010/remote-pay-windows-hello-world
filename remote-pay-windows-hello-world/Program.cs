﻿using System;
using System.IO;
using System.Text;
using com.clover.remote.order;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using System.Threading;

namespace remote_pay_windows_hello_world
{
    public class Program
    {

        public static bool isBusy = false;
        

        static void Main(string[] args)
        {
            string startFilePath = "c:/clover/SaleRequest.txt";

            //testFile
            //string startFilePath = "c:/users/bryanc/Desktop/test.txt";


            ICloverConnector cloverConnector;
            CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", "com.Fromuth.BC.tech", false, 1);
            cloverConnector = new CloverConnector(USBConfig);
            cloverConnector.AddCloverConnectorListener(new YourListener(cloverConnector));
            cloverConnector.InitializeConnection();

            Logging log = new Logging("CloverLog");
            log.WriteMessage("Clover Program Started");

            DateTime timeStamp = DateTime.Now;

            Thread.Sleep(5000);
            do
            {
                if (cloverConnector.IsReady && !isBusy)
                {

                    try
                    {
                        //output a clover isConnected file
                        if (File.Exists("c:/clover/isConnected.txt"))
                        {
                            File.WriteAllText("c:/clover/isConnected.txt", DateTime.Now.ToString());
                        }
                        else
                        {
                            File.Create("c:/clover/isConnected.txt");
                            File.WriteAllText("c:/clover/isConnected.txt", DateTime.Now.ToString());
                        }


                        //If the SaleRequest is made, proceed, else keep repeatig loop.
                        if (File.Exists(startFilePath) && !isBusy)
                        {
                            isBusy = true;

                            string startFileText = File.ReadAllText(startFilePath);
                            //File.Delete(startFilePath);
                            //File.Move(startFilePath, "c:/clover/clover-request-sale-file.txt");
                            string[] startFileContent = startFileText.Split('\t');
                            switch (startFileContent[0].ToLower())
                            {
                                case "SALE":
                                case "Sale":
                                case "sale":
                                    {
                                        //StartOrder(cloverConnector);
                                        StartSale(cloverConnector, startFileContent[1], Int32.Parse(startFileContent[2]));
                                        break;
                                    }
                                case "mrefund":
                                case "MREFUND":
                                case "MRefund":
                                case "refund":
                                    {
                                        StartRefund(cloverConnector, startFileContent[1], Int32.Parse(startFileContent[2]));
                                        break;
                                    }
                                case "fdrefund":
                                case "FDREFUND":
                                case "FDRefund":
                                    {
                                        StartDirectRefund(cloverConnector, startFileContent[1], startFileContent[2]);
                                        break;
                                    }
                                case "pdrefund":
                                case "PDREFUND":
                                case "PDRefund":
                                    {
                                        StartDirectRefund(cloverConnector, startFileContent[1], startFileContent[2], Int32.Parse(startFileContent[3]));
                                        break;
                                    }
                                case "cancel":
                                case "CANCEL":
                                case "Cancel":
                                    {
                                        cloverConnector.ShowMessage("Transaction Canceled by the cashier.");
                                        Thread.Sleep(1500);
                                        cloverConnector.ShowWelcomeScreen();
                                        break;
                                    }
                                default:
                                    {
                                        cloverConnector.ShowMessage("Invalid Response");
                                        Thread.Sleep(3000);
                                        cloverConnector.ShowWelcomeScreen();
                                        break;
                                    }

                            }

                            //File.Delete(startFilePath);


                        }
                        Thread.Sleep(5000);
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        log.WriteError(ex.Message);
                        isBusy = false;
                        
                        Thread.Sleep(5000);

                    }
                }
            } while (true);
        }

        public static void WriteMessage(TextWriter log_writer,string FileName,string message)
        {
            
            log_writer.WriteLine("# " + DateTime.Now.ToLongTimeString() + " " + message);
            log_writer.WriteLine();
            log_writer.Close();
        }

        public static void StartSale(ICloverConnector cloverConnector, string invNum, int amt)
        {
            
            //cloverConnector.ResetDevice();
            string invoiceNumber = invNum;
            SaleRequest sarequest = new SaleRequest();
            sarequest.Amount = amt;
            sarequest.ExternalId = invoiceNumber;

            sarequest.CardEntryMethods = CloverConnector.CARD_ENTRY_METHOD_MANUAL;
            sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_MAG_STRIPE;
            sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_ICC_CONTACT;
            sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_NFC_CONTACTLESS;

            sarequest.DisablePrinting = true;

            sarequest.ApproveOfflinePaymentWithoutPrompt = true;
            sarequest.AllowOfflinePayment = true;
            
             cloverConnector.Sale(sarequest);
            
        }

        public static void StartRefund(ICloverConnector cloverConnector, string invNum, int amt)
        {
            //cloverConnector.ResetDevice();
            ManualRefundRequest request = new ManualRefundRequest();
            request.ExternalId = invNum;
            request.Amount = amt;

            // Card Entry methods
            long CardEntry = 0;
            CardEntry |= CloverConnector.CARD_ENTRY_METHOD_MANUAL;
            CardEntry |= CloverConnector.CARD_ENTRY_METHOD_MAG_STRIPE;
            CardEntry |= CloverConnector.CARD_ENTRY_METHOD_ICC_CONTACT;
            CardEntry |= CloverConnector.CARD_ENTRY_METHOD_NFC_CONTACTLESS;

            request.CardEntryMethods = CardEntry;
            request.DisablePrinting = true;
            request.DisableRestartTransactionOnFail = true;

            cloverConnector.ManualRefund(request);


        }

        public static void StartDirectRefund(ICloverConnector cloverConnector, string paymentId, string orderId, int amt)
        {
            //cloverConnector.ResetDevice();
            RefundPaymentRequest refundRequest = new RefundPaymentRequest();
            refundRequest.PaymentId = paymentId;
            refundRequest.OrderId = orderId;
            refundRequest.Amount = amt;


            cloverConnector.RefundPayment(refundRequest);
        }


        public static void StartDirectRefund(ICloverConnector cloverConnector, string paymentId, string orderId)
        {
            //cloverConnector.ResetDevice();
            RefundPaymentRequest refundRequest = new RefundPaymentRequest();
            refundRequest.PaymentId = paymentId;
            refundRequest.OrderId = orderId;
            refundRequest.FullRefund = true;


            cloverConnector.RefundPayment(refundRequest);
        }

        //Experimenting on displaying orders
        //NOT READY YET
        public static void StartOrder(ICloverConnector cloverConnector)
        {
            cloverConnector.ResetDevice();
            DisplayOrder o = new DisplayOrder();
            DisplayLineItem l = new DisplayLineItem();
            DisplayPayment p = new DisplayPayment();
            l.name = "hello";
            l.price = "1234";
            o.addDisplayLineItem(l);

            o.subtotal = l.price;
            //o.tax = "0";
            o.total = o.subtotal + o.tax;
            o.currency = "USD";
            o.title = "My Order";

            p.amount = o.total;
            cloverConnector.ShowDisplayOrder(o);
        }



        class YourListener : DefaultCloverConnectorListener
        {
            public string output = "";
            string startFilePath = "c:/clover/SaleRequest.txt";
            public const string SaleFilePath = "c:/clover/sale.txt";
            public const string signatureApproveRequestFilePath = "c:/clover/SignatureApproveRequest.txt";
            public bool hasSignature = false;
            public bool deviceOffline = false;
            ICloverConnector cloverConnector;
            public YourListener(ICloverConnector cc) : base(cc)
            {
                cloverConnector = cc;
            }

            public override void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
            {
                for (int i = 0; i < request.Challenges.Count; i++)
                {
                    if (request.Challenges[i].type == ChallengeType.DUPLICATE_CHALLENGE)
                    {
                        
                    }
                    
                    else if (request.Challenges[i].type == ChallengeType.OFFLINE_CHALLENGE)
                    {
                        
                    }
                    //if (request.Payment.offline)
                    //{
                    //    deviceOffline = true;
                    //    cloverConnector.ShowMessage("TRANSACTION ERROR:\nDEVICE OFFLINE");
                    //    File.WriteAllText(SaleFilePath, "FAILED\tOFFLINE");
                    //    this.cloverConnector.RejectPayment(request.Payment, request.Challenges[i]);
                    //}
                }
                
                this.cloverConnector.AcceptPayment(request.Payment);               
            }


            // outputs signature coordinates to a file
            public override void OnVerifySignatureRequest(VerifySignatureRequest request)
            {
                output = "";
                foreach (Signature2.Stroke stroke in request.Signature.strokes)
                {
                    hasSignature = true;
                    output += "[";
                    for (int i = 0; i < stroke.points.Count; i++){
                        output += "[" + stroke.points[i].x + "," + stroke.points[i].y + "]";
                    }
                    output += "]";
                }
                File.WriteAllText(signatureApproveRequestFilePath, output);
                request.Accept();

            }

            //Accepting the signature from the client
            public  void verifySignature(VerifySignatureRequest request, string path)
            {
                request.Accept();
                //validating the file for signature validation
                bool flag = false;
                //while (flag)
                //{

                //    if (File.Exists(path))
                //    {
                //        string[] signatureResponse = File.ReadAllText(path).Split('\t');
                //        switch (signatureResponse[0])
                //        {
                //            case "Accept":
                //            case "accept":
                //            case "ACCEPT":
                //                {
                //                    request.Accept();
                //                    flag = false;
                //                    File.Delete(path);
                //                    break;
                //                }
                //            case "Reject":
                //            case "reject":
                //            case "REJECT":
                //                {
                //                    request.Reject();
                //                    File.Delete(path);
                //                    flag = false;
                //                    break;
                //                }
                //            default:
                //                {
                //                    cloverConnector.ShowMessage("Invalid Response");
                //                    Console.WriteLine("Invalid Response");
                //                    flag = false;
                //                    break;
                //                }
                //        }
                //    }
                //}
            }




            public override void OnSaleResponse(SaleResponse response)
            {
                string cloverMessage = "";
                output = "";
                
                if (response.Success)
                {
                    // payment was successful
                    // do something with response.Payment
                    output = "APPROVED\t" + response.Payment.amount + "\t" + response.Payment.id + "\t" + response.Payment.order.id + "\t";
                    cloverMessage = "TRANSACTION APPROVED";
                }
                else
                {
                    output = "FAILED\t" + response.Result + "\t" + response.Message;
                    cloverMessage = "TRANSACTION " + response.Result;
                }
                if (deviceOffline)
                {
                    output = "FAILED\tOFFLINE\t";
                    cloverMessage = "TRANSACTION FAILED:\nDEVICE OFFLINE";
                }



                    if (!String.IsNullOrEmpty(output))
                    {
                        if (!hasSignature)
                            output += "NONE\t";

                        File.WriteAllText(SaleFilePath, output);

                        File.Delete(startFilePath);
                    }
                    cloverConnector.ShowMessage(cloverMessage);
                    Thread.Sleep(3000);
                    cloverConnector.ShowWelcomeScreen();
                    hasSignature = false;
                    isBusy = false;
                
            }

            
            public override void OnRefundPaymentResponse(RefundPaymentResponse response)
            {
                //For some reason Boolean values of success and result and responses are flipped true and false
                output = "";
                if (response.Success)
                {

                    output = "APPROVED\t" + response.Result + "\t" + response.Success + "\t" + 
                        "\t" + response.Refund.amount.ToString() + "\t" + response.PaymentId + "\t" + response.OrderId + "\t";
                }
                else
                {
                    output = "FAILED\t" + response.Result + "\t" + response.Reason + "\t" + response.Success 
                        + "\t" + response.Message + "\t";

                }
                

                if (!hasSignature)
                    output += "NONE\t";

                File.WriteAllText(SaleFilePath, output);
                cloverConnector.ShowMessage("REFUND " + response.Result);
                File.Delete(startFilePath);
                Thread.Sleep(2000);
                hasSignature = false;
                isBusy = false;
                cloverConnector.ShowWelcomeScreen();
            }
            


            public override void OnManualRefundResponse(ManualRefundResponse response)
            {
                
                output = "";
                if (response.Success)
                {
                    output = "APPROVED\t" + response.Credit.amount +
                        "\t" + response.Credit.cardTransaction.last4 + "\t";
                }
                else
                {
                    output = "FAILED\t" + response.Result + "\t";

                }
                if (!hasSignature)
                    output += "NONE\t";

                File.WriteAllText(SaleFilePath, output);
                cloverConnector.ShowMessage("TRANSACTION " + response.Result);
                File.Delete(startFilePath);
                Thread.Sleep(3000);
                hasSignature = false;
                isBusy = false;
                cloverConnector.ShowWelcomeScreen();
            }


            // wait until this gets called to indicate the device
            // is ready to communicate before calling other methods
            public override void OnDeviceReady(MerchantInfo merchantInfo)
            {
                cloverConnector.ShowWelcomeScreen();
            }

            public override void OnDeviceDisconnected()
            {
                File.WriteAllText("c:/clover/notConnected.txt", DateTime.Now.ToString());
            }
        }
    }

    public class Logging
    {
        TextWriter log_writer;
        string Filename = "";

        public Logging(string logname)
        {
            DateTime d = DateTime.Now;
            Filename = "C:/clover/logs/" + logname + "_" + d.ToString("yyyy") + "_" + d.ToString("MM") + "_" + d.ToString("dd") +
                "_" + d.ToString("hh") + "_" + d.ToString("mm") + "_" + d.ToString("ss") + ".txt";

            if (!File.Exists("C:/clover/logs/"))
            {
                Directory.CreateDirectory("C:/clover/logs/");
            }
        }

        public void WriteMessage(string message)
        {
            log_writer = new StreamWriter(Filename, true);
            log_writer.WriteLine("# " + DateTime.Now.ToLongTimeString() + " " + message);
            log_writer.WriteLine();
            log_writer.Close();
        }

        public void WriteError(string message)
        {
            log_writer = new StreamWriter(Filename, true);
            log_writer.WriteLine("##############");
            log_writer.WriteLine("# ERROR " + DateTime.Now.ToLongTimeString() + " " + message);
            log_writer.WriteLine("##############");
            log_writer.WriteLine();
            log_writer.Close();
        }

    }
}
