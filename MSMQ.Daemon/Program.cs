//===============================================================================
// Microsoft FastTrack for Azure
// .Net Microsoft Message Queueing Sample
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using NetMSMQ.Repository;
using System;
using System.Threading.Tasks;

namespace MSMQ.Daemon
{
    class Program
    {
        static void Main(string[] args)
        {
            QueueProcessor queueProcessor = new QueueProcessor();
            // Uncomment the following line to load some sample messages into the local MSMQ queue
            //queueProcessor.CreateSampleData();

            Console.WriteLine("**** MSMQ Daemon is Starting ***");
            Task.Run(() => queueProcessor.ProcessMessages());
            Console.WriteLine("*** MSMQ Daemon is Running. Press any key to stop. ***");
            Console.Read();
            Console.WriteLine("*** MSMQ Daemon is Stopping ***");
            queueProcessor.Stop();
        }
    }
}
