using System;
using AdministrationAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace TestProject
{
    public class BaseControllerTest
    {

        public BaseControllerTest()
        {

        }

        public HttpContext GetAuthorizedHttpContext()
        {
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.Request.Method).Returns("GET");
            mockHttpContext.SetupGet(c => c.Request.Headers["Authorization"]).Returns("Bearer abcdef");
            return mockHttpContext.Object;
        }
    }
}

