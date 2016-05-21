﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Resources;
using ResxDescriptionFilters.Tests.TestData;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;

namespace ResxDescriptionFilters.Tests
{
    [TestClass]
    public class ResourceOperationFilterTests
    {
        [TestMethod]
        public void Apply_WhenAllInformationPresent()
        {
            // Arrange
            ResourceOperationFilter filter = new ResourceOperationFilter(Documentation.ResourceManager);

            Operation operation = new Operation()
            {
                parameters = new List<Parameter>()
                {
                    new Parameter() { name = "id" }
                }
            };

            Type controllerType = typeof(DocumentedController);
            var controllerDescriptor = new HttpControllerDescriptor(new HttpConfiguration(), nameof(DocumentedController), controllerType);
            HttpActionDescriptor actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, controllerType.GetMethod("Get"));
            ApiDescription apiDescription = new ApiDescription()
            {
                ActionDescriptor = actionDescriptor
            };

            // Act
            filter.Apply(operation, null, apiDescription);

            // Assert
            Assert.AreEqual("Description of the getter.", operation.description);
            Assert.AreEqual("Summary of the getter.", operation.summary);
            Assert.AreEqual("The id of the thing to get", operation.parameters.Single(p => p.name == "id").description);
        }
    }
}
