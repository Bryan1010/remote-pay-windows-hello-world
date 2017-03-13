using System;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using System.Threading;

namespace remote_pay_windows_hello_world
{
    class ProgramCopy
    {
        static void Main(string[] args)
        {
            int amountCharged = 0103;
            string invoiceNumber = "aa123f";
            ICloverConnector cloverConnector;
            Console.Write("initializing");
            CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration(null, "com.fromuthtennis.ustaproshop", false, 0000);

            cloverConnector = new CloverConnector(USBConfig);
            cloverConnector.AddCloverConnectorListener(new YourListener(cloverConnector));
            cloverConnector.InitializeConnection();


            Thread.Sleep(3000);
            if (cloverConnector.IsReady)
            {


            }

            else Console.Write("not connected");
            SaleResponse sresponse = new SaleResponse();

            //sresponse.Signature();
            VerifySignatureRequest t = new VerifySignatureRequest();



            AuthRequest arequest = new AuthRequest();





            Thread.Sleep(999999900);

            //Console.Write("finishing");
            cloverConnector.RemoveCloverConnectorListener(new YourListener(cloverConnector));
            //Environment.Exit(0);
        }

        class YourListener : DefaultCloverConnectorListener
        {
            ICloverConnector localcc;
            public YourListener(ICloverConnector cc) : base(cc)
            {
                localcc = cc;
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
                this.localcc.AcceptPayment(request.Payment);
            }

            public override void OnVerifySignatureRequest(VerifySignatureRequest request)
            {
                request.Accept();
            }



            public override void OnSaleResponse(SaleResponse response)
            {

                if (response.Success)
                {
                    localcc.ResetDevice();
                    Console.Write("fasndf\nadjfh\nasdfjkha\nasjldf\r\naskjdfh");
                }
                else
                {
                    localcc.ShowMessage("Order Canceled");
                    Thread.Sleep(3000);
                    localcc.ShowWelcomeScreen();

                }
            }

            // wait until this gets called to indicate the device
            // is ready to communicate before calling other methods
            public override void OnDeviceReady(MerchantInfo merchantInfo)
            {
                localcc.ResetDevice();
                int amountCharged = 503;
                string invoiceNumber = "aa1231";
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


                localcc.Sale(sarequest);
                VerifySignatureMessage a = new VerifySignatureMessage();
                VerifySignatureRequest vsrequest = new VerifySignatureRequest();
                localcc.AcceptSignature(vsrequest);
            }
        }
    }
}


