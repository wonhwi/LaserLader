using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityCode
{
  public static T DeepCopy<T>(this T obj)
  {
    using (var stream = new System.IO.MemoryStream())
    {
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      formatter.Serialize(stream, obj);
      stream.Position = 0;

      return (T)formatter.Deserialize(stream);
    }
  }

  public static void Clear<T>(this T[] array)
  {
    System.Array.Clear(array, 0, array.Length);
  }
}
