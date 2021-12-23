﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XUnity.Common.Constants;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Extensions
{
   internal static class GameObjectExtensions
   {
#if IL2CPP
      public static Component GetFirstComponentInSelfOrAncestor( this GameObject go, Il2CppSystem.Type type )
      {
         if( type == null ) return null;

         var current = go;

         while( current != null )
         {
            var foundComponent = UnityTypes.GameObject_Methods.GetComponent( current, type );
            if( foundComponent != null )
            {
               return foundComponent;
            }

            current = current.transform?.parent?.gameObject;
         }

         return null;
      }
#endif

#if MANAGED
      public static Component GetFirstComponentInSelfOrAncestor( this GameObject go, Type type )
      {
         if( type == null ) return null;

         var current = go;

         while( current != null )
         {
            var foundComponent = current.GetComponent( type );
            if( foundComponent != null )
            {
               return foundComponent;
            }

            current = current.transform?.parent?.gameObject;
         }

         return null;
      }
#endif
   }
}
