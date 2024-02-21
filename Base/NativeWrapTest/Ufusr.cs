using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;

namespace UGAot;

public partial class UfusrCaller
{


    // 导入user32.dll（包含我们需要的函数）并定义
    // 对应于本机函数的方法。
    [LibraryImport("user32.dll", EntryPoint = "MessageBoxW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);


    [UnmanagedCallersOnly(EntryPoint = "ufusr")]
    public static unsafe void Ufusr(char* param,int* returnCode,int paramLen)
    {

        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var localEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
            string strGb2312 = localEncoding.GetString((byte*)param, paramLen);
            using var test=new TestClass();
            test.Test();
            returnCode[0] = 0;
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


}

public partial class TestClass:IDisposable
{

    readonly UFSession uf;
    readonly Session _theSession;
    private bool disposedValue;

    public TestClass()
    {
        NXObjectManagerEx.SetGetProvider(new GetProvider());
        Console.WriteLine("Get Session Start");
        uf = UFSession.GetUFSession();
        _theSession = Session.GetSession();
        Console.WriteLine("Get Session End");

    }
    /// <summary>
    /// 获取当前选择器指针
    /// </summary>
    /// <returns></returns>

    [DllImport("libugui.dll", EntryPoint = "?SEL_ask_current_selection_context@@YAPEAUUGUI_selection_s@@XZ")]
    public static extern IntPtr SEL_ask_current_selection_context();

    public void Test2(int cursorView = 1)
    {
        double[] cursorPos = new double[3];
        // theUfSession.Ui.SetCursorView(cursorView);//希望在工程图视图中获取到投影视图中的信息需要设置为0，默认为1
        NXOpen.Utilities.JAM.StartCall();
        IntPtr select = SEL_ask_current_selection_context();
        NXOpen.Utilities.JAM.EndCall();
        Task.Run(async () =>
        {
            var stop=new Stopwatch();
            stop.Start();
            while (stop.ElapsedMilliseconds<1000000)
            {
                await Task.Delay(200);
                uf.Ui.AskSelCursorPos(select, out _, cursorPos);
                Console.WriteLine($"cursorPos:{cursorPos[0]},{cursorPos[1]},{cursorPos[2]}");
            }
            stop.Stop();
        });
        Console.WriteLine("hello");
    }

    public void Test3()
    {


        static void MotionFnT([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] screen_pos, 
        ref NXOpen.UF.UFUi.MotionCbData motion_cb_data, IntPtr data)
        {
            Console.WriteLine($"screen_pos:{screen_pos[0]},{screen_pos[1]},{screen_pos[2]}");
        }
        double[] cursorPos = new double[3];
        int response=0;
        while(response!=UFConstants.UF_UI_CANCEL&&response!=UFConstants.UF_UI_BACK)
        {
            uf.Ui.SpecifyScreenPosition("请选择一个对象", MotionFnT, nint.Zero, cursorPos, out _, out response);
        }
    }


    public void Test()
    {

        var watch = new Stopwatch();
        watch.Start();
        Console.WriteLine("Get parts Start");
        var parts = GetWorkAllParts();
        Console.WriteLine("Get parts End");
        foreach (Part part in parts.Cast<Part>())
        {
            List<Unit> units =
            [
                // part.UnitCollection.GetBase("面积"),
                // part.UnitCollection.GetBase("体积"),
                // part.UnitCollection.GetMeasureTypes("质量").FirstOrDefault(x => x.Abbreviation == "kg"),
                // part.UnitCollection.GetBase("长度"),
                // part.UnitCollection.GetBase("质量密度"),
                part.UnitCollection.GetBase("Area"),
                part.UnitCollection.GetBase("Volume"),
                // part.UnitCollection.GetMeasureTypes("Mass").FirstOrDefault(x => x.Abbreviation == "kg"),
                part.UnitCollection.GetBase("Length"),
                // part.UnitCollection.GetBase("MassDensity"),
            ];

            if (part.Bodies is null)
            {
                continue;
            }
            foreach (var body in part.Bodies)
            {
                if (body.IsBlanked)
                {
                    continue;
                }
                var mea = part.MeasureManager.NewMassProperties([.. units], 0.99, new[] { body });
                Console.WriteLine($"mass:{mea.Mass},area:{mea.Area}");
                mea.Dispose();
                foreach(var face in body.GetFaces())
                {
                    face.Highlight();
                }

            }
        }
        string message = $"耗时:{watch.ElapsedMilliseconds}ms";
        Console.WriteLine(message);
    }

    /// <summary>
    /// 获取当前所有模型
    /// </summary>
    /// <returns>所有模型的集合</returns>
    public IEnumerable<BasePart> GetWorkAllParts()
    {
        var parts = _theSession.Parts;
        var basePart = _theSession.Parts.Work;
        return GetAllSubParts(basePart);
    }

    /// <summary>
    /// 获取当前所有模型
    /// </summary>
    /// <returns>所有模型的集合</returns>
    public IEnumerable<INXObject> GetWorkAllComps()
    {
        var part = _theSession.Parts.Work;
        part.LoadThisPartFully();
        var rootComp = part.ComponentAssembly.RootComponent;
        if (rootComp is null)
        {
            return new INXObject[] { part };
        }

        var result = ExpandTreeData(rootComp,
        x => x.GetChildren()).
        Where(x => x.IsSuppressed == false);
        return result;

    }
    /// <summary>
    /// 获取指定模型及其下属所有模型的集合
    /// </summary>
    /// <param name="part">指定的模型</param>
    /// <returns>指定模型及其下属所有模型的集合</returns>
    public static IEnumerable<BasePart> GetAllSubParts(BasePart part,
    bool isDistinct = true)
    {
        part.LoadThisPartFully();
        var rootComp = part.ComponentAssembly.RootComponent;
        if (rootComp is null)
        {
            return new[] { part };
        }

        var result = ExpandTreeData(rootComp,
        x => x.GetChildren()).
        Where(x => x.IsSuppressed == false).
        Select(x => x.Prototype.OwningPart).
        Where(x => x != null);
        return isDistinct ? result.Distinct() : result;

    }

    /// <summary>
    /// 获取模型的顶层模型
    /// </summary>
    /// <returns>顶层模型</returns>
    public BasePart GetRootParts()
    {
        var basePart = _theSession.Parts.Work;
        var rootComp = basePart.ComponentAssembly.RootComponent;
        if (rootComp is null)
        {
            return basePart;
        }
        return rootComp.Prototype.OwningPart;
    }

    /// <summary>
    /// 将指定的树形结构展开为list,注意是延迟执行的
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="target">树的根节点</param>
    /// <param name="getChildren">从类型T获取chidren</param>
    /// <returns>展开后的树节点列表</returns>
    public static IEnumerable<T> ExpandTreeData<T>(T target, Func<T, IEnumerable<T>> getChildren)
    {
        var list = new List<T> { target };
        while (list.Count > 0)
        {
            var childrenList = new List<T>();
            foreach (var item in list)
            {
                yield return item;
                childrenList.AddRange(getChildren(item));
            }
            list = childrenList;
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
            this._theSession?.Dispose();
            this.uf?.Dispose();
            NXObjectManagerEx.ClearGetProvider();
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~TestClass()
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
