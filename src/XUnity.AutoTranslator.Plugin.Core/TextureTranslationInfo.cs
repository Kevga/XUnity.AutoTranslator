using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.Core.Extensions;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.AutoTranslator.Plugin.Utilities;
using XUnity.Common.Extensions;
using XUnity.Common.Logging;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core
{
   internal class TextureTranslationInfo
   {
      private static Dictionary<string, string> NameToHash = new Dictionary<string, string>();
      private static readonly Encoding UTF8 = new UTF8Encoding( false );
      private static Regex _hashByDataPattern;

      private string _key;
      private byte[] _originalData;
      private bool _initialized;
      private TextureFormat _textureFormat;

      public XUnity.Common.Utilities.WeakReference<Texture2D> Original { get; private set; }

      public Texture2D Translated { get; private set; }

      public Sprite TranslatedSprite { get; set; }

      public bool IsTranslated { get; set; }

      public bool IsDumped { get; set; }

      public bool UsingReplacedTexture { get; set; }

      // INTIALIZE to set texture format???
      // We also have specialized hooks for legacy textures???
      public void Initialize( Texture2D texture )
      {
         if( !_initialized )
         {
            _initialized = true;

            // NOT ALLOWED!!!!
            _textureFormat = texture.format;

            SetOriginal( texture );
         }
      }

      public void SetOriginal( Texture2D texture )
      {
         Original = XUnity.Common.Utilities.WeakReference<Texture2D>.Create( texture );
      }

      private void SetTranslated( Texture2D texture )
      {
         Translated = texture;
      }

      public void CreateTranslatedTexture( byte[] newData, ImageFormat format )
      {
         if( Translated == null )
         {
            var orig = Original.Target;

            var texture = ComponentHelper.CreateEmptyTexture2D( _textureFormat );
#warning HideAndDontSave?
            texture.LoadImageEx( newData, format, orig );

            SetTranslated( texture );

            texture.SetExtensionData( this );

            UsingReplacedTexture = true;
         }
      }

      public void CreateOriginalTexture()
      {
         if( !Original.IsAlive && _originalData != null )
         {
            var texture = ComponentHelper.CreateEmptyTexture2D( _textureFormat );
            texture.LoadImageEx( _originalData, ImageFormat.PNG, null );

            SetOriginal( texture );
         }
      }

      public string GetKey()
      {
         SetupHashAndData( Original.Target );
         return _key;
      }

      public byte[] GetOriginalData()
      {
         SetupHashAndData( Original.Target );
         return _originalData;
      }

      public byte[] GetOrCreateOriginalData()
      {
         // special handling if SetupHashAndData is changed to not support originalData
         // which frankly, is a memory drain

         SetupHashAndData( Original.Target );
         if( _originalData != null ) return _originalData;
         return Original.Target.GetTextureData().Data;
      }

      private TextureDataResult SetupKeyForNameWithFallback( string name, Texture2D texture )
      {
         bool detectedDuplicateName = false;
         string existingHash = null;
         string hash = null;

         TextureDataResult result = null;

         if( Settings.DetectDuplicateTextureNames )
         {
            result = texture.GetTextureData();
            hash = HashHelper.Compute( result.Data );

            if( NameToHash.TryGetValue( name, out existingHash ) )
            {
               if( existingHash != hash )
               {
                  XuaLogger.AutoTranslator.Warn( "Detected duplicate image name: " + name );
                  detectedDuplicateName = true;

                  Settings.AddDuplicateName( name );
               }
            }
            else
            {
               NameToHash[ name ] = hash;
            }
         }

         if( Settings.DuplicateTextureNames.Contains( name ) )
         {
            if( hash == null )
            {
               if( result == null )
               {
                  result = texture.GetTextureData();
               }

               hash = HashHelper.Compute( result.Data );
            }

            _key = hash;
         }
         else
         {
            _key = HashHelper.Compute( UTF8.GetBytes( name ) );
         }

         if( detectedDuplicateName && Settings.EnableTextureDumping )
         {
            var oldKey = HashHelper.Compute( UTF8.GetBytes( name ) );
            AutoTranslationPlugin.Current.TextureCache.RenameFileWithKey( name, oldKey, existingHash );
         }

         return result;
      }

      private void SetupHashAndData( Texture2D texture )
      {
         if( _key != null ) return;

         if( Settings.TextureHashGenerationStrategy == TextureHashGenerationStrategy.FromImageData )
         {
            GenerateHashFromImageData( texture );
         }
         else if( Settings.TextureHashGenerationStrategy == TextureHashGenerationStrategy.FromImageName )
         {
            var name = texture.GetTextureName( null ); // name may be duplicate, WILL be duplicate!
            if( name == null ) return;

            if( _hashByDataPattern != null && _hashByDataPattern.IsMatch( name ) )
            {
               GenerateHashFromImageData( texture );
               return;
            }

            var result = SetupKeyForNameWithFallback( name, texture );

            if( Settings.EnableTextureToggling || Settings.DetectDuplicateTextureNames )
            {
               if( result == null )
               {
                  result = texture.GetTextureData();
               }

               _originalData = result.Data;
            }
         }
         else if( Settings.TextureHashGenerationStrategy == TextureHashGenerationStrategy.FromImageNameAndScene )
         {
            var name = texture.GetTextureName( null ); // name may be duplicate, WILL be duplicate!
            if( name == null ) return;

            name += "|" + TranslationScopeHelper.GetActiveSceneId().ToString();

            var result = SetupKeyForNameWithFallback( name, texture );

            if( Settings.EnableTextureToggling || Settings.DetectDuplicateTextureNames )
            {
               if( result == null )
               {
                  result = texture.GetTextureData();
               }

               _originalData = result.Data;
            }
         }
      }

      private void GenerateHashFromImageData( Texture2D texture )
      {
         var result = texture.GetTextureData();
         _originalData = result.Data;
         _key = HashHelper.Compute( _originalData );
      }

      internal static void SetupRegex( string pattern )
      {
         if( pattern.IsNullOrWhiteSpace() )
         {
            return;
         }

         try
         {
            Regex.Match( "", pattern );
         }
         catch (ArgumentException)
         {
            XuaLogger.AutoTranslator.Error( "Invalid regex pattern for setting 'TextureHashDataPattern'!" );
         }

         _hashByDataPattern = new Regex( pattern, RegexOptions.Compiled );
      }
   }
}
