using System.Linq;
using RapideFix.Attributes;
using RapideFix.Extensions;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Attributes
{
  public partial class TagToPropertyMapperTests
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
      var result = uut.Get(58.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result.IsRepeating);
      Assert.NotNull(result.Current);
      Assert.Equal(1, result.Parent.Count);
      Assert.False(result.Parent.First().IsRepeating);
      Assert.Null(result.Parent.First().RepeatingTag);
    }

    [Fact]
    public void GivenSimpleTag_Map_ReturnsTagWithNoParents()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var result = uut.Get(55.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result.IsRepeating);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parent);
    }

    [Fact]
    public void GivenTagWithConverter_Map_ReturnsAndTypeConverterName()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var result = uut.Get(61.ToKnownTag());

      Assert.False(result.IsEncoded);
      Assert.False(result.IsRepeating);
      Assert.NotNull(result.Current);
      Assert.Empty(result.Parent);
      Assert.NotNull(result.TypeConverterName);
    }

    [Fact]
    public void GivenRepeatingSimpleType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var repeatingTag = uut.Get(56.ToKnownTag());
      var actualTag = uut.Get(57.ToKnownTag());

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsRepeating);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parent);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.True(actualTag.IsRepeating);
      Assert.NotNull(actualTag.Current);
      Assert.Empty(actualTag.Parent);
      Assert.NotNull(actualTag.RepeatingTag);
    }

    [Fact]
    public void GivenRepeatingComplexType_Map_ReturnsRepeating()
    {
      var uut = new TagToPropertyMapper();
      uut.Map<TestTypeParent>();
      var repeatingTag = uut.Get(59.ToKnownTag());
      var actualTag = uut.Get(60.ToKnownTag());

      Assert.False(repeatingTag.IsEncoded);
      Assert.False(repeatingTag.IsRepeating);
      Assert.NotNull(repeatingTag.Current);
      Assert.Empty(repeatingTag.Parent);
      Assert.Null(repeatingTag.TypeConverterName);

      Assert.False(actualTag.IsEncoded);
      Assert.False(actualTag.IsRepeating);
      Assert.NotNull(actualTag.Current);
      Assert.Equal(1, actualTag.Parent.Count);
      Assert.True(actualTag.Parent.First().IsRepeating);
      Assert.NotNull(actualTag.Parent.First().RepeatingTag);
    }

  }
}
