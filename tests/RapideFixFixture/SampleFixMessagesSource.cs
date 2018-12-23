using System.Collections.Generic;

namespace RapideFixFixture
{
  public class SampleFixMessagesSource
  {
    public const string Sample0 = "35=A|55=TestTag55|62=35|67=56.123|";
    public const string Sample1 = "35=A|62=35|67=56.123|68=Y";

    public static IEnumerable<object[]> GetSampleFixBodies()
    {
      yield return new[] { TestFixMessageBuilder.DefaultBody };
      yield return new[] { "35=8|49=PHLX|20=3|167=CS|54=1|38=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|" };
      yield return new[] { "35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|" };
    }

    public static IEnumerable<object[]> GetTestTypeParentMessageBodies()
    {
      yield return new[] { Sample0 };
      yield return new[] { "35=A|55=TestTag55|56=2|57=Enumerable1|57=Enumerable2|62=35|67=56.123|" };
      yield return new[] { "35=A|55=TestTag55|58=ChildProperty|" };
      yield return new[] { "35=A|55=TestTag55|59=2|60=35|601=36|60=37|" };
      yield return new[] { "35=A|55=TestTag55|61=35|" };
      yield return new[] { "35=A|55=TestTag55|64=2|65=1|65=2|" };
      yield return new[] { Sample1 };
    }

  }
}
