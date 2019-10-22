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
using Microsoft.Azure.ServiceBus;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NetMSMQ.Repository
{
    public class QueueProcessor
    {
        private static TopicClient _topicClient = null;
        private readonly string _queueName = ConfigurationManager.AppSettings["QueueName"];
        private readonly string _topicName = ConfigurationManager.AppSettings["TopicName"];
        private bool _isStopped = false;
        private bool _isRunning = false;

        public QueueProcessor()
        {
            // For performance reasons we are using a static instance of the TopicClient per best practices
            if (_topicClient == null)
            {
                string connectionString = ConfigurationManager.AppSettings["ServiceBus"];
                _topicClient = new TopicClient(connectionString, _topicName);
            }
            Trace.TraceInformation("QueueProcessor: Initialized.");
        }

        public async void ProcessMessages()
        {
            if (MessageQueue.Exists(_queueName))
            {
                // Use the ActiveXMessageFormatter for string data - use the XmlMessageFormatter for objects
                MessageQueue messageQueue = new MessageQueue(_queueName);
                messageQueue.Formatter = new ActiveXMessageFormatter();
                _isStopped = false;
                _isRunning = true;
                do
                {
                    if (_isStopped) break;

                    try
                    {
                        Trace.TraceInformation($"QueueProcessor: Checking queue {_queueName} for messages...");
                        System.Messaging.Message message = await Task.Run(() => messageQueue.Receive(new TimeSpan(0, 0, 10)));
                        try
                        {
                            // Convert the message body to a byte array - assumes message is simply JSON
                            Microsoft.Azure.ServiceBus.Message serviceBusmessage = new Microsoft.Azure.ServiceBus.Message(Encoding.UTF8.GetBytes(message.Body.ToString()));
                            await _topicClient.SendAsync(serviceBusmessage);
                            Trace.TraceInformation("QueueProcessor: Message successfully written to the service bus.");
                        }
                        catch (ServiceBusException ex)
                        {
                            Trace.TraceError($"QueueProcessor: Failed to write message to the service bus. Error is: {ex.Message}");
                            // Attempt to send message to the Service Bus failed - place message back on the MSMQ queue
                            messageQueue.Send(message);
                        }
                    }
                    catch (MessageQueueException)
                    {
                        // No message received - continue
                    }
                    catch
                    {
                        Trace.TraceError($"QueueProcessor: Exception occurred retrieving messages from MSMQ. Exiting.");
                        throw;
                    }
                } while (true);
                _isRunning = false;
            }
            else
            {
                Trace.TraceError($"QueueProcessor: Queue {_queueName} does not exist");
            }
        }

        public void Stop()
        {
            Trace.TraceInformation("QueueProcessor: Stop requested.");
            _isStopped = true;
            do
            {
                Task.Delay(1000).Wait();
            } while (_isRunning);
            Trace.TraceInformation("QueueProcessor: Completed processing messages.");
        }

        public void CreateSampleData()
        {
            // Put some sample messages in the MSMQ queue for testing
            // Use the ActiveXMessageFormatter for string data - use the XmlMessageFormatter for objects
            MessageQueue messageQueue = new MessageQueue(_queueName);
            System.Messaging.Message message = new System.Messaging.Message("{ \"id\": \"12345\" }", new ActiveXMessageFormatter());
            messageQueue.Send(message);
            message = new System.Messaging.Message("{ \"id\": \"54321\" }", new ActiveXMessageFormatter());
            messageQueue.Send(message);
            message = new System.Messaging.Message("{ \"id\": \"67890\" }", new ActiveXMessageFormatter());
            messageQueue.Send(message);
            message = new System.Messaging.Message("{ \"id\": \"09876\" }", new ActiveXMessageFormatter());
            messageQueue.Send(message);
        }
    }
}
