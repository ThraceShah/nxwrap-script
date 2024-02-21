#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NXOpen.Utilities;

/// <summary>
/// A base class for NXOpen session objects
/// </summary>
public partial class BaseSession:TaggedObject,IDisposable
{
    private bool disposedValue;

    [LibraryImport("libuginit")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_SESSION_initialize();

	[LibraryImport("libuginit")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_SESSION_finalize();

	[LibraryImport("libuginit")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_SESSION_terminate();

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_check_session_license([MarshalAs(UnmanagedType.LPStr)] string context);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_check_dotnet_author_license();

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_declare_alliance_context([MarshalAs(UnmanagedType.LPStr)] string context);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr JAM_get_filename([MarshalAs(UnmanagedType.LPStr)] string BaseDirectory, [MarshalAs(UnmanagedType.LPStr)] string FriendlyName);

	[LibraryImport("libpart", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAUTL_SESSION_set_test_output(string outputFile, int version);

	[LibraryImport("libpart")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAUTL_SESSION_close_test_output();

	[LibraryImport("libpart", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAUTL_SESSION_compare_test_output(string masterFile, string newFile);

	private static bool verifyAssembly(IntPtr filenameBytes, ref bool alliance_mode)
	{
		bool result = true;
		return result;
	}

	// private static bool verifyAssemblyData()
	// {
	// 	bool alliance_mode = false;
    //     IntPtr filenameBytes = JAM_get_filename(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
    //     bool flag = verifyAssembly(filenameBytes, ref alliance_mode);
    //     if (!flag)
	// 	{
	// 		StackTrace stackTrace = new StackTrace();
	// 		string text = "";
	// 		if (stackTrace.FrameCount > 0)
	// 		{
	// 			for (int i = 0; i < stackTrace.FrameCount; i++)
	// 			{
	// 				if (flag)
	// 				{
	// 					break;
	// 				}
	// 				MethodBase method = stackTrace.GetFrame(i).GetMethod();
	// 				Assembly assembly = method.Module.Assembly;
	// 				string location = assembly.Location;
	// 				if (!(location == text))
	// 				{
	// 					filenameBytes = JAM_get_filename("", location);
	// 					flag = verifyAssembly(filenameBytes, ref alliance_mode);
	// 					text = location;
	// 				}
	// 			}
	// 		}
	// 	}
	// 	if (!flag)
	// 	{
	// 		throw new Exception("Invalid NX signature found");
	// 	}
	// 	if (alliance_mode)
	// 	{
	// 		JAM_declare_alliance_context(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName);
	// 	}
	// 	return alliance_mode;
	// }


	/// <exclude />
	public BaseSession()
	{
		int num = JAM_SESSION_initialize();
		if(num != 0)
		{
			throw NXException.Create(num);
		}
		//InitLicense();
	}

	public static void StaticInitialize()
	{
		
	}

	/// <exclude />
	// public static void InitLicense()
	// {
	// 	bool flag = false;
	// 	try
	// 	{
	// 		flag = verifyAssemblyData();
	// 	}
	// 	catch (Exception ex)
	// 	{
	// 		if (JAM_check_dotnet_author_license() != 0)
	// 		{
    //             throw;
    //         }
	// 		Trace.WriteLine(ex);
	// 		Trace.WriteLine("Validation failed but author license exists - loading library");
	// 	}
	// 	if (!flag)
	// 	{
	// 		int num = JAM_check_session_license(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName);
	// 		if (num != 0)
	// 		{
	// 			Trace.WriteLine("Could not obtain runtime license");
	// 			throw NXException.Create(num);
	// 		}
	// 	}
	// }



	/// <exclude />
	public static void SetTestOutput(string outputFile)
	{
		int num = JAUTL_SESSION_set_test_output(outputFile, 1);
		if (num != 0)
		{
			throw NXException.Create(num);
		}
	}

	/// <exclude />
	public static void SetTestOutput(string outputFile, int version)
	{
		int num = JAUTL_SESSION_set_test_output(outputFile, version);
		if (num != 0)
		{
			throw NXException.Create(num);
		}
	}

	/// <exclude />
	public static void CloseTestOutput()
	{
		int num = JAUTL_SESSION_close_test_output();
		if (num != 0)
		{
			throw NXException.Create(num);
		}
	}

	/// <exclude />
	public static void CompareTestOutput(string originalFile, string newFile)
	{
		int num = JAUTL_SESSION_compare_test_output(originalFile, newFile);
		if (num != 0)
		{
			throw NXException.Create(num);
		}
	}

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
            }
            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            JAM_SESSION_terminate();
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~BaseSession()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
