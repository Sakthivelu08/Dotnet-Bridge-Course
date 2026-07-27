using System;
using BridgeCourse.Week1.Lab.Day2;

namespace BridgeCourse.Week1.Lab.Day3
{
    /// <summary>
    /// IPaymentStrategy interface defining the algorithm family.
    /// </summary>
    public interface IPaymentStrategy
    {
        void Pay(decimal amount);
    }

    /// <summary>
    /// Strategy implementation: Credit Card Payment.
    /// </summary>
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        private readonly string _cardNumber;
        private readonly string _cardHolderName;

        public CreditCardPaymentStrategy(string cardNumber, string cardHolderName)
        {
            _cardNumber = cardNumber;
            _cardHolderName = cardHolderName;
        }

        public void Pay(decimal amount)
        {
            // Reuse Logger Singleton from Day 2
            Logger.Instance.Log($"Processing Credit Card Payment of {amount:C} [Card: XXXX-XXXX-XXXX-{_cardNumber.Substring(Math.Max(0, _cardNumber.Length - 4))}, Holder: {_cardHolderName}]");
        }
    }

    /// <summary>
    /// Strategy implementation: UPI Payment.
    /// </summary>
    public class UpiPaymentStrategy : IPaymentStrategy
    {
        private readonly string _upiId;

        public UpiPaymentStrategy(string upiId)
        {
            _upiId = upiId;
        }

        public void Pay(decimal amount)
        {
            Logger.Instance.Log($"Processing UPI Payment of {amount:C} [VPA/UPI ID: {_upiId}]");
        }
    }

    /// <summary>
    /// Strategy implementation: NetBanking Payment.
    /// </summary>
    public class NetBankingPaymentStrategy : IPaymentStrategy
    {
        private readonly string _bankName;

        public NetBankingPaymentStrategy(string bankName)
        {
            _bankName = bankName;
        }

        public void Pay(decimal amount)
        {
            Logger.Instance.Log($"Processing NetBanking Payment of {amount:C} [Bank: {_bankName}]");
        }
    }

    /// <summary>
    /// Context class ShoppingCart which uses a dynamic IPaymentStrategy.
    /// </summary>
    public class ShoppingCart
    {
        private IPaymentStrategy _paymentStrategy;

        public decimal TotalAmount { get; set; }

        public ShoppingCart(decimal totalAmount, IPaymentStrategy paymentStrategy)
        {
            TotalAmount = totalAmount;
            _paymentStrategy = paymentStrategy;
        }

        /// <summary>
        /// Allows swapping strategies at runtime.
        /// </summary>
        public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
        {
            _paymentStrategy = paymentStrategy ?? throw new ArgumentNullException(nameof(paymentStrategy));
            Logger.Instance.Log($"Swapped payment strategy at runtime to: {paymentStrategy.GetType().Name}");
        }

        /// <summary>
        /// Processes checkout using the current payment strategy.
        /// </summary>
        public void Checkout()
        {
            if (TotalAmount <= 0)
            {
                throw new InvalidOperationException("Cannot checkout: Total amount must be greater than zero.");
            }

            Logger.Instance.Log($"Initiating checkout for total amount: {TotalAmount:C}");
            _paymentStrategy.Pay(TotalAmount);
        }
    }
}
