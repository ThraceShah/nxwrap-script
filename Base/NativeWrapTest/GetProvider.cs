
using System.Reflection;
using System.Runtime.InteropServices;
using NXOpen;
using NXOpen.Utilities;

sealed partial class GetProvider : IGetProvider
{
    public T Get<T>(Tag objectTag) where T : ITaggedObject
    {
        throw new NotImplementedException();
    }

    private string rootPath = null;
    public unsafe string GetRootPath()
    {
        if(rootPath!=null)
        {
            return rootPath;
        }
#if !AOT
        rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#else
        var funcPtr=(delegate* unmanaged<void>)&Empty;
        _ = GetModuleHandleExW(4u, (nint)funcPtr, out var hModule);
        var moduleName = stackalloc char[1024];
        _ = GetModuleFileNameW(hModule, (nint)moduleName, 1024u);
        rootPath = Path.GetDirectoryName(new string(moduleName));
#endif
        return rootPath;
    }

    [UnmanagedCallersOnly(EntryPoint = "Empty")]
    public static void Empty()
    {

    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetModuleHandleExW(uint dwFlags, nint lpModuleName, out nint phModule);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial uint GetModuleFileNameW(nint hModule, nint lpFilename, uint nSize);
}
