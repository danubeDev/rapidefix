using System;
using System.Linq;
using System.Text;
using RapideFix;
using RapideFix.DataTypes;
using RapideFix.MessageBuilders;
using Xunit;

namespace RapideFixFixture.MessageBuilders
{
  public class MessageBuilderTests
  {
    [Fact]
    public void Call_Constructor_DoesNothThrow()
    {
      var ex = Record.Exception(() => new MessageBuilder());
      Assert.Null(ex);
    }

    [Fact]
    public void GivenAddTag_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddTag(22, "test").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=8|22=test|10=050|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenTwoAddTag_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddTag(22, "test").AddTag(23, "test2").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=17|22=test|23=test2|10=247|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenBeginString_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddBeginString(SupportedFixVersion.Fix42).AddTag(22, "test").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.2|9=8|22=test|10=048|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenStringTag_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddTag("22=test|").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=8|22=test|10=050|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenStringNoEquals_Build_ThrowsArgumentException()
    {
      var uut = new MessageBuilder();
      Assert.Throws<ArgumentException>(() => uut.AddTag("22test|").Build());
    }

    [Fact]
    public void GivenStringNoSOH_Build_ThrowsArgumentException()
    {
      var uut = new MessageBuilder();
      Assert.Throws<ArgumentException>(() => uut.AddTag("22=test").Build());
    }

    [Fact]
    public void GivenTwoAddTagAddRaw_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddRaw("22=test|23=test2|").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=17|22=test|23=test2|10=247|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenEncodedTag_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddTag(22, "tést", MessageEncoding.UTF8).Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=9|22=", Encoding.UTF8.GetBytes("tést"), "|10=058|").SequenceEqual(bytes));
    }

    [Fact]
    public void GivenInvalidTwoAddTagAddRaw_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      string raw = "35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|";
      var bytes = uut.AddRaw(raw).Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage($"8=FIX.4.4|9=356|{raw}10=202|").SequenceEqual(bytes));
    }

    [Fact]
    public void ReuseMessageBuilderAddTag_Build_CreatesMessageWithTag()
    {
      var uut = new MessageBuilder();
      var bytes = uut.AddTag(21, "test3").Build();
      bytes = uut.AddTag(22, "test").Build();
      Assert.True(ConverterTestHelper.GetEncodedMessage("8=FIX.4.4|9=8|22=test|10=050|").SequenceEqual(bytes));
    }
  }
}
