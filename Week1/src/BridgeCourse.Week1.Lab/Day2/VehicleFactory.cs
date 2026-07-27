using System;

namespace BridgeCourse.Week1.Lab.Day2
{
    /// <summary>
    /// IVehicle interface defining operations for vehicles.
    /// </summary>
    public interface IVehicle
    {
        string Drive();
    }

    /// <summary>
    /// Concrete vehicle type: Car
    /// </summary>
    public class Car : IVehicle
    {
        public string Drive() => "Driving a sleek, aerodynamic Car.";
    }

    /// <summary>
    /// Concrete vehicle type: Bike
    /// </summary>
    public class Bike : IVehicle
    {
        public string Drive() => "Riding a light and speedy Bike.";
    }

    /// <summary>
    /// Concrete vehicle type: Truck
    /// </summary>
    public class Truck : IVehicle
    {
        public string Drive() => "Driving a massive, heavy-duty Truck.";
    }

    #region Simple Factory Pattern

    /// <summary>
    /// Simple Factory that returns vehicles based on string type identifiers.
    /// </summary>
    public static class VehicleFactory
    {
        public static IVehicle CreateVehicle(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Vehicle type cannot be null or empty.", nameof(type));
            }

            return type.ToLowerInvariant() switch
            {
                "car" => new Car(),
                "bike" => new Bike(),
                "truck" => new Truck(),
                _ => throw new ArgumentException($"Unknown vehicle type: '{type}'", nameof(type))
            };
        }
    }

    #endregion

    #region Factory Method Pattern

    /// <summary>
    /// Abstract Creator defining the Factory Method interface.
    /// </summary>
    public abstract class VehicleCreator
    {
        /// <summary>
        /// The Factory Method that subclasses implement.
        /// </summary>
        public abstract IVehicle CreateVehicle();

        /// <summary>
        /// Demonstrates that the creator's primary business logic relies on the product,
        /// decoupled from its concrete implementation.
        /// </summary>
        public string DeliverAndDrive()
        {
            IVehicle vehicle = CreateVehicle();
            return $"[Delivery Operations] System dispatched product. Result: {vehicle.Drive()}";
        }
    }

    /// <summary>
    /// Concrete Creator for Cars.
    /// </summary>
    public class CarFactory : VehicleCreator
    {
        public override IVehicle CreateVehicle() => new Car();
    }

    /// <summary>
    /// Concrete Creator for Bikes.
    /// </summary>
    public class BikeFactory : VehicleCreator
    {
        public override IVehicle CreateVehicle() => new Bike();
    }

    /// <summary>
    /// Concrete Creator for Trucks.
    /// </summary>
    public class TruckFactory : VehicleCreator
    {
        public override IVehicle CreateVehicle() => new Truck();
    }

    #endregion
}
