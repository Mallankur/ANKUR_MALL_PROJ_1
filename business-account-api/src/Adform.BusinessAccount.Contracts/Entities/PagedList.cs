using System.Collections.Generic;

namespace Adform.BusinessAccount.Contracts.Entities
{
    public class PagedList<T>
    {
        public List<T> Content { get; set; }
        public Page Page { get; set; }
        public Order Order { get; set; }
        public long? TotalCount { get; set; }
    }

}