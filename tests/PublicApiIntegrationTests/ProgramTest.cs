extern alias PublicApiAlias;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using PublicApiProgram = PublicApiAlias::Program;

namespace PublicApiIntegrationTests
{
    [TestClass]
    public class ProgramTest
    {
        private static WebApplicationFactory<PublicApiProgram>? _application;

        public static HttpClient NewClient
        {
            get
            {
                return _application!.CreateClient();
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            _application = new WebApplicationFactory<PublicApiProgram>();

        }
    }
}
