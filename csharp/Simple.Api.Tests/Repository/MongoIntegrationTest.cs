using System;
using System.Collections.Generic;
using System.Text;
using Mongo2Go;
using MongoDB.Driver;
using Simple.Api.Repository;

namespace Simple.Api.Tests.Repository
{
    public class MongoIntegrationTest:IDisposable
    {
        internal static MongoDbRunner Runner;
        internal static void CreateConnection()
        {
            Runner = MongoDbRunner.Start(singleNodeReplSet: false);
        }

        public void Dispose()
        {
            Runner.Dispose();
        }
    }
}
