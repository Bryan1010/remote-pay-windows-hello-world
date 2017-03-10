using System;
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
            Console.Write("initializing");
            CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", "thebrewery.tech.test", false, 1);

            cloverConnector = new CloverConnector(USBConfig);
            cloverConnector.AddCloverConnectorListener(new YourListener(cloverConnector));
            cloverConnector.InitializeConnection();

            Thread.Sleep(7000);

            cloverConnector.ShowMessage("hello world");

            Thread.Sleep(7000);

            Console.Write("finishing");
        }

        class YourListener : DefaultCloverConnectorListener
        {
            ICloverConnector cloverConnector;
            public YourListener(ICloverConnector cc) : base(cc) 
            {
                cloverConnector = cc;
            }

            public override void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
            {
                // there are only two possible challenges - DUPLICATE_CHALLENGE and OFFLINE_CHALLENGE 
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
            }

            public override void OnSaleResponse(SaleResponse response)
            {
                if (response.Success)
                {
                    // payment was successful
                    // do something with response.Payment
                }
                else
                {
                    // payment didn't complete, can look at response.Result, response.Reason for additional info
                }
            }

            // wait until this gets called to indicate the device
            // is ready to communicate before calling other methods
            public override void OnDeviceReady(MerchantInfo merchantInfo)
            {
            }
        }
    }
}


