using System;
using System.Linq;
using RapideFix.Business;
using RapideFix.DataTypes;
using RapideFix.Extensions;
using RapideFix.MessageBuilders;
using RapideFixFixture.TestTypes;
using Xunit;

namespace RapideFixFixture.Integration
{
  public class SetSinglePropertyTestsFixture : IDisposable
  {
    public SetSinglePropertyTestsFixture()
    {
      PropertyMapper = new TagToPropertyMapper();
      PropertyMapper.Map<TestTypeParent>();
      CompositeSetter = new CompositePropertySetter(new SubPropertySetterFactory());
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
      var testTag = new FixTagValue(63, testValue);

      _fixture.PropertyMapper.TryGet(testTag.Tag.ToKnownTag(), out var leaf);
      _fixture.CompositeSetter.Set(
        testTag.Value,
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.Tag63);
    }

    [Fact]
    public void MapAndSetString()
    {
      string testValue = "TraderEntity";
      var testTag = new FixTagValue(55, testValue);
      _fixture.PropertyMapper.TryGet(testTag.Tag.ToKnownTag(), out var leaf);
      _fixture.CompositeSetter.Set(
              testTag.Value,
              leaf,
              MessageContext,
              TargetObject);

      Assert.Equal(testValue, TargetObject.Tag55);
    }

    [Fact]
    public void MapAndSetDouble()
    {
      double testValue = 23.1;
      var testTag = new FixTagValue(67, testValue);
      _fixture.PropertyMapper.TryGet(testTag.Tag.ToKnownTag(), out var leaf);
      _fixture.CompositeSetter.Set(
        testTag.Value,
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.Tag67);
    }

    [Fact]
    public void MapAndSetChildTypeProperty()
    {
      string testValue = "VenueName";
      var testTag = new FixTagValue(58, testValue);
      _fixture.PropertyMapper.TryGet(testTag.Tag.ToKnownTag(), out var leaf);
      _fixture.CompositeSetter.Set(
        testTag.Value,
        leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue, TargetObject.CustomType.Tag58);
    }

    [Fact]
    public void MapAndSetRepeatingGroup()
    {
      var repeatingGroupTag = new FixTagValue(56, 3);
      _fixture.PropertyMapper.TryGet(repeatingGroupTag.Tag.ToKnownTag(), out var leaf);
      _fixture.CompositeSetter.Set(
        repeatingGroupTag.Value,
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
      var testTag0 = new FixTagValue(57, testValue0);
      var testTag1 = new FixTagValue(57, testValue1);
      var repeatingGroupTag = new FixTagValue(56, 3);
      _fixture.PropertyMapper.TryGet(repeatingGroupTag.Tag.ToKnownTag(), out var repeatingGroupLeaf);
      _fixture.CompositeSetter.Set(
        repeatingGroupTag.Value,
        repeatingGroupLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag0.Tag.ToKnownTag(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        testTag0.Value,
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag1.Tag.ToKnownTag(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        testTag1.Value,
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
      var testTag0 = new FixTagValue(60, testValue0);
      var testTag1 = new FixTagValue(60, testValue1);
      var repeatingGroupTag = new FixTagValue(59, 2);
      _fixture.PropertyMapper.TryGet(repeatingGroupTag.Tag.ToKnownTag(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        repeatingGroupTag.Value,
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag0.Tag.ToKnownTag(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        testTag0.Value,
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag1.Tag.ToKnownTag(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        testTag1.Value,
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
      var testTag00 = new FixTagValue(60, testValue00);
      var testTag01 = new FixTagValue(601, testValue01);
      var testTag10 = new FixTagValue(60, testValue10);
      var testTag11 = new FixTagValue(601, testValue11);
      var repeatingGroupTag = new FixTagValue(59, 2);
      _fixture.PropertyMapper.TryGet(repeatingGroupTag.Tag.ToKnownTag(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        repeatingGroupTag.Value,
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag00.Tag.ToKnownTag(), out var testTag00Leaf);
      _fixture.CompositeSetter.Set(
        testTag00.Value,
        testTag00Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag01.Tag.ToKnownTag(), out var testTag01Leaf);
      _fixture.CompositeSetter.Set(
        testTag01.Value,
        testTag01Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag10.Tag.ToKnownTag(), out var testTag10Leaf);
      _fixture.CompositeSetter.Set(
        testTag10.Value,
        testTag10Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag11.Tag.ToKnownTag(), out var testTag11Leaf);
      _fixture.CompositeSetter.Set(
              testTag11.Value,
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
      var testTag0 = new FixTagValue(65, testValue0);
      var testTag1 = new FixTagValue(65, testValue1);
      var repeatingGroupTag = new FixTagValue(64, 2);
      _fixture.PropertyMapper.TryGet(repeatingGroupTag.Tag.ToKnownTag(), out var repeatingGroupTagLeaf);
      _fixture.CompositeSetter.Set(
        repeatingGroupTag.Value,
        repeatingGroupTagLeaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag0.Tag.ToKnownTag(), out var testTag0Leaf);
      _fixture.CompositeSetter.Set(
        testTag0.Value,
        testTag0Leaf,
        MessageContext,
        TargetObject);

      _fixture.PropertyMapper.TryGet(testTag1.Tag.ToKnownTag(), out var testTag1Leaf);
      _fixture.CompositeSetter.Set(
        testTag1.Value,
        testTag1Leaf,
        MessageContext,
        TargetObject);

      Assert.Equal(testValue0, TargetObject.Tag65s.ElementAt(0).Value);
      Assert.Equal(testValue1, TargetObject.Tag65s.ElementAt(1).Value);
    }
  }
}
