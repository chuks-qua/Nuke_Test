using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TestApi.Models;

namespace TestApi.FunctionalTest
{
    public class CarEndpointTests
    {
        private static readonly HttpClient _httpClient = new HttpClient(
            new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true });

        private static readonly string[] BaseUrls = {
            "https://localhost:44307",
            "http://localhost:44307"
        };

        [Fact]
        public async Task ListCars()
        {
            List<Car> cars = null;
            foreach (var baseUrl in BaseUrls)
            {
                try
                {
                    cars = await _httpClient.GetFromJsonAsync<List<Car>>($"{baseUrl}/Cars");
                    break;
                }
                catch (HttpRequestException)
                {
                   
                }
            }

            Assert.NotNull(cars);
            Assert.Equal(3, cars.Count);
        }

        [Fact]
        public async Task GetVolvo()
        {
            Car car = null;
            foreach (var baseUrl in BaseUrls)
            {
                try
                {
                    car = await _httpClient.GetFromJsonAsync<Car>($"{baseUrl}/Cars/Volvo");
                    break;
                    
                }
                catch (HttpRequestException)
                {
                   
                }
            }

            Assert.NotNull(car);
            Assert.Equal("Volvo", car.Name);
        }
    }
}
