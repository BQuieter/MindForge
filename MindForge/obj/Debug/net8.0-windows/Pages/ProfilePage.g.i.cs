﻿#pragma checksum "..\..\..\..\Pages\ProfilePage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0794D66B012F27279D3249EE54CF2FF6FD7F64DC"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using MindForgeClient.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MindForgeClient.Pages {
    
    
    /// <summary>
    /// ProfilePage
    /// </summary>
    public partial class ProfilePage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 23 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ProfileImage;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LoginTextBlock;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ProfessionListBox;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ProfessionsComboBox;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid AddProfessionGrid;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Description;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button EditButton;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\..\Pages\ProfilePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MindForgeClient;component/pages/profilepage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Pages\ProfilePage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\Pages\ProfilePage.xaml"
            ((MindForgeClient.Pages.ProfilePage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ProfileImage = ((System.Windows.Controls.Image)(target));
            
            #line 23 "..\..\..\..\Pages\ProfilePage.xaml"
            this.ProfileImage.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ProfileImage_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\..\Pages\ProfilePage.xaml"
            this.ProfileImage.MouseEnter += new System.Windows.Input.MouseEventHandler(this.ProfileImage_MouseEnter);
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\..\Pages\ProfilePage.xaml"
            this.ProfileImage.MouseLeave += new System.Windows.Input.MouseEventHandler(this.ProfileImage_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 3:
            this.LoginTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.ProfessionListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 6:
            this.ProfessionsComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 43 "..\..\..\..\Pages\ProfilePage.xaml"
            this.ProfessionsComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ChooseProfession);
            
            #line default
            #line hidden
            
            #line 43 "..\..\..\..\Pages\ProfilePage.xaml"
            this.ProfessionsComboBox.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.ProfessionsComboBox_IsVisibleChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.AddProfessionGrid = ((System.Windows.Controls.Grid)(target));
            
            #line 50 "..\..\..\..\Pages\ProfilePage.xaml"
            this.AddProfessionGrid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.AddProfessionGrid_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Description = ((System.Windows.Controls.TextBox)(target));
            
            #line 73 "..\..\..\..\Pages\ProfilePage.xaml"
            this.Description.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Description_TextChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 79 "..\..\..\..\Pages\ProfilePage.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.Cancel_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.EditButton = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\..\..\Pages\ProfilePage.xaml"
            this.EditButton.Click += new System.Windows.RoutedEventHandler(this.Edit_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.SaveButton = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\..\..\Pages\ProfilePage.xaml"
            this.SaveButton.Click += new System.Windows.RoutedEventHandler(this.Save_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 5:
            
            #line 36 "..\..\..\..\Pages\ProfilePage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteProfession);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

