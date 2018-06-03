using System.Collections.Generic;

namespace RapideFixFixture
{
  public class SampleFixMessagesSource
  {
    public static IEnumerable<object[]> GetSampleFixBodies()
    {
      yield return new[] { TestFixMessageBuilder.DefaultBody };
      yield return new[] { "35=8|49=PHLX|20=3|167=CS|54=1|38=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|" };
      yield return new[] { "35=8|49=PHLX|56=PERS|52=20071123-05:30:00.000|11=ATOMNOCCC9990900|20=3|150=E|39=E|55=MSFT|167=CS|54=1|38=15|40=2|44=15|58=PHLX EQUITY TESTING|59=0|47=C|32=0|31=0|151=15|14=0|6=0|" };
    }
  }
}
