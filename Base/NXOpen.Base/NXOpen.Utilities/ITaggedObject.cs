namespace NXOpen.Utilities;

public interface ITaggedObject
{
    /// <summary>
    /// Gets the tag of the object.
    /// </summary>
    Tag Tag { get; }

    internal void SetTag(Tag tag);

    void initialize();

}