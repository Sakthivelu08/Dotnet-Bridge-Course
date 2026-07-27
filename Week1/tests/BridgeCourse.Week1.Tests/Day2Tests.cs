using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgeCourse.Week1.Lab.Day2;
using Xunit;

namespace BridgeCourse.Week1.Tests
{
    public class Day2Tests
    {
        #region Task 1.4 Singleton Tests

        [Fact]
        public async Task LoggerInstance_AlwaysReturnsSameInstance_AcrossMultipleTasks()
        {
            // Arrange
            var instances = new List<Logger>();
            var tasks = new List<Task>();
            object listLock = new object();

            // Act
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    Logger instance = Logger.Instance;
                    lock (listLock)
                    {
                        instances.Add(instance);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.NotEmpty(instances);
            Logger referenceInstance = instances[0];
            foreach (var instance in instances)
            {
                Assert.Same(referenceInstance, instance); // Same memory address/reference
                Assert.Equal(referenceInstance.GetHashCode(), instance.GetHashCode()); // Same hashcode
            }
        }

        #endregion

        #region Task 1.5 Factory & Factory Method Tests

        [Theory]
        [InlineData("car", typeof(Car))]
        [InlineData("bike", typeof(Bike))]
        [InlineData("truck", typeof(Truck))]
        public void SimpleVehicleFactory_CreatesCorrectType(string inputType, Type expectedType)
        {
            // Act
            IVehicle vehicle = VehicleFactory.CreateVehicle(inputType);

            // Assert
            Assert.NotNull(vehicle);
            Assert.IsType(expectedType, vehicle);
        }

        [Fact]
        public void SimpleVehicleFactory_UnknownType_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => VehicleFactory.CreateVehicle("submarine"));
        }

        [Fact]
        public void FactoryMethodCreators_CreateCorrectTypes()
        {
            // Arrange
            VehicleCreator carCreator = new CarFactory();
            VehicleCreator bikeCreator = new BikeFactory();
            VehicleCreator truckCreator = new TruckFactory();

            // Act
            IVehicle car = carCreator.CreateVehicle();
            IVehicle bike = bikeCreator.CreateVehicle();
            IVehicle truck = truckCreator.CreateVehicle();

            // Assert
            Assert.IsType<Car>(car);
            Assert.IsType<Bike>(bike);
            Assert.IsType<Truck>(truck);
            
            Assert.Contains("Driving a sleek", carCreator.DeliverAndDrive());
            Assert.Contains("Riding a light", bikeCreator.DeliverAndDrive());
            Assert.Contains("Driving a massive", truckCreator.DeliverAndDrive());
        }

        #endregion

        #region Task 1.6 Observer Tests

        [Fact]
        public void CustomInterfaceObserver_NotifiesAllSubscribedInvestors_AndStopsOnUnregister()
        {
            // Arrange
            var ticker = new CustomStockTicker("AAPL", 220.00m);
            var alice = new CustomInvestor("Alice");
            var bob = new CustomInvestor("Bob");

            ticker.Register(alice);
            ticker.Register(bob);

            // Act: Step 1 (Both are registered)
            ticker.Price = 225.00m;

            // Assert: Step 1
            Assert.Single(alice.UpdatesReceived);
            Assert.Single(bob.UpdatesReceived);
            Assert.Contains("Alice notified", alice.UpdatesReceived[0]);
            Assert.Contains("AAPL", alice.UpdatesReceived[0]);
            Assert.Contains("225", alice.UpdatesReceived[0]);
            
            Assert.Contains("Bob notified", bob.UpdatesReceived[0]);
            Assert.Contains("AAPL", bob.UpdatesReceived[0]);
            Assert.Contains("225", bob.UpdatesReceived[0]);

            // Act: Step 2 (Unregister Alice, update price)
            ticker.Unregister(alice);
            ticker.Price = 230.00m;

            // Assert: Step 2 (Alice does not receive, Bob does)
            Assert.Single(alice.UpdatesReceived); // Still 1
            Assert.Equal(2, bob.UpdatesReceived.Count); // Increased to 2
            Assert.Contains("Bob notified", bob.UpdatesReceived[1]);
            Assert.Contains("AAPL", bob.UpdatesReceived[1]);
            Assert.Contains("230", bob.UpdatesReceived[1]);
        }

        [Fact]
        public void EventBasedObserver_NotifiesAllSubscribedInvestors_AndStopsOnUnsubscribe()
        {
            // Arrange
            var ticker = new EventStockTicker("AAPL", 220.00m);
            var charlie = new EventInvestor("Charlie");
            var david = new EventInvestor("David");

            ticker.StockPriceChanged += charlie.OnPriceChanged;
            ticker.StockPriceChanged += david.OnPriceChanged;

            // Act: Step 1 (Both subscribed)
            ticker.Price = 225.00m;

            // Assert: Step 1
            Assert.Single(charlie.UpdatesReceived);
            Assert.Single(david.UpdatesReceived);
            Assert.Contains("Charlie notified", charlie.UpdatesReceived[0]);
            Assert.Contains("AAPL", charlie.UpdatesReceived[0]);
            Assert.Contains("225", charlie.UpdatesReceived[0]);
            
            Assert.Contains("David notified", david.UpdatesReceived[0]);
            Assert.Contains("AAPL", david.UpdatesReceived[0]);
            Assert.Contains("225", david.UpdatesReceived[0]);

            // Act: Step 2 (Unsubscribe Charlie, update price)
            ticker.StockPriceChanged -= charlie.OnPriceChanged;
            ticker.Price = 230.00m;

            // Assert: Step 2 (Charlie does not receive, David does)
            Assert.Single(charlie.UpdatesReceived); // Still 1
            Assert.Equal(2, david.UpdatesReceived.Count); // Increased to 2
            Assert.Contains("David notified", david.UpdatesReceived[1]);
            Assert.Contains("AAPL", david.UpdatesReceived[1]);
            Assert.Contains("230", david.UpdatesReceived[1]);
        }

        #endregion
    }
}
