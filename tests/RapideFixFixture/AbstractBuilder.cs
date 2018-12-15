using System;
using RapideFix.MessageBuilders;

namespace RapideFixFixture
{
  public class AbstractBuilder<TBuilder>
    where TBuilder : MessageBuilder
  {
    public TBuilder ActualBuilder { get; }

    public AbstractBuilder(TBuilder actualBuilder)
    {
      ActualBuilder = actualBuilder ?? throw new ArgumentNullException(nameof(actualBuilder));
    }

    public AbstractBuilder<TBuilder> Do(Action<TBuilder> action)
    {
      action(ActualBuilder);
      return this;
    }
  }
}
