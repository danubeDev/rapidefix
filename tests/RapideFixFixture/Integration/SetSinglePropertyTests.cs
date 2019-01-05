using System;
using System.Linq;
using System.Text;
using RapideFix;
using RapideFix.Business;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Integration
{
  public class SetSinglePropertyTestsFixture : IDisposable
  {
    public SetSinglePropertyTestsFixture()
    {
      PropertyMapper = new TagToPropertyMapper(new SubPropertySetterFactory());
      PropertyMapper.Map<TestTypeParent>();
      CompositeSetter = new CompositePropertySetter();
    }

    public CompositePropertySetter CompositeSetter { get; }

    public TagToPropertyMapper PropertyMapper { get; }

    public void Dispose()
    {
    }
  }

  public class SetSinglePropertyTests : IClassFixture<SetSinglePropertyTestsFixture>
  {
    private readonly SetSinglePropertyTestsFixture _fixture;
    public SetSinglePropertyTests(SetSinglePropertyTestsFixture fixture)
    {
      _fixture = fixture;
      TargetObject = new TestTypeParent();
      MessageContext = new FixMessageContext();
    }

    public TestTypeParent TargetObject { get; }

    public FixMessageContext MessageContext { get; }

    [Fact]
    public void MapAndSetInteger()
    {
      int testValue = 123;
      _fixture.PropertyMapper.TryGet(63.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue),
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.Tag63);
    }

    [Fact]
    public void MapAndSetString()
    {
      string testValue = "TraderEntity";
      _fixture.PropertyMapper.TryGet(55.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var leaf);
      _fixture.CompositeSetter.Set(
              Convert(testValue),
              leaf,
              MessageContext,
              TargetObject);

      Assert.Equal(testValue, TargetObject.Tag55);
    }

    [Fact]
    public void MapAndSetDouble()
    {
      double testValue = 23.1;
      _fixture.PropertyMapper.TryGet(67.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue.ToString()),
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.Tag67);
    }

    [Fact]
    public void MapAndSetChildTypeProperty()
    {
      string testValue = "VenueName";
      _fixture.PropertyMapper.TryGet(58.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue),
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.CustomType.Tag58);
    }

    [Fact]
    public void MapAndSetRepeatingGroup()
    {
      _fixture.PropertyMapper.TryGet(56.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var leaf);
      _fixture.CompositeSetter.Set(
        Convert(3),
        leaf,
        MessageContext,
        TargetObject);

      Assert.NotNull(TargetObject.Tag57s);
    }

    [Fact]
    public void MapAndSetEnumeratedString()
    {
      string testValue0 = "TraderComment0";
      string testValue1 = "TraderComment1";
      _fixture.PropertyMapper.TryGet(56.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var repeatingGroupLeaf);
      _fixture.CompositeSetter.Set(
        Convert(3),
        repeatingGroupLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(57.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue0),
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(57.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue1),
        testTag1Leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue0, TargetObject.Tag57s.ElementAt(0));
      Assert.Equal(testValue1, TargetObject.Tag57s.ElementAt(1));
    }

    [Fact]
    public void MapAndSetEnumeratedChildTypeProperty()
    {
      string testValue0 = "TraderComment0";
      string testValue1 = "TraderComment1";
      _fixture.PropertyMapper.TryGet(59.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        Convert(2),
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(60.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue0),
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(60.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue1),
        testTag1Leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue0, TargetObject.Tag59s.ElementAt(0).Tag60);
      Assert.Equal(testValue1, TargetObject.Tag59s.ElementAt(1).Tag60);
    }

    [Fact]
    public void MapAndSetAllEnumeratedChildTypeProperty()
    {
      string testValue00 = "TraderComment00";
      string testValue01 = "TraderComment01";
      string testValue10 = "TraderComment10";
      string testValue11 = "TraderComment11";
      _fixture.PropertyMapper.TryGet(59.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        Convert(2),
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(60.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag00Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue00),
        testTag00Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(601.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag01Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue01),
        testTag01Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(60.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag10Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue10),
        testTag10Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(601.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag11Leaf);
      _fixture.CompositeSetter.Set(
              Convert(testValue11),
              testTag11Leaf,
              MessageContext,
              TargetObject);

      Assert.Equal(testValue00, TargetObject.Tag59s.ElementAt(0).Tag60);
      Assert.Equal(testValue01, TargetObject.Tag59s.ElementAt(0).Tag601);
      Assert.Equal(testValue10, TargetObject.Tag59s.ElementAt(1).Tag60);
      Assert.Equal(testValue11, TargetObject.Tag59s.ElementAt(1).Tag601);
    }

    [Fact]
    public void MapAndSetEnumeratedTypeConvertedProperty()
    {
      int testValue0 = 6353;
      int testValue1 = 1132;
      _fixture.PropertyMapper.TryGet(64.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        Convert(2),
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(65.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue0),
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(65.ToKnownTag(), typeof(TestTypeParent).GetHashCode(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        Convert(testValue1),
        testTag1Leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue0, TargetObject.Tag65s.ElementAt(0).Value);
      Assert.Equal(testValue1, TargetObject.Tag65s.ElementAt(1).Value);
    }

    public byte[] Convert(string value)
    {
      return Encoding.ASCII.GetBytes(value);
    }

    public byte[] Convert(int value)
    {
      int digitsCount = (int)Math.Floor(Math.Log10(value) + 1);
      var encodedData = new byte[digitsCount];
      IntegerToFixConverter.Instance.Convert(value, into: encodedData, count: digitsCount);
      return encodedData;
    }
  }
}
