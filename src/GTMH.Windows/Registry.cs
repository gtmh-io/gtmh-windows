using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Text;

namespace GTMH
{
  public static class Registry
  {
    public static T GetValue<T>(string a_Key, T a_Default, string ? a_Basename = null)
    {
      var path = GetPath(a_Basename);
      var parentKey = global::Microsoft.Win32.Registry.CurrentUser;
      for(int i = 0; i != path.Length; ++i)
      {
        parentKey = parentKey.OpenSubKey(path[i]);
        if(parentKey == null)
        {
          return a_Default;
        }
      }
      var val = parentKey.GetValue(a_Key);
      if ( val == null ) return a_Default;
      return (T)System.Convert.ChangeType(val, typeof(T));
    }
    public static void SetValue<T>(string a_Key, T a_Value, string ? a_Basename = null)
    {
      var strVal = Convert.ToString(a_Value);
      if ( strVal == null ) throw new ArgumentException("You can't write this value");
    
      var path = GetPath(a_Basename);
			var parentKey = global::Microsoft.Win32.Registry.CurrentUser;
      for (int i = 0; i != path.Length; ++i)
      {
        parentKey = parentKey.CreateSubKey(path[i]);
      }
      parentKey.SetValue(a_Key, strVal, RegistryValueKind.String);
    }

    private static string[] GetPath(string ? a_Basename)
    {
      if(a_Basename == null)
      {
        var n = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        if(n != null)
        {
          return new[] { "Software", "GTMH", n};
        }
        else
        {
          return new[] { "Software", "GTMH" };
        }
      }
      else if(String.IsNullOrWhiteSpace(a_Basename))
      {
        return new[] { "Software", "GTMH" };
      }
      else
      {
        return new[] { "Software", "GTMH", a_Basename };
      }
    }
  }
}
