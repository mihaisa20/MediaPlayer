﻿

#pragma checksum "C:\Users\Mihai\Documents\GitHub\MediaPlayer\MediaPlayer\MediaPlayer\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7CA15F16599169E667B4EBD1C17A5FA6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaPlayer
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 14 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Set_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 79 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.PlayPause_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 80 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Next_track_Tapped;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 81 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Prev_track_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 82 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.FeelLucky_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


