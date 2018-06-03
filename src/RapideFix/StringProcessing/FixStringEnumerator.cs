using System;
using System.Collections;
using System.Collections.Generic;

namespace RapideFix.StringProcessing
{
  public sealed class FixStringEnumerator : IEnumerator<ReadOnlyMemory<char>>
  {
    #region Declarations

    private readonly string _fixString;

    private int _index = 0;

    #endregion

    #region Constructors

    public FixStringEnumerator(string fixString)
    {
      _fixString = fixString ?? throw new ArgumentNullException(nameof(fixString));
    }

    #endregion

    #region IEnumerator Properties

    public ReadOnlyMemory<char> Current { get; private set; }

    object IEnumerator.Current => Current;

    #endregion

    #region IEnumerator Methods

    public void Dispose() { }

    public bool MoveNext()
    {
      if (_index < _fixString.Length)
      {
        int nextSeparator = _fixString.IndexOf(Constants.VerticalBar, _index);
        if (0 < nextSeparator)
        {
          Current = _fixString.AsMemory(_index, nextSeparator - _index);
          _index = nextSeparator + 1;
          return true;
        }
      }

      return false;
    }

    public void Reset()
    {
      _index = 0;
      Current = null;
    }

    #endregion
  }
}
