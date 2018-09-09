using System;
using RapideFix.Business;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class ObjectPoolTests
  {
    [Fact]
    public void GivenNoDependencies_Construct_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new ObjectPool<TestChild>(null, o => o.ToString()));
      Assert.Throws<ArgumentNullException>(() => new ObjectPool<TestChild>(() => new TestChild(), null));
    }

    [Fact]
    public void GivenDependencies_Construct_DoesNotThrow()
    {
      var ex = Record.Exception(() => new ObjectPool<TestChild>(() => new TestChild(), o => o.ToString()));
      Assert.Null(ex);
    }

    [Fact]
    public void GivenEmptyPool_Rent_CreatesNewObject()
    {
      bool created = false;
      var uut = new ObjectPool<TestChild>(() => { created = true; return new TestChild(); }, o => o.ToString());
      Assert.NotNull(uut.Rent());
      Assert.True(created);
    }

    [Fact]
    public void GivenItem_Return_CleansUpObject()
    {
      bool cleanedUp = false;
      var uut = new ObjectPool<TestChild>(() => new TestChild(), o => { cleanedUp = true; });
      using(uut.Rent())
      { }
      Assert.True(cleanedUp);
    }

    [Fact]
    public void GivenReturnedItem_Rent_DoesNotCreateNewItem()
    {
      int createdNumber = 0;
      var uut = new ObjectPool<TestChild>(() => { createdNumber++; return new TestChild(); }, o => o.ToString());
      using(uut.Rent())
      { }
      using(uut.Rent())
      { }
      Assert.Equal(1, createdNumber);
    }
  }
}
