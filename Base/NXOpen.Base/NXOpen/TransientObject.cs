using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NXOpen.Utilities;

namespace NXOpen;

/// <summary>
/// A base class for all NXOpen transient objects
/// </summary>
public abstract class TransientObject :IDisposable
{
	internal IntPtr pointer;

	/// <summary>
	/// Handle of the internal object represented by this object.
	/// </summary>
	/// <remarks>This property should not be used by automation programs.</remarks>
	public IntPtr Handle => pointer;

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_sm_free")]
	private static extern void SM_free(IntPtr ptr);

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_clr_test_print_ptr_of_class")]
	private static extern int JAM_test_print_ptr_of_class(IntPtr ptr, string className, string varName, int lineNumber);

	public TransientObject(IntPtr ptr)
	{
		pointer = ptr;
	}

	internal void SetHandle(IntPtr ptr)
	{
		pointer = ptr;
	}

	public TransientObject()
	{
		pointer = IntPtr.Zero;
	}

	public virtual void initialize()
	{
	}


	/// <summary>
	/// Frees the object from memory.  
	/// </summary>
	/// <remarks>After this method is called, it is illegal to use the object.
	/// This method is automatically called when the object is 
	/// deleted by the garbage collector.  
	/// </remarks>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		DoFreeResource();
	}

	/// <summary>
	/// Frees the object from memory. 
	/// </summary>
	protected abstract void FreeResource();

	private void DoFreeResource()
	{
		if (pointer != IntPtr.Zero)
		{
			FreeResource();
			pointer = IntPtr.Zero;
		}
	}

	~TransientObject()
	{
		if (pointer != IntPtr.Zero)
		{
			DoFreeResource();
		}
	}

	/// <summary>
	/// Returns a String that represents the current Object.
	/// </summary>
	public new string ToString()
	{
		return GetType().Name + " " + pointer;
	}

	/// <exclude />
	public void PrintTestData(string variableName)
	{
		string className = GetType().ToString();
		StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
		StackFrame frame = stackTrace.GetFrame(1);
		int fileLineNumber = frame.GetFileLineNumber();
		int num = JAM_test_print_ptr_of_class(pointer, className, variableName, fileLineNumber);
		if (num != 0)
		{
			throw NXException.Create(num);
		}
	}
}
