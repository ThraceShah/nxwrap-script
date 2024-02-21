using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace NXOpen.Utilities;

/// <exclude />
public partial struct JAM
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct VariantValue
	{
		[FieldOffset(0)]
		internal int i;

		[FieldOffset(0)]
		internal double d;

		[FieldOffset(0)]
		internal byte l;

		[FieldOffset(0)]
		internal IntPtr ptr;
	}

	public enum VariantType
	{
		Int = 0,
		Double = 1,
		Logical = 2,
		String = 3,
		Tag = 4,
		Empty = 5,
		Variant = 6,
		Array = 4096
	}

	public struct Variant
	{
		internal VariantType type;

		internal int array_length;

		internal VariantValue val;

		internal Variant(int i)
		{
			type = VariantType.Int;
			array_length = 0;
			val.l = 0;
			val.d = 0.0;
			val.ptr = IntPtr.Zero;
			val.i = i;
		}

		internal Variant(double d)
		{
			type = VariantType.Double;
			array_length = 0;
			val.l = 0;
			val.i = 0;
			val.ptr = IntPtr.Zero;
			val.d = d;
		}

		internal Variant(bool b)
		{
			type = VariantType.Logical;
			array_length = 0;
			val.d = 0.0;
			val.i = 0;
			val.ptr = IntPtr.Zero;
			val.l = (byte)(b ? 1u : 0u);
		}

		internal Variant(Tag t)
		{
			type = VariantType.Tag;
			array_length = 0;
			val.l = 0;
			val.d = 0.0;
			val.ptr = IntPtr.Zero;
			val.i = (int)t;
		}

		internal Variant(string s)
		{
			type = VariantType.String;
			array_length = 0;
			val.l = 0;
			val.d = 0.0;
			val.i = 0;
			val.ptr = ToUTF8String(s);
		}

		internal Variant(int array_length, IntPtr ptr, VariantType elemType)
		{
			type = elemType | VariantType.Array;
			this.array_length = array_length;
			val.l = 0;
			val.d = 0.0;
			val.i = 0;
			val.ptr = ptr;
		}

		internal Variant(VariantType t)
		{
			type = VariantType.Empty;
			array_length = 0;
			val.l = 0;
			val.d = 0.0;
			val.i = 0;
			val.ptr = IntPtr.Zero;
		}
	}

	private static Encoding localeEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);

	private static Exception callbackException;

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_start_wrapped_call();

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_end_wrapped_call();

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_start_wrapped_uf_call(string context);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_end_wrapped_uf_call();

	[LibraryImport("libjam", EntryPoint = "JAM_sm_alloc")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr SM_alloc(int nbytes);

	[LibraryImport("libjam", EntryPoint = "JAM_sm_free")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void SM_free(IntPtr ptr);

	[LibraryImport("libjam", EntryPoint = "JAM_env_translate_variable", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ENV_translate_variable(string s);

	[LibraryImport("libjam", EntryPoint = "JAM_text_free")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void TEXT_free(IntPtr ptr);

	[LibraryImport("libjam", EntryPoint = "JAM_text_create_string")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr TEXT_create_string(IntPtr ptr);

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_reserve_license(string licence, string context);

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_reserve_license_pair(string license1, string license2, string context);

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial int JAM_reserve_license_eitheror(int nLicenses, IntPtr[] licenses, string context);

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_set_current_license_context(string context);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint JAM_ask_object_tag(IntPtr ptr);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr JAM_lookup_tag(uint tag);

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint JAM_lookup_singleton_tag(string className);

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr JAM_ask_object_tags(int nPtr, IntPtr ptr);

	internal static string TranslateVariable(string s)
	{
		IntPtr s2 = ENV_translate_variable(s);
		return ToStringFromLocale(s2);
	}

	public static byte[] ToByteArray(bool[] flags)
	{
		int num = flags.Length;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Convert.ToByte(flags[i]);
		}
		return array;
	}

	public static Tag[] ToTagArray(ITaggedObject[] objects)
	{
		int num = objects.Length;
		Tag[] array = new Tag[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ((objects[i] != null) ? objects[i].Tag : Tag.Null);
		}
		return array;
	}

    // public static Tag[] ToTagArray(object[] objects)
    // {
    // 	int num = objects.Length;
    // 	Tag[] array = new Tag[num];
    // 	for (int i = 0; i < num; i++)
    // 	{
    // 		array[i] = ((objects[i] != null) ? ((TaggedObject)objects[i]).Tag : Tag.Null);
    // 	}
    // 	return array;
    // }

    public static Tag[] ToTagArray<T>(T[] objects)where T:ITaggedObject
    {
        int num = objects.Length;
        Tag[] array = new Tag[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = (objects[i] != null) ? objects[i].Tag : Tag.Null;
        }
        return array;
    }


    public static IntPtr[] ToPointerArray(TransientObject[] objects)
	{
		int num = objects.Length;
		IntPtr[] array = new IntPtr[num];
		for (int i = 0; i < num; i++)
		{
			ref IntPtr reference = ref array[i];
			reference = ((objects[i] == null) ? IntPtr.Zero : objects[i].Handle);
		}
		return array;
	}

	public static int[] ToIdArray(IHasHandle[] objects)
	{
		int num = objects.Length;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ((objects[i] != null) ? objects[i].Handle : 0);
		}
		return array;
	}

	public static IntPtr[] ToPointerArray(TaggedObject[] objects)
	{
		int num = objects.Length;
		IntPtr[] array = new IntPtr[num];
		for (int i = 0; i < num; i++)
		{
			ref IntPtr reference = ref array[i];
			reference = ((objects[i] == null) ? IntPtr.Zero : Lookup(objects[i].Tag));
		}
		return array;
	}

	public unsafe static void FreeVariant(Variant v)
	{
		if ((v.type & VariantType.Array) != 0)
		{
			VariantType variantType = v.type & (VariantType)(-4097);
			if (variantType == VariantType.String)
			{
				int array_length = v.array_length;
				for (int i = 0; i < array_length; i++)
				{
					SM_free(Marshal.ReadIntPtr(v.val.ptr, i * sizeof(IntPtr)));
				}
			}
			SM_free(v.val.ptr);
		}
		else if (v.type == VariantType.String)
		{
			SM_free(v.val.ptr);
		}
	}

	public unsafe static Variant ToVariant(object o)
	{
		if (o is null)
		{
			return new Variant(VariantType.Empty);
		}
		if (o is int v)
		{
			return new Variant(v);
		}
		if (o is double v1)
		{
			return new Variant(v1);
		}
		if (o is bool v2)
		{
			return new Variant(v2);
		}
		if (o is TaggedObject @object)
		{
			return new Variant(@object.Tag);
		}
		if (o is string v3)
		{
			return new Variant(v3);
		}
		if (o is Array)
		{
			Type elementType = o.GetType().GetElementType();
			if (elementType == typeof(int))
			{
				int[] array = (int[])o;
				IntPtr intPtr = SM_alloc(array.Length * 4);
				Marshal.Copy(array, 0, intPtr, array.Length);
				return new Variant(array.Length, intPtr, VariantType.Int);
			}
			if (elementType == typeof(bool))
			{
				bool[] array2 = (bool[])o;
				IntPtr ptr = SM_alloc(array2.Length);
				for (int i = 0; i < array2.Length; i++)
				{
					Marshal.WriteByte(ptr, i, (byte)(array2[i] ? 1u : 0u));
				}
				return new Variant(array2.Length, ptr, VariantType.Logical);
			}
			if (elementType == typeof(double))
			{
				double[] array3 = (double[])o;
				IntPtr intPtr2 = SM_alloc(array3.Length * 8);
				Marshal.Copy(array3, 0, intPtr2, array3.Length);
				return new Variant(array3.Length, intPtr2, VariantType.Double);
			}
			if (elementType == typeof(string))
			{
				string[] array4 = (string[])o;
				IntPtr ptr2 = SM_alloc(array4.Length * sizeof(IntPtr));
				for (int j = 0; j < array4.Length; j++)
				{
					Marshal.WriteIntPtr(ptr2, j * sizeof(IntPtr), ToUTF8String(array4[j]));
				}
				return new Variant(array4.Length, ptr2, VariantType.String);
			}
			if (typeof(TaggedObject).IsAssignableFrom(elementType))
			{
				object[] array5 = (object[])o;
				IntPtr ptr3 = SM_alloc(array5.Length * 4);
				for (int k = 0; k < array5.Length; k++)
				{
					Marshal.WriteInt32(ptr3, k * 4, (int)((TaggedObject)array5[k]).Tag);
				}
				return new Variant(array5.Length, ptr3, VariantType.Tag);
			}
			object[] array6 = (object[])o;
			int num = Marshal.SizeOf<Variant>();
			IntPtr ptr4 = SM_alloc(array6.Length * num);
			byte* ptr5 = (byte*)ptr4.ToPointer();
			for (int l = 0; l < array6.Length; l++)
			{
				Variant variant = ToVariant(array6[l]);
				Marshal.StructureToPtr(variant, new IntPtr(ptr5), fDeleteOld: true);
				ptr5 += num;
			}
			return new Variant(array6.Length, ptr4, VariantType.Variant);
		}
		throw new ArgumentException("Can not handle object type: " + o.GetType().ToString());
	}

	public static void FreeVariantArray(Variant[] v)
	{
		for (int i = 0; i < v.Length; i++)
		{
			if ((v[i].type & VariantType.Array) != 0)
			{
				SM_free(v[i].val.ptr);
			}
		}
	}

	public static Variant[] ToVariantArray(object[] objects)
	{
		if (objects == null)
		{
			return null;
		}
		int num = objects.Length;
		Variant[] array = new Variant[num];
		for (int i = 0; i < num; i++)
		{
			ref Variant reference = ref array[i];
			reference = ToVariant(objects[i]);
		}
		return array;
	}

	public static object ToObject(Variant v)
	{
		switch (v.type & (VariantType)(-4097))
		{
		case VariantType.Empty:
			if ((v.type & VariantType.Array) != 0)
			{
				throw new ArgumentException("array of Variant.Empty not allowed");
			}
			return null;
		case VariantType.Int:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToIntArray(v.array_length, v.val.ptr);
			}
			return v.val.i;
		case VariantType.Double:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToDoubleArray(v.array_length, v.val.ptr);
			}
			return v.val.d;
		case VariantType.Logical:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToBoolArray(v.array_length, v.val.ptr);
			}
			return v.val.l != 0;
		case VariantType.String:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToStringArrayFromUTF8(v.array_length, v.val.ptr);
			}
			return ToStringFromUTF8(v.val.ptr);
		case VariantType.Tag:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToObjectArray<TaggedObject>(v.array_length, v.val.ptr);
			}
			return NXObjectManager.Get((Tag)v.val.i);
		case VariantType.Variant:
			if ((v.type & VariantType.Array) != 0)
			{
				return ToObjectArray(v.array_length, v.val.ptr);
			}
			throw new ArgumentException("VariantType.Variant only valid for arrays");
		default:
			throw new ArgumentException("bad Variant object");
		}
	}

	public static object[] ToObjectArray(Tag[] tags)
	{
		object[] array = new object[tags.Length];
		for (int i = 0; i < tags.Length; i++)
		{
			array[i] = NXObjectManager.Get(tags[i]);
		}
		return array;
	}

	public unsafe static T[] ToStructureArray<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors| DynamicallyAccessedMemberTypes.NonPublicConstructors)] 
	T>(int count, IntPtr structs)where T:struct
	{
		if (structs == IntPtr.Zero)
		{
			return null;
		}
		structs.ToPointer();
		var array = new T[count];
		IntPtr ptr = structs;
		int num = Marshal.SizeOf<T>();
		for (int i = 0; i < count; i++)
		{
			array[i]=Marshal.PtrToStructure<T>(ptr);
			ptr = (IntPtr)((byte*)ptr.ToPointer() + num);
		}
		return array;
	}

	public unsafe static object[] ToObjectArray(int count, IntPtr variants)
	{
		if (variants == IntPtr.Zero)
		{
			return null;
		}
        object[] array = new object[count];
		IntPtr ptr = variants;
		int num = Marshal.SizeOf<Variant>();
		for (int i = 0; i < count; i++)
		{
			Variant v = Marshal.PtrToStructure<Variant>(ptr);
			array[i] = ToObject(v);
			ptr = (IntPtr)((byte*)ptr.ToPointer() + num);
		}
		SM_free(variants);
		return array;
	}

    // public unsafe static Array ToObjectArray(Type elemType, int count, IntPtr tags)
    // {
    // 	uint* ptr = (uint*)tags.ToPointer();
    // 	Array array = Array.CreateInstance(elemType, count);
    // 	for (int i = 0; i < count; i++)
    // 	{
    // 		if (ptr[i] != 0)
    // 		{
    // 			array.SetValue(NXObjectManager.Get((Tag)ptr[i]), i);
    // 		}
    // 	}
    // 	SM_free(tags);
    // 	return array;
    // }

    public unsafe static T[] ToObjectArray<T>(int count, IntPtr tags)where T:ITaggedObject,new()
    {
    	uint* ptr = (uint*)tags.ToPointer();
    	var array = new T[count];
    	for (int i = 0; i < count; i++)
    	{
    		if (ptr[i] != 0)
    		{
                array[i] = NXObjectManager.Get<T>((Tag)ptr[i]);
            }
    	}
    	SM_free(tags);
    	return array;
    }

    // public static Array ToObjectArray(Type elemType, int count, IntPtr tagsOrObjectArray, bool isNXTagObject)
	// {
	// 	IntPtr zero = IntPtr.Zero;
	// 	if (isNXTagObject)
	// 	{
	// 		zero = JAM_ask_object_tags(count, tagsOrObjectArray);
	// 		SM_free(tagsOrObjectArray);
	// 	}
	// 	else
	// 	{
	// 		zero = tagsOrObjectArray;
	// 	}
	// 	return ToObjectArray(elemType, count, zero);
	// }

    public static T[] ToObjectArray<T>(int count, IntPtr tagsOrObjectArray, bool isNXTagObject)where T : ITaggedObject, new()
    {
        nint zero;
        if (isNXTagObject)
        {
            zero = JAM_ask_object_tags(count, tagsOrObjectArray);
            SM_free(tagsOrObjectArray);
        }
        else
        {
            zero = tagsOrObjectArray;
        }
        return ToObjectArray<T>(count, zero);
    }

    public unsafe static T[] ToObjectArray2<T>(int count, IntPtr tags) where T : ITaggedObject
    {
        uint* ptr = (uint*)tags.ToPointer();
        var array = new T[count];
        for (int i = 0; i < count; i++)
        {
            if (ptr[i] != 0)
            {
                array[i] = NXObjectManagerEx.Get<T>((Tag)ptr[i]);
            }
        }
        SM_free(tags);
        return array;
    }

    public static T[] ToObjectArray2<T>(int count, IntPtr tagsOrObjectArray, bool isNXTagObject) where T : ITaggedObject
    {
        nint zero;
        if (isNXTagObject)
        {
            zero = JAM_ask_object_tags(count, tagsOrObjectArray);
            SM_free(tagsOrObjectArray);
        }
        else
        {
            zero = tagsOrObjectArray;
        }
        return ToObjectArray2<T>(count, zero);
    }

    // public unsafe static Array ToEnumArray(Type elemType, int count, IntPtr enums)
	// {
	// 	int* ptr = (int*)enums.ToPointer();
	// 	Array array = Array.CreateInstance(elemType, count);
	// 	for (int i = 0; i < count; i++)
	// 	{
	// 		array.SetValue(Enum.ToObject(elemType, ptr[i]), i);
	// 	}
	// 	SM_free(enums);
	// 	return array;
	// }

    public unsafe static T[] ToEnumArray<T>(int count, IntPtr enums)where T : unmanaged, Enum
    {
        T* ptr = (T*)enums.ToPointer();
        var array = new T[count];
        for (int i = 0; i < count; i++)
        {
            array[i]=*(ptr+i);
        }
        SM_free(enums);
        return array;
    }


    public static int[] ToIntArray(int count, IntPtr ints)
	{
		int[] array = new int[count];
		if (count > 0)
		{
			Marshal.Copy(ints, array, 0, count);
		}
		SM_free(ints);
		return array;
	}

	public static double[] ToDoubleArray(int count, IntPtr doubles)
	{
		double[] array = new double[count];
		if (count > 0)
		{
			Marshal.Copy(doubles, array, 0, count);
		}
		SM_free(doubles);
		return array;
	}

	public unsafe static bool[] ToBoolArray(int count, IntPtr logicals)
	{
		bool[] array = new bool[count];
		byte* ptr = (byte*)logicals.ToPointer();
		for (int i = 0; i < count; i++)
		{
			array[i] = ptr[i] != 0;
		}
		SM_free(logicals);
		return array;
	}

	public unsafe static string[] ToStringArrayFromLocale(int count, IntPtr strings)
	{
		string[] array = new string[count];
		IntPtr* ptr = (IntPtr*)strings.ToPointer();
		for (int i = 0; i < count; i++)
		{
			array[i] = ToStringFromLocale(ptr[i]);
		}
		SM_free(strings);
		return array;
	}

	internal unsafe static string[] ToStringArrayFromUTF8(int count, IntPtr strings)
	{
		string[] array = new string[count];
		byte** ptr = (byte**)strings.ToPointer();
		for (int i = 0; i < count; i++)
		{
			array[i] = ToStringFromUTF8((IntPtr)ptr[i]);
		}
		SM_free(strings);
		return array;
	}

	public unsafe static string[] ToStringArrayFromText(int count, IntPtr strings)
	{
		string[] array = new string[count];
		IntPtr* ptr = (IntPtr*)strings.ToPointer();
		for (int i = 0; i < count; i++)
		{
			array[i] = ToStringFromText(ptr[i]);
		}
		SM_free(strings);
		return array;
	}

	private unsafe static int strlen(IntPtr ip)
	{
		byte* ptr = (byte*)ip.ToPointer();
		byte* ptr2;
		for (ptr2 = ptr; *ptr2 != 0; ptr2++)
		{
		}
		return (int)(ptr2 - ptr);
	}

	public static string ToStringFromText(IntPtr text)
	{
		if (text == IntPtr.Zero)
		{
			return null;
		}
		int num = strlen(text);
		byte[] array = new byte[num];
		if (num > 0)
		{
			Marshal.Copy(text, array, 0, num);
		}
		TEXT_free(text);
		return Encoding.UTF8.GetString(array);
	}

	public static string ToStringFromLocale(IntPtr s, bool doFree)
	{
		if (s == IntPtr.Zero)
		{
			return null;
		}
		int num = strlen(s);
		byte[] array = new byte[num];
		if (num > 0)
		{
			Marshal.Copy(s, array, 0, num);
		}
		if (doFree)
		{
			SM_free(s);
		}
		return localeEncoding.GetString(array);
	}

	public static string ToStringFromLocale(IntPtr s)
	{
		return ToStringFromLocale(s, doFree: true);
	}

	internal static string ToStringFromUTF8(IntPtr s)
	{
		if (s == IntPtr.Zero)
		{
			return null;
		}
		int num = strlen(s);
		byte[] array = new byte[num];
		if (num > 0)
		{
			Marshal.Copy(s, array, 0, num);
		}
		SM_free(s);
		return Encoding.UTF8.GetString(array);
	}

	public unsafe static IntPtr ToText(string s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte* ptr = stackalloc byte[(int)(uint)(bytes.Length + 1)];
		for (int i = 0; i < bytes.Length; i++)
		{
			ptr[i] = bytes[i];
		}
		ptr[bytes.Length] = 0;
		return TEXT_create_string((IntPtr)ptr);
	}

	public static IntPtr[] ToTextArray(string[] strings)
	{
		IntPtr[] array = new IntPtr[strings.Length];
		for (int i = 0; i < strings.Length; i++)
		{
			ref IntPtr reference = ref array[i];
			reference = ToText(strings[i]);
		}
		return array;
	}

	public static void FreeTextArray(IntPtr[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			TextFree(array[i]);
		}
	}

	public static IntPtr[] ToLocaleStringArray(string[] strings)
	{
		IntPtr[] array = new IntPtr[strings.Length];
		for (int i = 0; i < strings.Length; i++)
		{
			ref IntPtr reference = ref array[i];
			reference = ToLocaleString(strings[i]);
		}
		return array;
	}

	public static void FreeLocaleStringArray(IntPtr[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			FreeLocaleString(array[i]);
		}
	}

	public static IntPtr ToLocaleString(string s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		byte[] bytes = localeEncoding.GetBytes(s);
		if (!localeEncoding.GetString(bytes).Equals(s))
		{
			throw NXException.Create(1510010);
		}
		IntPtr intPtr = SM_alloc(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte(intPtr, bytes.Length, 0);
		return intPtr;
	}

	internal static IntPtr ToUTF8String(string s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		IntPtr intPtr = SM_alloc(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte(intPtr, bytes.Length, 0);
		return intPtr;
	}

	public static void FreeLocaleString(IntPtr p)
	{
		SM_free(p);
	}

	public static void TextFree(IntPtr text)
	{
		TEXT_free(text);
	}

	public static void SMFree(IntPtr p)
	{
		SM_free(p);
	}

	public static void StartCall()
	{
		JAM_start_wrapped_call();
	}

	public static void StartCall(string license)
	{
		int num = JAM_reserve_license(license, GetLicenseContext());
		if (num != 0)
		{
			throw NXException.Create(num);
		}
		JAM_start_wrapped_call();
	}

	public static void StartCall(string license1, string license2)
	{
		int num = JAM_reserve_license_pair(license1, license2, GetLicenseContext());
		if (num != 0)
		{
			throw NXException.Create(num);
		}
		JAM_start_wrapped_call();
	}

	public static void StartCall(string[] licenses)
	{
		IntPtr[] array = ToLocaleStringArray(licenses);
		int num = JAM_reserve_license_eitheror(licenses.Length, array, GetLicenseContext());
		FreeLocaleStringArray(array);
		if (num != 0)
		{
			throw NXException.Create(num);
		}
		JAM_start_wrapped_call();
	}

	public static void EndCall()
	{
		JAM_end_wrapped_call();
	}

	public static void StartUFCall()
	{
		JAM_start_wrapped_uf_call(GetLicenseContext());
	}

	public static void EndUFCall()
	{
		JAM_end_wrapped_uf_call();
	}

	public static Tag Lookup(IntPtr ptr)
	{
		return (Tag)JAM_ask_object_tag(ptr);
	}

	public static IntPtr Lookup(Tag tag)
	{
		return JAM_lookup_tag((uint)tag);
	}

	public static Tag GetSingletonTag(string className)
	{
		return (Tag)JAM_lookup_singleton_tag(className);
	}

	public static string GetLicenseContext()
	{
		return NXObjectManagerEx.GetProvider.GetRootPath() + AppDomain.CurrentDomain.FriendlyName;
	}

	public static void SetLicenseContext()
	{
		JAM_set_current_license_context(GetLicenseContext());
	}

	[LibraryImport("libjam")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_clear_callback_exception();

	[LibraryImport("libjam", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void JAM_set_callback_exception(string s);

	public static void SetCallbackException(Exception e)
	{
		callbackException = e;
		JAM_set_callback_exception(e.Message);
	}

	public static void ClearCallbackException()
	{
		callbackException = null;
		JAM_clear_callback_exception();
	}
}
