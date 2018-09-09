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
      TagMapLeaf result = uut.TryGet(58.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result is IEnumerableTag);
      Assert.NotNull(result.Current);
      Assert.Equal(1, result.Parents.Count);
      Assert.False(result.Parents.First() is IEnumerableTag);
    }

    [Fact]
    public void GivenSimpleTag_Map_ReturnsTagWithNoParents()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var result = uut.TryGet(55.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result is IEnumerableTag);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parents);
    }

    [Fact]
    public void GivenTagWithConverter_Map_ReturnsAndTypeConverterName()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var result = uut.TryGet(61.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result is IEnumerableTag);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parents);
      Assert.NotNull(result.TypeConverterName);
    }

    [Fact]
    public void GivenRepeatingSimpleType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var repeatingTag = uut.TryGet(56.ToKnownTag());
      var actualTag = uut.TryGet(57.ToKnownTag());

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag is IEnumerableTag);
      Assert.True(repeatingTag is RepeatingGroupTagMapLeaf);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.True(actualTag is IEnumerableTag);
      Assert.NotNull(actualTag.Current);
      Assert.Empty(actualTag.Parents);
      Assert.NotEqual(0, ((IEnumerableTag)actualTag).RepeatingTagNumber);
    }

    [Fact]
    public void GivenRepeatingComplexType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var repeatingTag = uut.TryGet(59.ToKnownTag());
      var actualTag = uut.TryGet(60.ToKnownTag());

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag is IEnumerableTag);
      Assert.True(repeatingTag is RepeatingGroupTagMapLeaf);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.False(actualTag is IEnumerableTag);
      Assert.NotNull(actualTag.Current);
      Assert.Equal(1, actualTag.Parents.Count);
      Assert.True(actualTag.Parents.First() is IEnumerableTag);
      Assert.NotEqual(0, actualTag.Parents.OfType<IEnumerableTag>().First().RepeatingTagNumber);
    }

  }
}
