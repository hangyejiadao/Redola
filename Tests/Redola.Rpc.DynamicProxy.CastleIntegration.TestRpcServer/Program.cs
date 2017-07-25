﻿using System;
using Logrila.Logging;
using Logrila.Logging.NLogIntegration;
using Redola.ActorModel;
using Redola.Rpc.TestContracts;

namespace Redola.Rpc.DynamicProxy.CastleIntegration.TestRpcServer
{
    class Program
    {
        static ILog _log;

        static Program()
        {
            NLogLogger.Use();
            _log = Logger.Get<Program>();
        }

        static void Main(string[] args)
        {
            var localXmlFilePath = Environment.CurrentDirectory + @"\\XmlConfiguration\\ActorConfiguration.xml";
            var localXmlFileActorConfiguration = LocalXmlFileActorConfiguration.Load(localXmlFilePath);
            var localXmlFileActorDirectory = new LocalXmlFileActorDirectory(localXmlFileActorConfiguration);

            var localActor = new RpcActor(localXmlFileActorConfiguration);

            localActor.Register<IHelloService>(new HelloService());
            localActor.Register<ICalcService>(new CalcService());
            localActor.Register<IOrderService>(new OrderService());

            localActor.Bootup(localXmlFileActorDirectory);

            while (true)
            {
                try
                {
                    string text = Console.ReadLine().ToLowerInvariant();
                    if (text == "quit" || text == "exit")
                    {
                        break;
                    }
                    else if (text == "reconnect")
                    {
                        localActor.Shutdown();
                        localActor.Bootup();
                    }
                    else
                    {
                        _log.WarnFormat("Cannot parse the operation for input [{0}].", text);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }

            localActor.Shutdown();
        }
    }
}
