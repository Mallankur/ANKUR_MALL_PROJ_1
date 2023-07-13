namespace Adform.BusinessAccount.Contracts.Entities
{
    public class Order
    {
        /// <summary>
        /// Property name from a contract for sorting
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Sorting direction
        /// </summary>
        public OrderDirection OrderDirection { get; set; }
    }

}