using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true);

var configurationRoot = builder.Build();
var accessToken = configurationRoot["accesstoken"];

var flurlClient = new FlurlClient();
flurlClient.Settings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver()
});

await "https://api.usecloudpress.com/v2/exports/google-docs"
    .WithClient(flurlClient)
    .WithOAuthBearerToken(accessToken)
    .PostMultipartAsync(content =>
    {
        content.AddFile("file",
            Path.Combine(Directory.GetCurrentDirectory(), "document.json"),
            contentType: "application/vnd.google-apps.document",
            fileName: "file.txt");
        content.AddJson("options",
            new
            {
                Destination = 0, // Set your connection ID
                FieldMappings = new
                {
                    PublishedAt = new
                    {
                        Value = new DateTimeOffset(2022, 3, 21, 11, 0, 0, 0, TimeSpan.FromHours(7))
                    },
                    Slug = new
                    {
                        Value = new
                        {
                            _type = "slug",
                            Current = "a-sample-slug"
                        }
                    },
                    Author = new
                    {
                        Value = new
                        {
                            _ref = "4e2d254a-f378-48b8-9210-8a943cb5beff",
                            _type = "reference"
                        }
                    },
                    Categories = new
                    {
                        Value = new[]
                        {
                            new
                            {
                                _key = "abcd1234",
                                _ref = "817decbf-5f2d-47db-98ab-31c243ffdf49",
                                _type = "reference"
                            }
                        }
                    }
                }
            });
    });
