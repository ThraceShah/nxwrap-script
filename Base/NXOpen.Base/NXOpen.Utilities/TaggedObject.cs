using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NXOpen.Utilities;

namespace NXOpen;

/// <summary>
/// An object which has a Tag.
/// </summary>
public class TaggedObject:ITaggedObject
{
	private const int ERROR_JAM_base = 3615000;

	private const int JAM_ERR_OBJECT_NOT_ALIVE = 3615013;

	private Tag m_tag;

	/// <summary>
	/// Returns the tag of this object.
	/// </summary>
	/// <remarks>The tag of the object is an identifier for the object.
	/// It is used when calling methods of
	/// classes in the NXOpen.UF namespace.
	/// </remarks>
	public Tag Tag
	{
		get
		{
			if (m_tag == Tag.Null)
			{
				throw NXException.CreateWithoutUndoMark(3615013);
			}
			return m_tag;
		}
	}

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_clr_test_print_tag_of_class")]
	private static extern int JAM_test_print_tag_of_class(int tag, string className, string varName, int lineNumber);

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	private static extern IntPtr JAM_lookup_tag(uint tag);

	internal void SetTag(Tag tag)
	{
		m_tag = tag;
	}

	void ITaggedObject.SetTag(NXOpen.Tag tag)
	{
        m_tag = tag;
	}

	public virtual void initialize()
	{
		
	}

	/// <summary>
	/// Returns a String that represents the current Object. 
	/// </summary>
	/// <returns>A String that represents the current Object. </returns>
	public override string ToString()
	{
		return GetType().Name + " " + Tag;
	}

	/// <summary>
	/// This is an internal method for testing purposes that should not be called.
	/// </summary>
	/// <param name="variableName"></param>
	public void PrintTestData(string variableName)
	{
		string className = GetType().ToString();
		StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
		StackFrame frame = stackTrace.GetFrame(1);
		int num = frame.GetFileLineNumber();
		if (num == 0)
		{
			num = -1;
		}
		int num2 = JAM_test_print_tag_of_class((int)Tag, className, variableName, num);
		if (num2 != 0)
		{
			throw NXException.Create(num2);
		}
	}
}
