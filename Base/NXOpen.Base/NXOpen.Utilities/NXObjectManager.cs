using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NXOpen.Utilities;

/// <summary>
/// Maintains a mapping between Tags and objects.
/// </summary>
/// <remarks>This class can be used to help use methods in the NXOpen.UF namespace
/// together with objects in the NXOpen namespace.</remarks>
public class NXObjectManager
{

	public static TaggedObject GetObjectFromUInt(uint tagValue)
	{
		return Get((Tag)tagValue);
	}

	/// <summary>
	/// Returns the object corresponding to a tag. Use this method in
	/// a remoting application.
	/// </summary>
	/// <remarks>The object that is returned by this method is unique for given tag
	/// while that tag is alive. The Tag property of the object will return the
	/// tag with which it was obtained by this method.
	/// Methods in classes in the NXOpen.UF namespace may return tags and 
	/// GetTaggedObject can be used to obtain the corresponding object.
	/// </remarks>
	/// <param name="objectTag">Tag for which an object is wanted.</param>
	/// <returns>The object for this tag</returns>
	// public TaggedObject GetTaggedObject(Tag objectTag)
	// {
	// 	return Get(objectTag);
	// }

	/// <summary>
	/// Returns the object corresponding to a tag. 
	/// </summary>
	/// <remarks>The object that is returned by this method is unique for given tag
	/// while that tag is alive. The Tag property of the object will return the
	/// tag with which it was obtained by this method.
	/// Methods in classes in the NXOpen.UF namespace may return tags and Get
	/// can be used to obtain the corresponding object.
	/// </remarks>
	/// <param name="objectTag">Tag for which an object is wanted.</param>
	/// <returns>The object for this tag</returns>
	public static TaggedObject Get(Tag objectTag)
	{
		return Get(objectTag);
	}

	public static T Get<T>(Tag objectTag)where T : ITaggedObject,new ()
	{
		if (objectTag == Tag.Null)
		{
			return default;
		}
        T t = new();
		t.SetTag(objectTag);
		t.initialize();
		return t;
    }

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static T Get<T>(Tag objectTag,string className) where T : ITaggedObject, new()
    {
        return Get<T>(objectTag);
    }


    public static T Construct<T>(IntPtr arg) where T : TransientObject,new()
    {
		var obj= new T();
        obj.SetHandle(arg);
		obj.initialize();
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static T Construct<T>(string className,IntPtr arg) where T : TransientObject, new()
    {
		return Construct<T>(arg);
    }



}


public interface IGetProvider
{
	T Get<T>(Tag objectTag) where T : ITaggedObject;

	string GetRootPath();
}
public class NXObjectManagerEx
{
	private static IGetProvider getProvider;

	public static IGetProvider GetProvider => getProvider;

	public static void SetGetProvider(IGetProvider provider)
	{
		getProvider = provider;
	}

	public static void ClearGetProvider()
	{
		getProvider = null;
	}

    public static T Get<T>(Tag objectTag) where T : ITaggedObject
    {
        if (objectTag == Tag.Null)
        {
            return default;
        }
		if(getProvider!=null)
		{
			var t = getProvider.Get<T>(objectTag);
            t.SetTag(objectTag);
            t.initialize();
        }
        throw new NotImplementedException();
    }
}
