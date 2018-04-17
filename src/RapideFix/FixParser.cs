using System;
using System.IO;

namespace RapideFix
{
  public class FixParser
  {
    private readonly FixParserSettings _settings;

    public FixParser(FixParserSettings settings)
    {
      _settings = settings;
    }

    public TMessage Parse<TMessage>(string data)
    {
      throw new NotImplementedException();
    }

    public TMessage Parse<TMessage>(byte[] data)
    {
      throw new NotImplementedException();
    }

    public TMessage Parse<TMessage>(Stream data)
    {
      throw new NotImplementedException();
    }

  }
}
