using NUnit.Framework;

namespace Adform.BusinessAccount.Integration.Test;

using static Testing;

public abstract class TestFixtureBase
{
    [OneTimeSetUp]
    public void Init()
    {
        CreateTestMongoDb();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        DropTestMongoDb();
    }
}