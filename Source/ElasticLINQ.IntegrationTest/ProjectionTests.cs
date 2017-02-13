using System;
using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class ProjectionTests
    {
        [Fact]
        public void ProjectWithObjectInitializerAndNoContructorArgs()
        {

            DataAssert.Same<WebUser>(q => q.Select(x => new WebUser { Id = x.Id, Email = x.Forename }));
        }

        [Fact]
        public void ProjectWithObjectInitializerAndContructorArgs()
        {
            DataAssert.Same<WebUser>(q => q.Select(x => new WebUser(x.PasswordHash) { Surname = x.Email }));
        }

        [Fact]
        public void ProjectWithContructorArgsAndNoObjectInitializer()
        {
            DataAssert.Same<WebUser>(q => q.Select(x => new WebUser(x.Username)));
        }
    }
}