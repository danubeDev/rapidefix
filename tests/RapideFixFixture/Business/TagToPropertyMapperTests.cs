using System.Linq;
using RapideFix.Business;
using RapideFix.Business.Data;
using RapideFix.Extensions;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Business
{
  public class TagToPropertyMapperTests
  {
    [Fact]
    public void GivenType_Map_DoesNotThrow()
    {
      var uut = new TagToPropertyMapper();
      var exception = Record.Exception(() => uut.Map<TestTypeParent>());
      Assert.Null(exception);
    }

    [Fact]
    public void GivenComplexChildType_Map_ReturnsChildsTag()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      uut.TryGet(58.ToKnownTag(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Equal(1, result.Parents.Count);
      Assert.False(result.Parents.First().IsEnumerable);
    }

    [Fact]
    public void GivenSimpleTag_Map_ReturnsTagWithNoParents()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      uut.TryGet(55.ToKnownTag(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parents);
    }

    [Fact]
    public void GivenTagWithConverter_Map_ReturnsAndTypeConverterName()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      uut.TryGet(61.ToKnownTag(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parents);
      Assert.NotNull(result.TypeConverterName);
    }

    [Fact]
    public void GivenRepeatingSimpleType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      uut.TryGet(56.ToKnownTag(), out TagMapLeaf repeatingTag);
      uut.TryGet(57.ToKnownTag(), out TagMapLeaf actualTag);

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsEnumerable);
      Assert.True(repeatingTag.IsRepeatingGroupTag);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.True(actualTag.IsEnumerable);
      Assert.NotNull(actualTag.Current);
      Assert.Empty(actualTag.Parents);
      Assert.NotEqual(0, actualTag.RepeatingTagNumber);
    }

    [Fact]
    public void GivenRepeatingComplexType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      uut.TryGet(59.ToKnownTag(), out TagMapLeaf repeatingTag);
      uut.TryGet(60.ToKnownTag(), out TagMapLeaf actualTag);

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsEnumerable);
      Assert.True(repeatingTag.IsRepeatingGroupTag);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.False(actualTag.IsEnumerable);
      Assert.NotNull(actualTag.Current);
      Assert.Equal(1, actualTag.Parents.Count);
      Assert.True(actualTag.Parents.First().IsEnumerable);
      Assert.NotEqual(0, actualTag.Parents.First(x => x.IsEnumerable).RepeatingTagNumber);
    }

  }
}
