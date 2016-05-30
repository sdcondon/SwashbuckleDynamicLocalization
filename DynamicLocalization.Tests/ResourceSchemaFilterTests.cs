namespace Swashbuckle.DynamicLocalization.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Swashbuckle.DynamicLocalization.Tests.TestData;
    using Swashbuckle.Swagger;
    using System.Collections.Generic;

    /// <summary>
    /// Unit tests for the <see cref="ResourceSchemaFilter" /> class.
    /// </summary>
    [TestClass]
    public class ResourceSchemaFilterTests
    {
        [TestMethod]
        public void Apply_WhenAllInformationIsPresent()
        {
            // Arrange
            ResourceSchemaFilter filter = new ResourceSchemaFilter(Documentation.ResourceManager);

            Schema model = new Schema()
            {
                properties = new Dictionary<string, Schema>()
                {
                    { "FirstProperty", new Schema() },
                    { "SecondProperty", new Schema() }
                }
            };

            // Act
            filter.Apply(model, null, typeof(DocumentedModel));

            // Assert
            Assert.AreEqual("The test model.", model.description);
            Assert.AreEqual("The first property", model.properties["FirstProperty"].description);
            Assert.AreEqual("The second property", model.properties["SecondProperty"].description);
        }

        [TestMethod]
        public void Apply_WhenNoInformationIsPresent()
        {
            // Arrange
            ResourceSchemaFilter filter = new ResourceSchemaFilter(Documentation.ResourceManager);

            Schema model = new Schema()
            {
                properties = new Dictionary<string, Schema>()
                {
                    { "FirstProperty", new Schema() },
                    { "SecondProperty", new Schema() }
                }
            };

            // Act
            filter.Apply(model, null, typeof(UndocumentedModel));

            // Assert
            Assert.AreEqual(null, model.description);
            Assert.AreEqual(null, model.properties["FirstProperty"].description);
            Assert.AreEqual(null, model.properties["SecondProperty"].description);
        }
    }
}
