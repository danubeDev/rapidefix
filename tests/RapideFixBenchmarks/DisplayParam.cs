namespace RapideFixBenchmarks
{
  public class DisplayParam
  {
    public DisplayParam(string message) => Value = message;

    public string Value { get; }

    public override string ToString()
    {
      return $"{Value.Substring(0, 3)} L:{Value.Length}";
    }
  }
}
