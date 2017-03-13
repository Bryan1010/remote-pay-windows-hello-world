using System;
using System.IO;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using System.Threading;

namespace remote_pay_windows_hello_world
{
    class Program
    {
        static void Main(string[] args)
        {
            ICloverConnector cloverConnector;
            //Console.Write("initializing");
            CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", "thebrewery.tech.test", false, 1);

            cloverConnector = new CloverConnector(USBConfig);
            cloverConnector.AddCloverConnectorListener(new YourListener(cloverConnector));
            cloverConnector.InitializeConnection();
            while (true)
            {

                Thread.Sleep(799999000);

            }

            //Thread.Sleep(7000);

            //cloverConnector.ShowMessage("hello world");


            // close connection to clover
            //cloverConnector.Dispose();
            //Console.Write("finishing");
        }



        class YourListener : DefaultCloverConnectorListener
        {
            public String data;
            //public StreamWriter reader = new StreamWriter("c:/users/bryanc/Desktop/cloverData.txt");
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
                        // handle the challenge, "This might be a duplicate payment, continue?"
                    }

                    if (request.Challenges[i].type == ChallengeType.OFFLINE_CHALLENGE)
                    {
                        // handle an offline payment request challenge
                    }
                }

                
                // for the sake of a hello world demo, just accept all payments
                this.cloverConnector.AcceptPayment(request.Payment);
                //reader.Close();
            }

            public override void OnVerifySignatureRequest(VerifySignatureRequest request)
            {
                string output = "";
                File.WriteAllText("c:/users/bryanc/Desktop/cloverData.txt", "signature: " + request.Signature.strokes.ToString());
                foreach(Signature2.Stroke stroke in request.Signature.strokes)
                {
                    if (stroke.points.Count == 1)
                    {
                        Signature2.Point dot = stroke.points[0];
                        output += "[[" + dot.x.ToString() + ',' + dot.y.ToString() + "]";
                    }
                    else if (stroke.points.Count > 1)
                    {
                        output = "[";
                        for (int i = 0; i < stroke.points.Count; i++)
                        {
                            Signature2.Point dot = stroke.points[i];

                            output += "[" + dot.x.ToString() + "," + dot.y.ToString() + "]";

                            if(i == stroke.points.Count - 1)
                            {
                                output += "],";
                            }
                            
                        }
                    }

                    output += "],aaaa\n";

                }
                    File.WriteAllText("c:/users/bryanc/Desktop/cloverData.txt", "\n\n\n\n" +output+ "\n\n\n\n" + request.Signature.width + "\n\n\n\n" + "\n\n\n\n" + request.Payment.amount + "\n\n\n\n");
                Console.Write(request.Signature.ToString());
                request.Accept();

                Console.Write("\n\n\n\n" + "signature: " + request.Signature.height + "\n\n\n\n\n");
            }

            public override void OnSaleResponse(SaleResponse response)
            {
                    Console.Write(response.Payment.ToString());
                if (response.Success)
                {
                    // payment was successful
                    // do something with response.Payment
                    //Console.Write();
                    data += response.Payment.amount.ToString();
                    File.WriteAllText("c:/users/bryanc/Desktop/cloverData.txt", "Payment of: "+ response.Payment.amount);
                    
                }
                else 
                {
                    
                    cloverConnector.ShowMessage("Transaction Failed.");
                    File.WriteAllText("c:/users/bryanc/Desktop/cloverData.txt", "TRANNSACTION FAILED: " + response.Result + "\nDetails: " + response.Reason);
                    Console.WriteLine("\n\n\n" + data + "\n\n\n");
                
                    // payment didn't complete, can look at response.Result, response.Reason for additional info
                }
            }


            // wait until this gets called to indicate the device
            // is ready to communicate before calling other methods
            public override void OnDeviceReady(MerchantInfo merchantInfo)
            {
                cloverConnector.ResetDevice();
                int amountCharged = 03;
                Random rand = new Random();
                string invoiceNumber = rand.Next(1000, 5000).ToString();
                SaleRequest sarequest = new SaleRequest();
                sarequest.Amount = amountCharged;
                sarequest.ExternalId = invoiceNumber; //KAD46SD3SFR7P

                sarequest.CardEntryMethods = CloverConnector.CARD_ENTRY_METHOD_MANUAL;
                sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_MAG_STRIPE;
                sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_ICC_CONTACT;
                sarequest.CardEntryMethods |= CloverConnector.CARD_ENTRY_METHOD_NFC_CONTACTLESS;

                sarequest.DisablePrinting = true;

                sarequest.ApproveOfflinePaymentWithoutPrompt = true;
                sarequest.AllowOfflinePayment = true;


                cloverConnector.Sale(sarequest);

            }
        }
    }
}