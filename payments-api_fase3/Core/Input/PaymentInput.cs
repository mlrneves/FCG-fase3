namespace Core.Input
{
    public class PaymentInput
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}