using System;
using System.Collections.Generic;

namespace BridgeCourse.Week1.Lab.Day2
{
    #region 1. Custom IObserver / ISubject Interface Implementation

    /// <summary>
    /// Custom Observer interface.
    /// </summary>
    public interface ICustomObserver
    {
        void Update(string stockSymbol, decimal price);
    }

    /// <summary>
    /// Custom Subject interface.
    /// </summary>
    public interface ICustomSubject
    {
        void Register(ICustomObserver observer);
        void Unregister(ICustomObserver observer);
        void Notify();
    }

    /// <summary>
    /// StockTicker implementing custom subject interface.
    /// </summary>
    public class CustomStockTicker : ICustomSubject
    {
        private readonly List<ICustomObserver> _observers = new List<ICustomObserver>();
        private decimal _price;

        public string StockSymbol { get; }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    Notify();
                }
            }
        }

        public CustomStockTicker(string stockSymbol, decimal initialPrice)
        {
            StockSymbol = stockSymbol;
            _price = initialPrice;
        }

        public void Register(ICustomObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Unregister(ICustomObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(StockSymbol, _price);
            }
        }
    }

    /// <summary>
    /// Investor implementing custom observer interface.
    /// </summary>
    public class CustomInvestor : ICustomObserver
    {
        public string Name { get; }
        public List<string> UpdatesReceived { get; } = new List<string>();

        public CustomInvestor(string name)
        {
            Name = name;
        }

        public void Update(string stockSymbol, decimal price)
        {
            string log = $"[Custom Observer] Investor {Name} notified: {stockSymbol} is now {price:C}";
            UpdatesReceived.Add(log);
            Console.WriteLine(log);
        }
    }

    #endregion

    #region 2. C# Events Implementation

    /// <summary>
    /// StockTicker using standard C# events.
    /// </summary>
    public class EventStockTicker
    {
        private decimal _price;

        public string StockSymbol { get; }

        // Event backing the delegate
        public event Action<string, decimal>? StockPriceChanged;

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnStockPriceChanged(StockSymbol, _price);
                }
            }
        }

        public EventStockTicker(string stockSymbol, decimal initialPrice)
        {
            StockSymbol = stockSymbol;
            _price = initialPrice;
        }

        protected virtual void OnStockPriceChanged(string stockSymbol, decimal price)
        {
            // Triggers the delegate chain
            StockPriceChanged?.Invoke(stockSymbol, price);
        }
    }

    /// <summary>
    /// Investor subscribing to standard C# events.
    /// </summary>
    public class EventInvestor
    {
        public string Name { get; }
        public List<string> UpdatesReceived { get; } = new List<string>();

        public EventInvestor(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Event handler matching Action<string, decimal> signature.
        /// </summary>
        public void OnPriceChanged(string stockSymbol, decimal price)
        {
            string log = $"[C# Event] Investor {Name} notified: {stockSymbol} is now {price:C}";
            UpdatesReceived.Add(log);
            Console.WriteLine(log);
        }
    }

    #endregion

    /*
     * COMPARISON: CUSTOM INTERFACE OBSERVER vs. C# EVENTS
     * ----------------------------------------------------
     * 
     * 1. Coupling and Flexibility:
     *    - Interface Observer: Requires the Observer class (Investor) to implement ICustomObserver. 
     *      This couples the class to a specific interface signature. 
     *    - C# Events: Decoupled. The Observer doesn't need to implement any interface; it only needs 
     *      to supply a method (or a lambda expression) that matches the event delegate signature.
     * 
     * 2. Boilerplate Code:
     *    - Interface Observer: The Subject must manually write logic to manage a List<T>, add/remove
     *      observers, and loop through the list during notification.
     *    - C# Events: Managing the list of subscribers and safe multicast dispatching is handled
     *      automatically by the C# compiler/CLR delegate invocation list.
     * 
     * 3. Memory Management (The Lapsed Listener Problem):
     *    - Both: If an observer is registered to a long-lived subject, it will not be garbage collected
     *      until it is unregistered (using Unregister or -=). 
     *    - C# Events: Can be slightly easier to forget to clean up (e.g., event subscription inside
     *      short-lived objects leads to leaks unless explicit -= is called in Dispose).
     * 
     * 4. Multi-threading and safety:
     *    - Interface Observer: Custom locking is required on the subscriber list to prevent 
     *      modification while enumerating during notifications.
     *    - C# Events: Event subscription adding/removing is inherently thread-safe in modern .NET
     *      (compiler emits thread-safe delegate combination methods).
     */
}
