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
            public YourListener(ICloverConnector cc) : base(cc) 
            {
            }

            public override void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
            {
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


