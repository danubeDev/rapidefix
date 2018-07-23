using System;

namespace RapideFix.Business.Data
{
  public interface IEnumerableTag
  {
    int RepeatingTagNumber { get; }

    Type InnerType { get; }
  }
}
