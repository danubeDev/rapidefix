namespace RapideFixFixture
{
  /// <summary>
  /// Represents a fix tag and related value in encoded bytes.
  /// </summary>
  internal interface IFixTagValue
  {
    /// <summary>
    /// Copies the values of the tag to the data byte array to the given offset of data.
    /// Returns the new offset.
    /// </summary>
    /// <returns>The new offset.</returns>
    int CopyTo(byte[] data, int offset);

    /// <summary>
    /// Returns the encoded length of data in bytes.
    /// </summary>
    /// <returns>Number of bytes.</returns>
    int GetLength();

    /// <summary>
    /// Displays the fix tag in human readable form.
    /// </summary>
    string ToString();

    /// <summary>
    /// Returns the fix tag and value in encoded bytes.
    /// </summary>
    byte[] ToBytes();
  }
}
