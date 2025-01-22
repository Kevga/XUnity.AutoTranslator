
using System;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.Common.Constants;

namespace XUnity.AutoTranslator.Plugin.Core.UIResize
{
   public class NGUI_OverflowMode: INGUI_OverflowMode
   {
      private int? _mode;

      public NGUI_OverflowMode( string[] args )
      {
         if( args.Length != 1 ) throw new ArgumentException( $"{nameof(NGUI_OverflowMode)} requires one argument." );

         _mode = (int)EnumHelper.GetValues( UnityTypes.UILabelOverflowMode.ClrType, args[ 0 ] );
      }

      public int? GetMode()
      {
         return _mode;
      }
   }
}
