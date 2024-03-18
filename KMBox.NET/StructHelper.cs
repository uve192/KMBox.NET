using System.Runtime.InteropServices;

namespace KMBox.NET;

/// <summary>
/// Tiny structure helper. Turns structs to bytes and back.
/// </summary>
public static class StructHelper
{
    public static byte[] StructToByteArray<T>(T req)
        where T : struct
    {
        var size = Marshal.SizeOf(req);
        var arr = new byte[size];

        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(req, ptr, false);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        
        return arr;
    }
    
    public static byte[] StructToByteArray<T, T2>(T req, T2 req2)
        where T : struct
        where T2 : struct
    {
        var buffSize = Marshal.SizeOf<T>() + Marshal.SizeOf<T2>();
        var buff = new byte[buffSize];

        var struct1 = StructToByteArray(req);
        var struct2 = StructToByteArray(req2);
        
        struct1.CopyTo(buff, 0);
        struct2.CopyTo(buff, struct1.Length);

        return buff;
    }
    
    public static T ByteArrayToStruct<T>(byte[] bytes)
        where T : struct
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        
        try
        {
            var ptr = handle.AddrOfPinnedObject();
            return (T)Marshal.PtrToStructure(ptr, typeof(T))!;
        }
        finally
        {
            handle.Free();
        }
    }
}