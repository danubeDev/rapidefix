using System;
using System.Collections;
using System.Collections.Generic;

namespace RapideFix.StringProcessing
{
  public class FixStringEnumerable : IEnumerable<ReadOnlyMemory<char>>
  {
    private readonly string _fixString;

    public FixStringEnumerable(string fixString)
    {
      _fixString = fixString;
    }

    public IEnumerator<ReadOnlyMemory<char>> GetEnumerator() => new FixStringEnumerator(_fixString);

    IEnumerator IEnumerable.GetEnumerator() => new FixStringEnumerator(_fixString);
  }
}
