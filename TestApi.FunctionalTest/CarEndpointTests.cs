using System.Net.Http.Json;
using TestApi.Models;

namespace TestApi.FunctionalTest
{
    public class CarEndpointTests
    {
        private static readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new("https://localhost:44307") };

        [Fact]
        public async void ListCars()
        {
            var cars = await _httpClient.GetFromJsonAsync<List<Car>>("/Cars");
            Assert.NotNull(cars);
            Assert.Equal(3, cars.Count);
        }

        [Fact]
        public async Task GetVolvo()
        {
            var car = await _httpClient.GetFromJsonAsync<Car>("/Cars/Volvo");
            Assert.NotNull(car);
            Assert.Equal("Volvo", car.Name);
        }
    }
}