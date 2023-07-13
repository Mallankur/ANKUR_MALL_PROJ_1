using Xunit;

namespace Adform.BusinessAccount.Acceptance.Test;

[CollectionDefinition(BusinessAccountWebApplicationCollection.COLLECTION_NAME)]
public class BusinessAccountWebApplicationCollection : ICollectionFixture<BusinessAccountWebApplicationFixture>
{
    internal const string COLLECTION_NAME = "BusinessAccountCollection";
}