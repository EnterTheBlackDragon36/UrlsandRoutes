using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Routing;
using Moq;
using System.Reflection;
using System.Web.Mvc;


namespace UrlsandRoutes.Tests
{
    [TestClass]
    public class RouteTests
    {
        private HttpContextBase CreateHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            // create the mock request
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            // create the mock response
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            // create the mock context, using the request and response
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);

            // return the mocked context
            return mockContext.Object;
        }

        private void TestRouteMatch(string url, string controller, string action, object routeProperties = null, string httpMethod = "GET")
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Act - process the route
            RouteData result = routes.GetRouteData(CreateHttpContext(url, httpMethod));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeProperties));
        }

        private bool TestIncomingRouteResult(RouteData routeResult, string controller, string action, object propertySet = null)
        {
            Func<object, object, bool> valCompare = (v1, v2) =>
                {
                    return StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;
                };

            bool result = valCompare(routeResult.Values["controller"], controller)
                && valCompare(routeResult.Values["action"], action);

            if (propertySet != null)
            {
                PropertyInfo[] propInfo = propertySet.GetType().GetProperties();
                foreach (PropertyInfo pi in propInfo)
                {
                    if (!(routeResult.Values.ContainsKey(pi.Name)
                        && valCompare(routeResult.Values[pi.Name],
                        pi.GetValue(propertySet, null))))
                    {

                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void TestRouteFail(string url)
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Act - process the route
            RouteData result = routes.GetRouteData(CreateHttpContext(url));

            // Assert
            Assert.IsTrue(result == null || result.Route == null);
        }

        //[TestMethod]
        //public void TestIncomingRoutes()
        //{
        //    // Check for the URL that we hope to receive
        //    TestRouteMatch("~/Admin/Index", "Admin", "Index");

        //    // Check that the values are being obtained from the segments
        //    TestRouteMatch("~/One/Two", "One", "Two");

        //    // Ensure that too many or too few segments fails to match
        //    TestRouteFail("~/Admin/Index/Segment");
        //    TestRouteFail("~/Admin");
        //}

        //[TestMethod]
        //public void TestIncomingRoutes()
        //{
        //    TestRouteMatch("~/", "Home", "Index");
        //    TestRouteMatch("~/Customer", "Customer", "Index");
        //    TestRouteMatch("~/Customer/List", "Customer", "List");
        //    TestRouteFail("~/Customer/List/All");
        //}

        //[TestMethod]
        //public void TestIncomingRoutes()
        //{
        //    TestRouteMatch("~/", "Home", "Index");
        //    TestRouteMatch("~/Customer", "Customer", "Index");
        //    TestRouteMatch("~/Shop/Index", "Home", "Index");
        //    TestRouteMatch("~/Customer/List", "Customer", "List");
        //    TestRouteFail("~/Customer/List/All");
        //}

        //[TestMethod]
        //public void TestIncomingRoutes()
        //{
        //    TestRouteMatch("~/", "Home", "Index", new { id = "DefaultId" });
        //    TestRouteMatch("~/Customer", "Customer", "index", new { id = "DefaultId" });
        //    TestRouteMatch("~/Customer/List", "Customer", "List",
        //        new { id = "DefaultId" });
        //    TestRouteMatch("~/Customer/List/All", "Customer", "List", new { id = "All" });
        //    TestRouteFail("~/Customer/List/All/Delete");
        //}


        //[TestMethod]
        //public void TestIncomingRoutes()
        //{
        //    TestRouteMatch("~/", "Home", "Index");
        //    TestRouteMatch("~/Customer", "Customer", "index");
        //    TestRouteMatch("~/Customer/List", "Customer", "List");
        //    TestRouteMatch("~/Customer/List/All", "Customer", "List", new { id = "All" });
        //    TestRouteFail("~/Customer/List/All/Delete");
        //}


        [TestMethod]
        public void TestIncomingRoutes()
        {
            TestRouteMatch("~/", "Home", "Index");
            TestRouteMatch("~/Home", "Home", "Index");
            TestRouteMatch("~/Home/Index", "Home", "Index");

            TestRouteMatch("~/Home/About", "Home", "About");
            TestRouteMatch("~/Home/About/MyId", "Home", "About", new { id = "MyId" });
            TestRouteMatch("~/Home/About/MyId/More/Segments", "Home", "About",
                new {
                    id = "MyId",
                    catchall = "More/Segments"
                });

            TestRouteFail("~/Home/OtherAction");
            TestRouteFail("~/Account/Index");
            TestRouteFail("~/Account/About");
        }


        [TestMethod]
        public void TestOutgoingRoutes()
        {
            // Arrange
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            RequestContext context = new RequestContext(CreateHttpContext(), new RouteData());

            // Act - generate the URL
            string result = UrlHelper.GenerateUrl(null, "Index", "Home", null, routes, context, true);

            // Assert
            Assert.AreEqual("/App/DoIndex", result);
        }
    }
}
