namespace Adform.BusinessAccount.Contracts.Entities
{
    public class Page
    {
        private int MaxLimit { get; }

        public Page() : this(100, 100)
        {
        }

        protected Page(int limit, int maxLimit)
        {
            ReturnTotalCount = false;
            Offset = 0;
            Limit = limit;
            MaxLimit = maxLimit;
        }

        /// <summary>
        /// Flag if total count should be returned
        /// </summary>
        public bool ReturnTotalCount { get; set; }

        /// <summary>
        /// How many items should be in a page
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// How many items should be skipped
        /// </summary>
        public int Offset { get; set; }
    }

}