using NUnit.Framework;

namespace WebApi.Integration.Test
{
    public class Tests
    {
        //private readonly WebApi<WebApi.Startup> _factory;

       // public Tests(WebApplicationFactory<WebApi.Startup> factory)
        //{
           // _factory = factory;
        //}

        [SetUp]
        public void Setup()
        {
        }

        //[Test]
        //public void Post_DeleteMessageHandler_ReturnsRedirectToRoot()
        //{
        //    // Arrange
        //    var client = _factory.WithWebHostBuilder(builder =>
        //        {
        //            builder.ConfigureServices(services =>
        //            {
        //                var serviceProvider = services.BuildServiceProvider();

        //                using (var scope = serviceProvider.CreateScope())
        //                {
        //                    var scopedServices = scope.ServiceProvider;
        //                    var db = scopedServices
        //                        .GetRequiredService<ApplicationDbContext>();
        //                    var logger = scopedServices
        //                        .GetRequiredService<ILogger<IndexPageTests>>();

        //                    try
        //                    {
        //                        Utilities.ReinitializeDbForTests(db);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        logger.LogError(ex, "An error occurred seeding " +
        //                                            "the database with test messages. Error: {Message}",
        //                            ex.Message);
        //                    }
        //                }
        //            });
        //        })
        //        .CreateClient(new WebApplicationFactoryClientOptions
        //        {
        //            AllowAutoRedirect = false
        //        });
        //    var defaultPage = await client.GetAsync("/");
        //    var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

        //    //Act
        //    var response = await client.SendAsync(
        //        (IHtmlFormElement)content.QuerySelector("form[id='messages']"),
        //        (IHtmlButtonElement)content.QuerySelector("form[id='messages']")
        //            .QuerySelector("div[class='panel-body']")
        //            .QuerySelector("button"));

        //    // Assert
        //    Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
        //    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        //    Assert.Equal("/", response.Headers.Location.OriginalString);
        //}
    }
}