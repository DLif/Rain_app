﻿

#pragma checksum "C:\Users\David\Documents\GitHub\Rain_app\3.2\RainMan\RainMan\ColorSlider.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BEC0E9E0C91C81E20BBA5FDE8C02DB92"
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
    partial class ColorSlider : global::Windows.UI.Xaml.Controls.UserControl
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid imageGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard moveSlider; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation moveSliderAnimation; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Shapes.Rectangle mover; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Image Slider; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///ColorSlider.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            imageGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("imageGrid");
            moveSlider = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("moveSlider");
            moveSliderAnimation = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("moveSliderAnimation");
            mover = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("mover");
            Slider = (global::Windows.UI.Xaml.Controls.Image)this.FindName("Slider");
        }
    }
}


