using RapideFix.Business.Data;
using RapideFix.DataTypes;

namespace RapideFix.Business
{
  /// <summary>
  /// Sets an object's parent objects.
  /// </summary>
  public interface IParentSetter
  {
    /// <summary>
    /// Sets the parent object of a child property, before the child can be set, or returns the child object if already set
    /// </summary>
    /// <param name="leaf">Representation of the child property</param>
    /// <param name="parent">Representation of the property to be set</param>
    /// <param name="fixMessageContext">Message context</param>
    /// <param name="targetObject">Parent object, whose child is created</param>
    /// <returns>Returns the newly created child or an existing one</returns>
    object Set(TagMapLeaf leaf, TagMapParent parent, FixMessageContext fixMessageContext, object targetObject);
  }
}