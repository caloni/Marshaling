using System.Reflection;
using System.Runtime.InteropServices;

public class VariableLengthArrayMarshaler : ICustomMarshaler
{
    private readonly Type objectType;
    private readonly string countFieldPath;

    private VariableLengthArrayMarshaler(string cookie)
    {
        // Expecting cookie format: "Namespace.TypeName;countFieldPath"
        var parts = cookie.Split(';');
        if (parts.Length != 2)
            throw new ArgumentException("Cookie must be in the format 'Namespace.TypeName;countFieldPath'.");

        objectType = Type.GetType(parts[0]) ?? throw new ArgumentException($"Type '{parts[0]}' not found.");
        countFieldPath = parts[1];
    }

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return new VariableLengthArrayMarshaler(cookie);
    }

    public void CleanUpManagedData(object ManagedObj) { }

    public void CleanUpNativeData(IntPtr pNativeData)
    {
        Marshal.FreeHGlobal(pNativeData);
    }

    public int GetNativeDataSize() => -1;

    public IntPtr MarshalManagedToNative(object ManagedObj)
    {
        if (ManagedObj == null)
            return IntPtr.Zero;

        int count = GetNestedFieldValue<int>(ManagedObj, countFieldPath);

        FieldInfo valuesField = objectType.GetField("values");
        if (valuesField == null)
            throw new ArgumentException("Field 'values' not found.");

        Array values = (Array)valuesField.GetValue(ManagedObj);
        if (values.Length != count)
            throw new ArgumentException("Length of 'values' array does not match the count.");

        Type elementType = values.GetType().GetElementType();
        int elementSize = Marshal.SizeOf(elementType);
        int fixedSize = Marshal.OffsetOf(objectType, "values").ToInt32();
        int totalSize = fixedSize + elementSize * count;

        IntPtr ptr = Marshal.AllocHGlobal(totalSize);
        Marshal.StructureToPtr(ManagedObj, ptr, false);

        IntPtr arrayPtr = IntPtr.Add(ptr, fixedSize);
        for (int i = 0; i < count; i++)
        {
            IntPtr elementPtr = IntPtr.Add(arrayPtr, i * elementSize);
            Marshal.StructureToPtr(values.GetValue(i), elementPtr, false);
        }

        return ptr;
    }

    public object MarshalNativeToManaged(IntPtr pNativeData)
    {
        if (pNativeData == IntPtr.Zero)
            return null;

        object obj = Marshal.PtrToStructure(pNativeData, objectType);
        int count = GetNestedFieldValue<int>(obj, countFieldPath);

        FieldInfo valuesField = objectType.GetField("values");
        if (valuesField == null)
            throw new ArgumentException("Field 'values' not found.");

        Type elementType = valuesField.FieldType.GetElementType();
        Array values = Array.CreateInstance(elementType, count);

        int elementSize = Marshal.SizeOf(elementType);
        IntPtr arrayPtr = IntPtr.Add(pNativeData, Marshal.OffsetOf(objectType, "values").ToInt32());
        for (int i = 0; i < count; i++)
        {
            IntPtr elementPtr = IntPtr.Add(arrayPtr, i * elementSize);
            values.SetValue(Marshal.PtrToStructure(elementPtr, elementType), i);
        }

        valuesField.SetValue(obj, values);
        return obj;
    }

    private static T GetNestedFieldValue<T>(object obj, string fieldPath)
    {
        string[] fields = fieldPath.Split('.');
        object currentObj = obj;
        foreach (string fieldName in fields)
        {
            if (currentObj == null)
                throw new NullReferenceException($"Field '{fieldName}' is null.");

            FieldInfo field = currentObj.GetType().GetField(fieldName);
            if (field == null)
                throw new ArgumentException($"Field '{fieldName}' not found in type '{currentObj.GetType().FullName}'.");

            currentObj = field.GetValue(currentObj);
        }
        return (T)currentObj;
    }
}

