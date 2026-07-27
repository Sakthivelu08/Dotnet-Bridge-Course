using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BridgeCourse.Week1.Lab.Day1;
using BridgeCourse.Week1.Lab.Day2;
using BridgeCourse.Week1.Lab.Day3;
using BridgeCourse.Week1.Lab.Day4;

namespace BridgeCourse.Week1.Lab
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("     Dotnet Bridge Course - Week 1: Lab         ");
            Console.WriteLine("=================================================");

            // Check if run in non-interactive/automated mode
            if (args.Length > 0 && args[0].ToLower() == "--auto")
            {
                Console.WriteLine("Running in automated mode for Days 1, 2, 3, and 4...");
                RunExceptionDemo();
                RunDisposalDemo();
                await RunAsyncDemo();
                
                RunSingletonDemo();
                RunFactoryDemo();
                RunObserverDemo();

                RunStrategyDemo();
                RunRepoUoWDemo();
                RunAdapterFacadeDemo();

                RunTplDemo();
                RunReflectionDemo();
                RunAttributeDemo();
                return;
            }

            while (true)
            {
                Console.WriteLine("\nChoose Day and Task to run:");
                Console.WriteLine("--- Day 1 ---");
                Console.WriteLine("1. Task 1.1: Custom Exception & Catch Order");
                Console.WriteLine("2. Task 1.2: TempFileManager (IDisposable)");
                Console.WriteLine("3. Task 1.3: Async User Data Fetcher");
                Console.WriteLine("--- Day 2 ---");
                Console.WriteLine("4. Task 1.4: Thread-safe Logger Singleton");
                Console.WriteLine("5. Task 1.5: Vehicle Factory & Factory Method");
                Console.WriteLine("6. Task 1.6: StockTicker (Observer & Events)");
                Console.WriteLine("--- Day 3 ---");
                Console.WriteLine("7. Task 1.7: ShoppingCart Strategy Swap");
                Console.WriteLine("8. Task 1.8: Repository & Unit of Work");
                Console.WriteLine("9. Task 1.9: XmlReportAdapter & OrderFacade");
                Console.WriteLine("--- Day 4 ---");
                Console.WriteLine("10. Task 1.10: TPL Performance Benchmark");
                Console.WriteLine("11. Task 1.11: Invoice Reflection Inspection");
                Console.WriteLine("12. Task 1.12: Custom Attribute Validator");
                Console.WriteLine("13. Exit");
                Console.Write("Enter your choice (1-13): ");

                var input = Console.ReadLine();
                if (input == "13" || input == null) break;

                try
                {
                    switch (input)
                    {
                        case "1":
                            RunExceptionDemo();
                            break;
                        case "2":
                            RunDisposalDemo();
                            break;
                        case "3":
                            await RunAsyncDemo();
                            break;
                        case "4":
                            RunSingletonDemo();
                            break;
                        case "5":
                            RunFactoryDemo();
                            break;
                        case "6":
                            RunObserverDemo();
                            break;
                        case "7":
                            RunStrategyDemo();
                            break;
                        case "8":
                            RunRepoUoWDemo();
                            break;
                        case "9":
                            RunAdapterFacadeDemo();
                            break;
                        case "10":
                            RunTplDemo();
                            break;
                        case "11":
                            RunReflectionDemo();
                            break;
                        case "12":
                            RunAttributeDemo();
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nDemo execution failed: {ex.Message}");
                }
            }

            Console.WriteLine("\nExiting. Thank you!");
        }

        #region Day 1 Demos

        static void RunExceptionDemo()
        {
            Console.WriteLine("\n--- Running Exception & Catch Order Demo ---");
            var account = new BankAccount(100m);
            try
            {
                account.Withdraw(40m);
                account.Withdraw(80m); 
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"[Main Catch] Caught InsufficientFundsException! Deficit: {ex.DeficitAmount:C}");
            }

            Console.WriteLine("\nParsing testing for FormatException, OverflowException, and general Exception:");
            Console.WriteLine($"Result with '123': {ExceptionOrderDemo.ParseAndProcess("123")}");
            Console.WriteLine($"Result with 'abc': {ExceptionOrderDemo.ParseAndProcess("abc")}");
            Console.WriteLine($"Result with '9999999999999': {ExceptionOrderDemo.ParseAndProcess("9999999999999")}");
            Console.WriteLine($"Result with null: {ExceptionOrderDemo.ParseAndProcess(null)}");
        }

        static void RunDisposalDemo()
        {
            Console.WriteLine("\n--- Running Disposal Demo ---");
            string pathInsideUsing;
            using (var manager = new TempFileManager())
            {
                pathInsideUsing = manager.FilePath;
                Console.WriteLine($"[Inside using] Temp file exists? {File.Exists(pathInsideUsing)}");
            }
            Console.WriteLine($"[Outside using] Temp file exists? {File.Exists(pathInsideUsing)}");
        }

        static async Task RunAsyncDemo()
        {
            Console.WriteLine("\n--- Running Async User Data Fetcher Demo ---");
            var fetcher = new UserDataFetcher();
            var userIds = new List<int> { 101, 102, 103 };

            var (seqData, seqTime) = await fetcher.RunSequentialFetchesAsync(userIds);
            var (conData, conTime) = await fetcher.RunConcurrentFetchesAsync(userIds);

            Console.WriteLine("\nSummary of Fetch Performance:");
            Console.WriteLine($"- Sequential duration: {seqTime} ms");
            Console.WriteLine($"- Concurrent duration: {conTime} ms");
            double speedup = (double)seqTime / conTime;
            Console.WriteLine($"- Concurrent execution is {speedup:F2}x faster!");
        }

        #endregion

        #region Day 2 Demos

        static void RunSingletonDemo()
        {
            Console.WriteLine("\n--- Running Logger Singleton Concurrency Demo ---");
            var threads = new List<Thread>();
            var tasks = new List<Task>();

            Console.WriteLine("Spawning 5 Threads...");
            for (int i = 1; i <= 5; i++)
            {
                int id = i;
                var thread = new Thread(() =>
                {
                    Logger.Instance.Log($"Message from Thread {id}");
                });
                threads.Add(thread);
            }

            Console.WriteLine("Starting 5 Tasks...");
            for (int i = 1; i <= 5; i++)
            {
                int id = i;
                tasks.Add(Task.Run(() =>
                {
                    Logger.Instance.Log($"Message from Task {id}");
                }));
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"\nDemo finished. Final proof of Singleton: Logger.Instance hashcode is {Logger.Instance.GetHashCode()}.");
        }

        static void RunFactoryDemo()
        {
            Console.WriteLine("\n--- Running Vehicle Factory Demo ---");
            
            Console.WriteLine("\n1. Simple Factory Creation (without 'new' in caller):");
            IVehicle car = VehicleFactory.CreateVehicle("car");
            IVehicle bike = VehicleFactory.CreateVehicle("bike");
            IVehicle truck = VehicleFactory.CreateVehicle("truck");
            
            Console.WriteLine($"[Simple Factory] {car.GetType().Name} says: {car.Drive()}");
            Console.WriteLine($"[Simple Factory] {bike.GetType().Name} says: {bike.Drive()}");
            Console.WriteLine($"[Simple Factory] {truck.GetType().Name} says: {truck.Drive()}");

            Console.WriteLine("\n2. Factory Method Creation (without 'new' of product in caller):");
            VehicleCreator carCreator = new CarFactory();
            VehicleCreator bikeCreator = new BikeFactory();
            VehicleCreator truckCreator = new TruckFactory();

            Console.WriteLine(carCreator.DeliverAndDrive());
            Console.WriteLine(bikeCreator.DeliverAndDrive());
            Console.WriteLine(truckCreator.DeliverAndDrive());
        }

        static void RunObserverDemo()
        {
            Console.WriteLine("\n--- Running StockTicker Observer Demo ---");

            Console.WriteLine("\nA. Testing Custom IObserver/ICustomSubject Interfaces:");
            var customTicker = new CustomStockTicker("GOOGL", 180.50m);
            var customInvestor1 = new CustomInvestor("Alice");
            var customInvestor2 = new CustomInvestor("Bob");

            customTicker.Register(customInvestor1);
            customTicker.Register(customInvestor2);

            Console.WriteLine("Changing price of GOOGL to 182.10m:");
            customTicker.Price = 182.10m;

            customTicker.Unregister(customInvestor1);
            Console.WriteLine("\nAlice unregistered. Changing price of GOOGL to 185.00m:");
            customTicker.Price = 185.00m;

            Console.WriteLine("\nB. Testing standard C# Events:");
            var eventTicker = new EventStockTicker("MSFT", 420.00m);
            var eventInvestor1 = new EventInvestor("Charlie");
            var eventInvestor2 = new EventInvestor("David");

            eventTicker.StockPriceChanged += eventInvestor1.OnPriceChanged;
            eventTicker.StockPriceChanged += eventInvestor2.OnPriceChanged;

            Console.WriteLine("Changing price of MSFT to 425.50m:");
            eventTicker.Price = 425.50m;

            eventTicker.StockPriceChanged -= eventInvestor1.OnPriceChanged;
            Console.WriteLine("\nCharlie unsubscribed. Changing price of MSFT to 430.00m:");
            eventTicker.Price = 430.00m;
        }

        #endregion

        #region Day 3 Demos

        static void RunStrategyDemo()
        {
            Console.WriteLine("\n--- Running ShoppingCart Strategy Swap Demo ---");
            
            var cart = new ShoppingCart(150.75m, new CreditCardPaymentStrategy("1234567890123456", "John Doe"));
            cart.Checkout();

            cart.SetPaymentStrategy(new UpiPaymentStrategy("john.doe@okaxis"));
            cart.TotalAmount = 85.00m;
            cart.Checkout();

            cart.SetPaymentStrategy(new NetBankingPaymentStrategy("HDFC Bank"));
            cart.TotalAmount = 210.50m;
            cart.Checkout();
        }

        static void RunRepoUoWDemo()
        {
            Console.WriteLine("\n--- Running Repository and Unit of Work CRUD Demo ---");
            StudentRepository.Clear();
            CourseRepository.Clear();

            using (IUnitOfWork uow = new UnitOfWork())
            {
                var student1 = new Student { Name = "Sakthi", Email = "sakthi@example.com" };
                var student2 = new Student { Name = "Velu", Email = "velu@example.com" };
                uow.Students.Add(student1);
                uow.Students.Add(student2);

                var course1 = new Course { Title = "C# Advanced Concepts", Code = "CS-101" };
                var course2 = new Course { Title = "Design Patterns in .NET", Code = "CS-102" };
                uow.Courses.Add(course1);
                uow.Courses.Add(course2);

                uow.Save();

                Console.WriteLine("\nListing Students stored in database seam:");
                foreach (var s in uow.Students.GetAll())
                {
                    Console.WriteLine($"- Student ID: {s.Id}, Name: {s.Name}, Email: {s.Email}");
                }

                student2.Email = "velu.updated@example.com";
                uow.Students.Update(student2);
                uow.Courses.Delete(course1.Id);

                uow.Save();
            }
        }

        static void RunAdapterFacadeDemo()
        {
            Console.WriteLine("\n--- Running Adapter and Facade Demo ---");

            Console.WriteLine("\n1. Running OrderFacade (Coordinating inventory, payment, and shipping):");
            var orderFacade = new OrderFacade();
            orderFacade.PlaceOrder(5001, 101, 2, 500.00m, "upi", "sakthi@okaxis");

            Console.WriteLine("\n2. Running XmlReportAdapter (Adapting JSON report into Legacy XML generator):");
            var generator = new XmlReportGenerator();
            var adapter = new XmlReportAdapter(generator);

            var localJsonReport = new JsonReportData
            {
                OrderId = 5001,
                TotalAmount = 500.00m
            };

            string result = adapter.ConvertAndGenerateReport(localJsonReport);
            Console.WriteLine($"Result from legacy adapter system: {result}");
        }

        #endregion

        #region Day 4 Demos

        static void RunTplDemo()
        {
            Console.WriteLine("\n--- Running TPL Concurrency Benchmarks ---");
            Console.WriteLine("Warning: Running sequential benchmarks might take up to 10 seconds.");
            
            long tParallel = TplComparison.RunParallelForEach();
            long tThreadPool = TplComparison.RunThreadPoolTasks();
            long tRawThreads = TplComparison.RunRawThreads();
            long tSequential = TplComparison.RunSequential();

            Console.WriteLine("\nSummary of Benchmarks:");
            Console.WriteLine($"- Parallel.ForEach: {tParallel} ms");
            Console.WriteLine($"- Task.Run (ThreadPool): {tThreadPool} ms");
            Console.WriteLine($"- Raw OS Threads: {tRawThreads} ms");
            Console.WriteLine($"- Sequential Loop: {tSequential} ms");

            double speedup = (double)tSequential / tParallel;
            Console.WriteLine($"Parallel.ForEach is {speedup:F2}x faster than a sequential loop!");
        }

        static void RunReflectionDemo()
        {
            Console.WriteLine("\n--- Running Reflection Demonstration ---");
            
            // 1. Inspect metadata
            Console.WriteLine("\n1. Inspecting Invoice Metadata dynamically:");
            string metadata = InvoiceReflector.ReflectAndInspect();
            Console.WriteLine(metadata);

            // 2. Manipulate instance
            Console.WriteLine("\n2. Instantiating and modifying object entirely via Reflection:");
            Invoice reflectedInvoice = InvoiceReflector.CreateAndModifyViaReflection(777, "Original Customer", 950.25m, "Reflected Customer!");
            
            Console.WriteLine($"Resulting invoice summary: {reflectedInvoice.GetSummary()}");
            Console.WriteLine($"Verified Customer Name: '{reflectedInvoice.CustomerName}'");
        }

        static void RunAttributeDemo()
        {
            Console.WriteLine("\n--- Running Custom Attribute Validator Demo ---");

            // User 1: Valid Name (Length: 6)
            var validUser = new User { Name = "Sakthi", Email = "sakthi@example.com" };
            var (isValid1, warnings1) = ValidationEngine.Validate(validUser);
            Console.WriteLine($"Validating '{validUser.Name}': IsValid = {isValid1}");
            
            // User 2: Over-length Name (Length: 17)
            var invalidUser = new User { Name = "Sakthivelu Selvam", Email = "sakthivelu@example.com" };
            var (isValid2, warnings2) = ValidationEngine.Validate(invalidUser);
            Console.WriteLine($"\nValidating '{invalidUser.Name}': IsValid = {isValid2}");
            foreach (var warning in warnings2)
            {
                Console.WriteLine($"[WARNING TRIGGERED] {warning}");
            }
        }

        #endregion
    }
}
