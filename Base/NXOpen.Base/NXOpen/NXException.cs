using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NXOpen.Utilities;

namespace NXOpen;

/// <summary>
/// A base class for exceptions thrown by NXOpen methods
/// </summary>
[Serializable]
public class NXException : ApplicationException
{
	private const int ErrorBase = 3520000;

	private const int ErrorUnexpectedSuccess = 3520057;

	private int m_status;

	private int m_undo_mark;

	private string m_message;

	/// <summary>
	/// Gets the error code associated with this exception.
	/// </summary>
	public int ErrorCode => m_status;

	/// <summary>
	/// Gets the error message associated with this exception.
	/// </summary>
	public override string Message
	{
		get
		{
			if (m_message == null)
			{
				initMessage();
			}
			return m_message;
		}
	}

	/// <summary>
	/// Gets the undo mark associated with this exception.
	/// </summary>
	public int UndoMark => m_undo_mark;

	private NXException(int status, int undo_mark)
		: base("NX error status: " + status)
	{
		m_status = status;
		m_undo_mark = undo_mark;
	}

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	private static extern int JAM_get_exception_undo_mark();

	/// <summary>
	/// Creates an NXException.
	/// </summary>
	public static NXException Create(int status)
	{
		int undo_mark = JAM_get_exception_undo_mark();
		return new NXException(status, undo_mark);
	}

	public static NXException CreateWithoutUndoMark(int status)
	{
		return new NXException(status, 0);
	}

	[DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	private static extern void JAM_decode_error(int code, out IntPtr s);

	private void initMessage()
	{
		JAM_decode_error(m_status, out var s);
		if (s == IntPtr.Zero)
		{
			m_message = base.Message;
		}
		else
		{
			m_message = JAM.ToStringFromLocale(s, doFree: false);
		}
	}

	internal static void ThrowBadTagException()
	{
		throw new Exception("Attempt to use deleted object");
	}

	/// <summary>
	/// Assert if the error code is unexpected.
	/// </summary>
	public void AssertErrorCode(int status)
	{
		if (m_status != status)
		{
			throw new ApplicationException("Unexpected error code: " + status, this);
		}
	}

	public static void ThrowUnexpectedSuccess()
	{
		throw CreateWithoutUndoMark(3520057);
	}

    [Obsolete]
	private NXException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		m_status = info.GetInt32("m_status");
		m_undo_mark = info.GetInt32("m_undo_mark");
		m_message = info.GetString("m_message");
	}

    [Obsolete]
    public override void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		initMessage();
		si.AddValue("m_status", m_status);
		si.AddValue("m_undo_mark", m_undo_mark);
		si.AddValue("m_message", m_message);
		base.GetObjectData(si, context);
	}
}
