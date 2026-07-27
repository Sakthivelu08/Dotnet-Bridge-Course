using System;
using BridgeCourse.Week1.Lab.Day2;

namespace BridgeCourse.Week1.Lab.Day3
{
    #region Adapter Pattern

    /// <summary>
    /// Target: A JSON report structure used inside our system.
    /// </summary>
    public class JsonReportData
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Adaptee: A legacy / third-party generator that only processes XML formatted strings.
    /// </summary>
    public class XmlReportGenerator
    {
        public string GenerateXmlReport(string xmlContent)
        {
            // Simple validation that it is indeed XML formatted
            if (!xmlContent.StartsWith("<Report>") || !xmlContent.EndsWith("</Report>"))
            {
                throw new ArgumentException("Invalid report format: Expected XML report content.", nameof(xmlContent));
            }

            // Print report receipt
            Logger.Instance.Log($"[XmlReportGenerator] Received XML payload:\n{xmlContent}");
            return $"[SUCCESS] Legacy report processed successfully. Payload length: {xmlContent.Length} chars.";
        }
    }

    /// <summary>
    /// Adapter: Converts a JsonReportData instance into the XML format required by XmlReportGenerator.
    /// </summary>
    public class XmlReportAdapter
    {
        private readonly XmlReportGenerator _xmlReportGenerator;

        public XmlReportAdapter(XmlReportGenerator xmlReportGenerator)
        {
            _xmlReportGenerator = xmlReportGenerator ?? throw new ArgumentNullException(nameof(xmlReportGenerator));
        }

        /// <summary>
        /// Adapts JSON input into XML and triggers the third-party generator.
        /// </summary>
        public string ConvertAndGenerateReport(JsonReportData jsonData)
        {
            if (jsonData == null) throw new ArgumentNullException(nameof(jsonData));

            // Map JSON properties to XML representation
            string xmlPayload = $"<Report>" +
                                $"<OrderId>{jsonData.OrderId}</OrderId>" +
                                $"<Total>{jsonData.TotalAmount:F2}</Total>" +
                                $"</Report>";

            Logger.Instance.Log($"[XmlReportAdapter] Adapting JSON Order {jsonData.OrderId} to XML payload.");
            return _xmlReportGenerator.GenerateXmlReport(xmlPayload);
        }
    }

    #endregion

    #region Facade Pattern

    // Subsystem A
    public class InventorySystem
    {
        public bool CheckStock(int itemId, int quantity)
        {
            Logger.Instance.Log($"[InventorySubsystem] Checking stock for Item ID: {itemId}, Qty: {quantity}...");
            return true; // Assume always in stock for demo
        }

        public void DeductStock(int itemId, int quantity)
        {
            Logger.Instance.Log($"[InventorySubsystem] Deducting Qty: {quantity} of Item ID: {itemId} from stock.");
        }
    }

    // Subsystem B (leveraging Strategy pattern from Task 1.7)
    public class PaymentSystem
    {
        public void ProcessPayment(decimal amount, string method, string detail)
        {
            Logger.Instance.Log($"[PaymentSubsystem] Setting up payment of {amount:C} using: {method}");
            
            // Choose payment strategy
            IPaymentStrategy strategy = method.ToLowerInvariant() switch
            {
                "upi" => new UpiPaymentStrategy(detail),
                "netbanking" => new NetBankingPaymentStrategy(detail),
                _ => new CreditCardPaymentStrategy(detail, "Valued Customer")
            };

            strategy.Pay(amount);
        }
    }

    // Subsystem C
    public class ShippingSystem
    {
        public void ScheduleShipment(int orderId, string address = "Default Customer Address")
        {
            Logger.Instance.Log($"[ShippingSubsystem] Scheduling delivery for Order ID: {orderId} to: '{address}'");
        }
    }

    /// <summary>
    /// Facade providing a simplified interface over Inventory, Payment, and Shipping subsystems.
    /// </summary>
    public class OrderFacade
    {
        private readonly InventorySystem _inventory;
        private readonly PaymentSystem _payment;
        private readonly ShippingSystem _shipping;

        public OrderFacade()
        {
            _inventory = new InventorySystem();
            _payment = new PaymentSystem();
            _shipping = new ShippingSystem();
        }

        /// <summary>
        /// Single clean entry method coordinating all underlying complex subsystem interactions.
        /// </summary>
        public bool PlaceOrder(int orderId, int itemId, int quantity, decimal amount, string paymentMethod, string paymentDetail)
        {
            Logger.Instance.Log($"=== [OrderFacade] Starting Order Placement: Order #{orderId} ===");

            // Step 1: Check inventory
            if (!_inventory.CheckStock(itemId, quantity))
            {
                Logger.Instance.Log($"[OrderFacade] Order placement failed: Item ID {itemId} is out of stock.");
                return false;
            }

            // Step 2: Deduct stock
            _inventory.DeductStock(itemId, quantity);

            // Step 3: Process payment
            _payment.ProcessPayment(amount, paymentMethod, paymentDetail);

            // Step 4: Schedule shipment
            _shipping.ScheduleShipment(orderId);

            Logger.Instance.Log($"=== [OrderFacade] Order #{orderId} successfully processed! ===");
            return true;
        }
    }

    #endregion
}
