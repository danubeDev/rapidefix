using System;
using RapideFix.Business.Data;

namespace RapideFix.Business
{
  public interface ITagToPropertyMapper
  {
    /// <summary>
    /// Gets a tag setter descriptor or null.
    /// </summary>
    /// <param name="tag">The byte representation of the tag.</param>
    /// <returns>A tagmap leaf with optional parents.</returns>
    bool TryGet(ReadOnlySpan<byte> tag, out TagMapLeaf result);

    /// <summary>
    /// Gets the type to use for deserialization or null.
    /// </summary>
    /// <param name="value">Value of MessageType tag</param>
    /// <returns>Type object</returns>
    Type TryGetMessageType(ReadOnlySpan<byte> value);

    /// <summary>
    /// Maps a given type.
    /// </summary>
    void Map(Type type);

    /// <summary>
    /// Maps the generic type T.
    /// </summary>
    void Map<T>();
  }
}