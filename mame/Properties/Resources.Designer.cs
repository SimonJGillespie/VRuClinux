﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mame.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("mame.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap _1 {
            get {
                object obj = ResourceManager.GetObject("_1", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;mame version=&quot;1&quot;&gt;
        ///&lt;!--  &lt;game name=&quot;rtype&quot; board=&quot;M72&quot;&gt;
        ///    &lt;parent&gt;&lt;/parent&gt;
        ///    &lt;direction&gt;&lt;/direction&gt;
        ///    &lt;description&gt;R-Type (World)&lt;/description&gt;
        ///    &lt;year&gt;1987&lt;/year&gt;
        ///    &lt;manufacturer&gt;Irem&lt;/manufacturer&gt;
        ///  &lt;/game&gt;
        ///  &lt;game name=&quot;rtypej&quot; board=&quot;M72&quot;&gt;
        ///    &lt;parent&gt;rtype&lt;/parent&gt;
        ///    &lt;direction&gt;&lt;/direction&gt;
        ///    &lt;description&gt;R-Type (Japan)&lt;/description&gt;
        ///    &lt;year&gt;1987&lt;/year&gt;
        ///    &lt;manufacturer&gt;Irem&lt;/manufacturer&gt;
        ///  &lt;/game&gt;
        ///  &lt;game name=&quot;rtypejp&quot; board=&quot;M72&quot;&gt;
        ///    &lt;par [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string mame {
            get {
                return ResourceManager.GetString("mame", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Detail: https://www.codeproject.com/Articles/1275365/MAME-NET
        ///You should install Microsoft .NET Framework 3.5 or higher before running the program. You should download MAME.NET ROM files in roms directory.
        ///Hotkey: F3 -- soft reset, F7 -- load state, Shift+F7 -- save state, F8 -- replay input, Shift+F8 -- record input (start and stop), 0-9 and A-Z after state related hotkey -- handle certain files, F10 -- toggle global throttle, P -- pause and continue, shift+P -- skip a frame.
        ///Control key: 1 -- P1 start, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string readme {
            get {
                return ResourceManager.GetString("readme", resourceCulture);
            }
        }
    }
}
