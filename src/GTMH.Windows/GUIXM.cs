using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GTMH.S11n.GUI
{
  public static class GUIXM
  {
    public static bool SaveWindowState(this Form a_This, string ? a_FormName = null)
    {
      a_FormName = (a_FormName ?? a_This.Name) ?? "DefaultFormName";
      bool rval = false;
      try
      {
        Func<string, string> name_for = s => a_FormName + "_" + s;
        Registry.SetValue(name_for("WindowState"), a_This.WindowState);
        Registry.SetValue(name_for("X"), a_This.Location.X);
        Registry.SetValue(name_for("Y"), a_This.Location.Y);
        Registry.SetValue(name_for("Width"), a_This.Size.Width);
        Registry.SetValue(name_for("Height"), a_This.Size.Height);
        rval = true;
      }
      catch { }
      return rval;
    }
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern short GetKeyState(int nVirtKey);
    public const int VK_LCONTROL = 0xA2;
    public const int VK_RCONTROL = 0xA3;
    public const int VK_CONTROL = 0x11;
     
    public static bool IsLControlDown(this Control a_Form) { return GetKeyState(VK_LCONTROL) <0; }
    public static bool IsRControlDown(this Control a_Form) { return GetKeyState(VK_RCONTROL) <0; }
    public static bool IsControlDown(this Control a_Form) { return GetKeyState(VK_CONTROL) <0; }

    public static bool LoadWindowState(this Form a_This, string ? a_FormName = null)
    {
      a_FormName = (a_FormName ?? a_This.Name) ?? "DefaultFormName";
      bool rval = false;
      try
      {
        if(!a_This.IsLControlDown()) // allow to suppress in case of nutzo
        {
          Func<string, string> name_for = s => a_FormName + "_" + s;
          var loc = new Point(Registry.GetValue(name_for("X"), a_This.Location.X), Registry.GetValue(name_for("Y"), a_This.Location.Y));
          var sz = new Size(Registry.GetValue(name_for("Width"), a_This.Size.Width), Registry.GetValue(name_for("Height"), a_This.Size.Height));
          // allow for monitors disappearing
          if(Array.Find(Screen.AllScreens, _ => _.WorkingArea.IntersectsWith(new Rectangle(loc, sz))) != null)
          {
            a_This.Size = sz;
            a_This.Location = loc;
          }
          a_This.WindowState = Registry.GetValue(name_for("WindowState"), a_This.WindowState);
        }
        rval = true;
      }
      catch { } 
      return rval;
    }
		public struct FileDlgLast : IDisposable
		{
			private FileDialog m_Dlg { get; }
			private string m_Key { get; }
      private bool m_Commit;

			public FileDlgLast(FileDialog a_Dlg, string a_Key)
			{
				this.m_Dlg = a_Dlg;
				this.m_Key = a_Key;
        this.m_Commit = false;

        if (a_Key != null)
        {
          try
          {
            var last = Registry.GetValue(a_Key, "");
            if (System.IO.Directory.Exists(last))
            {
              a_Dlg.InitialDirectory = last;
            }
          }
          catch { }
        }
			}

      public void Commit()
      {
        m_Commit = true;
      }

			public void Dispose()
			{
        if (m_Commit && m_Key != null)
        {
          var k = m_Key;
          var d = m_Dlg;
          try
          {
            Registry.SetValue(k, System.IO.Path.GetDirectoryName(d.FileName));
          }
          catch { }
        }
			}
		}
		public struct FolderDlgLast : IDisposable
		{
			private FolderBrowserDialog m_Dlg { get; }
			private string m_Key { get; }
      private bool m_Commit;

			public FolderDlgLast(FolderBrowserDialog a_Dlg, string a_Key)
			{
				this.m_Dlg = a_Dlg;
				this.m_Key = a_Key;
        this.m_Commit = false;

        if (a_Key != null)
        {
          try
          {
            var last = Registry.GetValue(a_Key, "");
            if (System.IO.Directory.Exists(last))
            {
              a_Dlg.SelectedPath = last;
            }
          }
          catch { }
        }
			}

      public void Commit()
      {
        m_Commit = true;
      }

			public void Dispose()
			{
        if (m_Commit && m_Key != null)
        {
          var k = m_Key;
          var d = m_Dlg;
          try
          {
            Registry.SetValue(k, d.SelectedPath);
          }
          catch { } 
        }
			}
		}

		public static FileDlgLast LastLocation(this FileDialog a_Dlg, string a_Key)
    {
      return new FileDlgLast(a_Dlg, a_Key);
    }
		public static FolderDlgLast LastLocation(this FolderBrowserDialog a_Dlg, string a_Key)
    {
      return new FolderDlgLast(a_Dlg, a_Key);
    }
  }
}
