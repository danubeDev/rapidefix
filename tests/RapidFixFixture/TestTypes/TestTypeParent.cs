﻿using System.Collections.Generic;
using System.ComponentModel;
using RapideFix.Attributes;

namespace RapideFixFixture.TestTypes
{
  public class TestTypeParent
  {
    [FixTag(55)]
    public string Tag55 { get; set; }

    [RepeatingGroup(56)]
    [FixTag(57)]
    public IEnumerable<string> Tag57s { get; set; }

    public TestChild CustomType { get; set; }

    [RepeatingGroup(59)]
    public IEnumerable<TestMany> Tag59s { get; set; }

    [FixTag(61)]
    [TypeConverter(typeof(TestConverter))]
    public TestConvertable Tag61 { get; set; }
  }

}
