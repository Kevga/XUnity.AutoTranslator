using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if IL2CPPBE2
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

namespace UnityEngine
{
   public class TextAsset : Object
   {
      public TextAsset( IntPtr pointer ) : base( IntPtr.Zero ) => throw new NotImplementedException();

      public string text => throw new NotImplementedException();

#if IL2CPP
      public UnhollowerBaseLib.Il2CppStructArray<byte> bytes => throw new NotImplementedException();
#elif IL2CPPBE2
      public Il2CppStructArray<byte> bytes => throw new NotImplementedException();
#else
      public byte[] bytes => throw new NotImplementedException();
#endif
   }
}
