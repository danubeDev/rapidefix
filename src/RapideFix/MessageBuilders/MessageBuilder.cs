using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapideFix.DataTypes;

namespace RapideFix.MessageBuilders
{
  public class MessageBuilder
  {
    protected readonly FixTagValueConverter _converter;
    private MessageEncoding _encoding;
    protected SupportedFixVersion _version;
    private byte[] _builderArray;
    protected int _currentLength;
    private int _availableLength;

    public MessageBuilder()
    {
      _version = SupportedFixVersion.Fix44;
      _availableLength = 256;
      _currentLength = 0;
      _converter = new FixTagValueConverter();
      _builderArray = ArrayPool<byte>.Shared.Rent(_availableLength);
    }

    public MessageBuilder AddTag(int tag, string value)
    {
      int expectedLength = (int)Math.Floor(Math.Log10(tag) + 1) + value.Length + 2;
      AddTag(tag, value, expectedLength);
      return this;
    }

    public MessageBuilder AddRaw(string value)
    {
      int expectedLength = Encoding.ASCII.GetByteCount(value);
      byte[] currentArray = ArrayPool<byte>.Shared.Rent(expectedLength);
      int offset = _converter.Get(value, currentArray);
      Concat(currentArray.AsSpan(0, offset));
      ArrayPool<byte>.Shared.Return(currentArray, true);
      return this;
    }

    public MessageBuilder AddTag(int tag, string value, MessageEncoding encoding)
    {
      if(_encoding == null)
      {
        _encoding = encoding;
      }
      else if(_encoding != encoding)
      {
        throw new InvalidOperationException("Encoding is already set");
      }

      int expectedLength = (int)Math.Floor(Math.Log10(tag) + 1) + encoding.GetEncoder().GetByteCount(value) + 2;
      AddTag(tag, value, expectedLength);
      return this;
    }

    public MessageBuilder AddTag(string tagAndValue)
    {
      if(!tagAndValue.Contains(Constants.Equal))
      {
        throw new ArgumentException($"Missing '{Constants.Equal}' on tag and value");
      }
      if(tagAndValue.Last() != Constants.VerticalBar)
      {
        throw new ArgumentException("Missing SOH char");
      }
      AddRaw(tagAndValue);
      return this;
    }

    public MessageBuilder AddBeginString(SupportedFixVersion version)
    {
      _version = version;
      return this;
    }

    private MessageBuilder AddTag(int tag, string value, int expectedLength)
    {
      byte[] currentArray = ArrayPool<byte>.Shared.Rent(expectedLength);
      int offset = _converter.Get(tag, value, currentArray);
      Concat(currentArray.AsSpan(0, offset));
      ArrayPool<byte>.Shared.Return(currentArray, true);
      return this;
    }

    private void Concat(Span<byte> current)
    {
      if((_availableLength - _currentLength) < current.Length)
      {
        IncreaseSize(current.Length);
      }
      current.CopyTo(_builderArray.AsSpan(_currentLength));
      _currentLength += current.Length;
    }

    private void IncreaseSize(int minimum)
    {
      _availableLength = _availableLength < minimum ? _availableLength + minimum : _availableLength * 2;
      var newBuilderArray = ArrayPool<byte>.Shared.Rent(_availableLength);
      _builderArray.AsSpan(0, _currentLength).CopyTo(newBuilderArray);
      ArrayPool<byte>.Shared.Return(_builderArray, true);
      _availableLength = newBuilderArray.Length;
      _builderArray = newBuilderArray;
    }

    public byte[] Build()
    {
      int expectedLength = CalculateRequiredSize();
      byte[] result = new byte[expectedLength];
      Build(result);
      return result;
    }

    public int Build(Span<byte> into)
    {
      int offset = AddVersion(into);
      offset += AddLength(into, offset);
      _builderArray.AsSpan(0, _currentLength).CopyTo(into.Slice(offset));
      offset += _currentLength;
      offset += AddChecksum(into, offset);
      Clear();
      return offset;
    }

    protected virtual void Clear()
    {
      _currentLength = 0;
      ArrayPool<byte>.Shared.Return(_builderArray);
      _builderArray = ArrayPool<byte>.Shared.Rent(_availableLength);
    }

    protected virtual int CalculateRequiredSize()
    {
      return _currentLength +
        KnownFixTags.FixVersion.Length + _version.Value.Length + 1 +
        KnownFixTags.Length.Length + (int)Math.Floor(Math.Log10(_currentLength) + 1) +
        KnownFixTags.Checksum.Length + 3;
    }

    protected virtual int AddChecksum(Span<byte> into, int offset)
    {
      byte checksumValue = 0;
      for(int i = 0; i < offset; i++)
      {
        checksumValue += into[i];
      }
      return _converter.Get(10, (int)checksumValue, into.Slice(offset), 3);
    }

    protected virtual int AddLength(Span<byte> into, int offset)
    {
      return _converter.Get(9, _currentLength, into.Slice(offset));
    }

    protected virtual int AddVersion(Span<byte> into)
    {
      KnownFixTags.FixVersion.CopyTo(into);
      int offset = KnownFixTags.FixVersion.Length;
      _version.Value.CopyTo(into.Slice(offset));
      offset += _version.Value.Length;
      into[offset] = Constants.SOHByte;
      return offset + 1;
    }

  }

}
