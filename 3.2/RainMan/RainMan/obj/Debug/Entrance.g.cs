﻿

#pragma checksum "C:\Users\Denis\documents\visual studio 2013\Projects\RainMan\RainMan\Entrance.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "80E088EE973E82F23391A68BAF8298DD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RainMan
{
    partial class Entrance : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 34 "..\..\Entrance.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.fadeOutText_Completed;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

