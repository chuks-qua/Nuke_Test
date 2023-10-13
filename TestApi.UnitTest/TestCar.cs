using TestApi.Models;

namespace TestApi.UnitTest
{
    public class TestCar
    {
        [Fact]
        public void CanCreate()
        {
            Car car = new Car { Name = "A", Color = "B", YearManufactured = 1 };
            var (a, b, c) = ("A", "B", 1);
            Assert.Equal(a, car.Name);
            Assert.Equal(b, car.Color);
            Assert.Equal(c, car.YearManufactured);
        }
    }
}