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
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      var exception = Record.Exception(() => uut.Map<TestTypeParent>());
      Assert.Null(exception);
    }

    [Fact]
    public void GivenComplexChildType_Map_ReturnsChildsTag()
    {
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      uut.Map<TestTypeParent>();
      uut.TryGet(58.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Single(result.Parents);
      Assert.False(result.Parents.First().IsEnumerable);
    }

    [Fact]
    public void GivenSimpleTag_Map_ReturnsTagWithNoParents()
    {
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      uut.Map<TestTypeParent>();
      uut.TryGet(55.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Null(result.Parents);
    }

    [Fact]
    public void GivenTagWithConverter_Map_ReturnsAndTypeConverterName()
    {
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      uut.Map<TestTypeParent>();
      uut.TryGet(61.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf result);

      Assert.False(result.IsEncoded);
      Assert.False(result.IsEnumerable);
      Assert.NotNull(result.Current);
      Assert.Null(result.Parents);
      Assert.NotNull(result.TypeConverterName);
    }

    [Fact]
    public void GivenRepeatingSimpleType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      uut.Map<TestTypeParent>();
      uut.TryGet(56.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf repeatingTag);
      uut.TryGet(57.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf actualTag);

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsEnumerable);
      Assert.NotNull(repeatingTag.Current);
      Assert.Null(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.True(actualTag.IsEnumerable);
      Assert.NotNull(actualTag.Current);
      Assert.Null(actualTag.Parents);
      Assert.NotEqual(0, actualTag.RepeatingTagNumber);
    }

    [Fact]
    public void GivenRepeatingComplexType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper(new SubPropertySetterFactory());
      uut.Map<TestTypeParent>();
      uut.TryGet(59.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf repeatingTag);
      uut.TryGet(60.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out TagMapLeaf actualTag);

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsEnumerable);
      Assert.NotNull(repeatingTag.Current);
      Assert.Null(repeatingTag.Parents);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.False(actualTag.IsEnumerable);
      Assert.NotNull(actualTag.Current);
      Assert.Single(actualTag.Parents);
      Assert.True(actualTag.Parents.First().IsEnumerable);
      Assert.NotEqual(0, actualTag.Parents.First(x => x.IsEnumerable).RepeatingTagNumber);
    }

  }
}
