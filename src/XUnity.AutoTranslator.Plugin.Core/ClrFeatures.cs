using System;
using System.Reflection.Emit;
using XUnity.Common.Constants;

namespace XUnity.AutoTranslator.Plugin.Core
{
   internal static class ClrFeatures
   {
      internal static bool SupportsNet4x { get; } = false;

      internal static bool SupportsReflectionEmit { get; } = false;

      static ClrFeatures()
      {
         try
         {
            SupportsNet4x = ClrTypes.Task != null;
         }
         catch( Exception )
         {

         }

         try
         {
#if !IL2CPPBE2
            TestReflectionEmit();
#endif

            SupportsReflectionEmit = true;
         }
         catch( Exception )
         {
            SupportsReflectionEmit = false;
         }
      }

#if !IL2CPPBE2
      private static void TestReflectionEmit()
      {
         MethodToken t1 = default( MethodToken );
         MethodToken t2 = default( MethodToken );
         var ok = t1 == t2;
      }
#endif
   }
}
