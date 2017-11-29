using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for CompletionControls.xaml
    /// </summary>
    public partial class CompletionControls : UserControl
    {
        public CompletionWindow completionWindow { get; set; }

        public CompletionControls(CompletionWindow completionWindow)
        {
            InitializeComponent();
            this.Effect = null;
            this.completionWindow = completionWindow;
            completionWindow.Width = 350;
            this.Width = 350;
            AddStandards();
            LoadStacks();
        }

        static CompletionControls()
        {
            dict = new Dictionary<int, System.Windows.Controls.Image>();

            int c = GetHashCode((int)SymbolKind.Property, (int)0, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Property_16x));
            c = GetHashCode((int)SymbolKind.Property, 0, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.PropertyProtect_16x));
            c = GetHashCode((int)SymbolKind.Property, 0, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.PropertyPrivate_16x));

            c = GetHashCode((int)SymbolKind.Method, 0, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Method_purple_16x));
            c = GetHashCode((int)SymbolKind.Method, 0, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.MethodProtect_16x));
            c = GetHashCode((int)SymbolKind.Method, 0, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.MethodPrivate_16x));

            c = GetHashCode((int)SymbolKind.Field, 0, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Field_blue_16x));
            c = GetHashCode((int)SymbolKind.Field, 0, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.FieldProtect_16x));
            c = GetHashCode((int)SymbolKind.Field, 0, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.FieldPrivate_16x));

            c = GetHashCode((int)SymbolKind.Event, 0, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Event_orange_16x1));
            c = GetHashCode((int)SymbolKind.Event, 0, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.EventProtect_16x));
            c = GetHashCode((int)SymbolKind.Event, 0, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.EventPrivate_16x));

            c = GetHashCode((int)SymbolKind.Namespace, 0, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Namespace_16x));
            c = GetHashCode((int)SymbolKind.Namespace, 0, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.NamespaceProtect_16x));
            c = GetHashCode((int)SymbolKind.Namespace, 0, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.NamespaceProtect_16x));

            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Class, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Class_yellow_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Class, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.ClassProtected_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Class, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.ClassPrivate_16x));

            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Interface, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Interface_blue_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Interface, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.InterfaceProtect_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Interface, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.InterfacePrivate_16x));

            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Delegate, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Delegate_purple_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Delegate, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.DelegateProtected_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Delegate, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.DelegatePrivate_16x));

            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Enum, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Enumerator_orange_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Enum, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.EnumProtect_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Enum, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.EnumPrivate_16x));

            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Struct, (int)Accessibility.Public, 0);
            dict.Add(c, Load(WinExplorers.ve.Structure_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Struct, (int)Accessibility.Protected, 0);
            dict.Add(c, Load(WinExplorers.ve.StructureProtect_16x));
            c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Struct, (int)Accessibility.Private, 0);
            dict.Add(c, Load(WinExplorers.ve.StructurePrivate_16x));

            c = GetHashCode((int)0, (int)0, (int)0, ((string)"NamespaceDeclarationSyntax").GetHashCode());
            dict.Add(c, Load(WinExplorers.ve.CS_ProjectSENode_16x));
            c = GetHashCode((int)0, (int)0, (int)0, ((string)"MethodDeclarationSyntax").GetHashCode());
            dict.Add(c, Load(WinExplorers.ve.Method_purple_16x));
            c = GetHashCode((int)0, (int)0, (int)0, ((string)"PropertyDeclarationSyntax").GetHashCode());
            dict.Add(c, Load(WinExplorers.ve.Property_16x));
            c = GetHashCode((int)0, (int)0, (int)0, ((string)"FieldDeclarationSyntax").GetHashCode());
            dict.Add(c, Load(WinExplorers.ve.Field_blue_16x));
            c = GetHashCode((int)0, (int)0, (int)0, ((string)"EventDeclarationSyntax").GetHashCode());
            dict.Add(c, Load(WinExplorers.ve.Event_orange_16x));

            //c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Structure, 0, 0);
            //dict.Add(c, Load(WinExplorers.ve.Structure_16x));
            //c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Structure, (int)Accessibility.Protected, 0);
            //dict.Add(c, Load(WinExplorers.ve.StructureProtect_16x));
            //c = GetHashCode((int)SymbolKind.NamedType, (int)TypeKind.Structure, (int)Accessibility.Private, 0);
            //dict.Add(c, Load(WinExplorers.ve.StructurePrivate_16x));

            c = GetHashCode((int)SymbolKind.Local, 0, 0, 0);
            dict.Add(c, Load(WinExplorers.ve.LocalVariable_16x));

            c = GetHashCode((int)SymbolKind.Parameter, 0, 0, 0);
            dict.Add(c, Load(WinExplorers.ve.LocalVariable_16x));
        }

        public static System.Windows.Controls.Image Load(System.Drawing.Bitmap b)
        {
            return GetImageExt(GetImageSource(b));
        }

        public static Dictionary<int, System.Windows.Controls.Image> dict { get; set; }

        public void AddStandards()
        {
            std = new List<object>();
            string[] items = { "Keywords", "Snippets" };

            CsCompletionData d = new CsCompletionData(items[0]);
            d.Image = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.KeywordSnippet_16x);
            d.Search = "";
            d.obs = "Keywords";
            std.Add(d);
            d = new CsCompletionData(items[1]);
            d.Image = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Snippet_16x);
            d.Search = "";
            d.obs = "Snippets";
            std.Add(d);
        }

        public class ConvertBitmapToBitmapImage
        {
            /// <summary>
            /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
            /// </summary>
            /// <param name="src">A bitmap image</param>
            /// <returns>The image as a BitmapImage for WPF</returns>
            static public BitmapImage Convert(Bitmap src)
            {
                MemoryStream ms = new MemoryStream();
                ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage image = new BitmapImage();
                image.BeginInit();

                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.DecodePixelWidth = 16;
                image.EndInit();
                return image;
            }
        }

        static public System.Windows.Controls.Image GetImage()
        {
            BitmapImage s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_256x);
            System.Windows.Controls.Image b = new System.Windows.Controls.Image();
            b.Width = 16;
            b.Height = 16;
            b.Stretch = Stretch.UniformToFill;
            b.Source = s;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.Margin = new Thickness(2, 2, 2, 2);
            return b;
        }

        static public System.Windows.Controls.Image GetImage(int hc)
        {
            System.Windows.Controls.Image b = new System.Windows.Controls.Image();

            if (dict.ContainsKey(hc))
                b.Source = dict[hc].Source;
            else b.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_16x);

            b.Width = 16;
            b.Height = 16;
            b.Stretch = Stretch.UniformToFill;

            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.Margin = new Thickness(2, 2, 2, 2);
            return b;
        }

        static public System.Windows.Controls.Image GetImageData(object d)
        {
            //BitmapImage s = GetImageSource(d) as BitmapImage;
            //System.Windows.Controls.Image b = new System.Windows.Controls.Image();
            //b.Width = 16;
            //b.Height = 16;
            //b.Stretch = Stretch.UniformToFill;
            //b.Source = s;
            //b.HorizontalAlignment = HorizontalAlignment.Left;
            //b.Margin = new Thickness(2, 2, 2, 2);
            //return b;

            System.Windows.Controls.Image b = new System.Windows.Controls.Image();

            int hc = GetHashCode(d);

            if (dict.ContainsKey(hc))
                b.Source = dict[hc].Source;
            else b.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_16x);

            b.Width = 16;
            b.Height = 16;
            b.Stretch = Stretch.UniformToFill;

            b.HorizontalAlignment = HorizontalAlignment.Left;
            //   b.Margin = new Thickness(2, 2, 2, 2);
            return b;
        }

        static public System.Windows.Controls.Image GetImageExt(ImageSource s)
        {
            //BitmapImage s = d as BitmapImage;
            System.Windows.Controls.Image b = new System.Windows.Controls.Image();
            b.Width = 16;
            b.Height = 16;
            b.Stretch = Stretch.UniformToFill;
            b.Source = s;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.Margin = new Thickness(2, 2, 2, 2);
            return b;
        }

        static public System.Windows.Media.ImageSource GetImageSource()
        {
            BitmapImage s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_256x);
            return s;
        }

        static public System.Windows.Media.ImageSource GetImageSource(System.Drawing.Bitmap b)
        {
            return ConvertBitmapToBitmapImage.Convert(b);
        }

        private List<object> std = new List<object>();

        public void LoadStacks(List<object> register = null)
        {
            if (register == null)
            {
                System.Windows.Controls.Image b = GetImage();
                //stacks.Children.Add(b);
                b = GetImage();
                //stacks.Children.Add(b);
            }
            else
            {
                int i = 0;
                stacks.Children.Clear();
                foreach (var c in register)
                {
                    if (i > 7)
                        break;
                    System.Windows.Controls.Image b = GetImage((int)c);
                    System.Windows.Controls.Primitives.ToggleButton button = new ToggleButton();
                    button.Content = b;
                    button.BorderThickness = new Thickness(0, 0, 0, 0);
                    button.Click += Button_Click;

                    button.Tag = c;
                    stacks.Children.Add(button);
                    i++;
                }
                foreach (var c in std)
                {
                    if (i > 7)
                        break;
                    CsCompletionData d = c as CsCompletionData;
                    System.Windows.Controls.Image b = GetImageData(d.obs);
                    System.Windows.Controls.Primitives.ToggleButton button = new ToggleButton();
                    button.Content = b;
                    button.BorderThickness = new Thickness(0, 0, 0, 0);
                    button.Click += Button_Click;

                    button.Tag = c;
                    stacks.Children.Add(button);
                    i++;

                    //System.Windows.Controls.Image b = GetImageExt(d.Image as ImageSource);
                    //System.Windows.Controls.Primitives.ToggleButton button = new ToggleButton();
                    //button.Content = b;
                    //button.BorderThickness = new Thickness(0, 0, 0, 0);
                    //button.Click += Button_Click;

                    //button.Tag = c;
                    //stacks.Children.Add(button);
                }
            }
        }

        private bool allitems = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            object obs = b.Tag;
            if (obs == null)
                return;

            if (b.IsChecked == true)
                FilterByKind(obs, true);
            else
                FilterByKind(obs, false);
            allitems = false;
        }

        public void FilterByKind(object obs, bool add = true)
        {
            ICSharpCode.AvalonEdit.CodeCompletion.CompletionListBox c = completionList.ListBox;
            var b = completionList.Master;
            List<CsCompletionData> d = new List<CsCompletionData>();
            if (add)
            {
                if (allitems == false)
                    foreach (var bc in c.Items)
                        d.Add(bc as CsCompletionData);
                for (int i = 0; i < completionList.Master.Count; i++)
                {
                    CsCompletionData dd = (CsCompletionData)completionList.Master[i];
                    if (CompletionControls.GetHashCode(dd.obs) == (int)obs)
                        d.Add(dd);
                    //if (obs is SymbolKind)
                    //{
                    //    SymbolKind s = (SymbolKind)obs;
                    //    if (dd.obs is SymbolKind)
                    //        if (s == (SymbolKind)dd.obs)
                    //            d.Add(dd);

                    //}
                    //else if (obs is TypeKind)
                    //{
                    //    TypeKind s = (TypeKind)obs;
                    //    if (dd.obs is TypeKind)
                    //        if (s == (TypeKind)dd.obs)
                    //            d.Add(dd);
                    //} else if (obs is CsCompletionData)
                    //{
                    //    string s = ((CsCompletionData)obs).obs as string;
                    //    if (dd.obs is string)
                    //        if ( s == (string)dd.obs)
                    //            d.Add(dd);
                    //}
                }
            }
            else
            {
                foreach (var bc in c.Items)
                    d.Add(bc as CsCompletionData);
                for (int i = 0; i < c.Items.Count; i++)
                {
                    CsCompletionData dd = (CsCompletionData)c.Items[i];
                    if (CompletionControls.GetHashCode(dd.obs) == (int)obs)
                        d.Remove(dd);
                    //if (obs is SymbolKind)
                    //{
                    //    SymbolKind s = (SymbolKind)obs;

                    //    if (dd.obs is SymbolKind)
                    //        if (s == (SymbolKind)dd.obs)
                    //            d.Remove(dd);

                    //}
                    //else if (obs is TypeKind)
                    //{
                    //    TypeKind s = (TypeKind)obs;
                    //    if (dd.obs is TypeKind)
                    //        if (s == (TypeKind)dd.obs)
                    //            d.Remove(dd);
                    //}
                    //if (obs is CsCompletionData)
                    //{
                    //    string s = ((CsCompletionData)obs).obs as string;
                    //    if (dd.obs is string)
                    //        if (s == (string)dd.obs)
                    //            d.Remove(dd);
                    //}
                }
            }

            c.ItemsSource = d;
        }

        private static Func<IEnumerable<int?>, int> computeHashCode = items =>
      items
      .Select(item => item == null ? 0 : item.GetHashCode())
      .Aggregate(23, (hash, itemHash) => hash * 31 + itemHash);

        static public int GetHashCode(object obs)
        {
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            if (obs is ISymbol)
            {
                ISymbol ts = (ISymbol)obs;
                a = (int)ts.Kind;
                c = (int)ts.DeclaredAccessibility;
            }
            if (obs is ITypeSymbol)
            {
                ITypeSymbol ts = (ITypeSymbol)obs;
                b = (int)ts.TypeKind;
            }
            if (obs is string)
            {
                string s = (string)obs;
                d = s.GetHashCode();
            }
            if (obs is NamespaceDeclarationSyntax)
            {
                string s = (string)("NamespaceDeclarationSyntax");
                d = s.GetHashCode();
            }
            else if (obs is MethodDeclarationSyntax)
            {
                string s = (string)("MethodDeclarationSyntax");
                d = s.GetHashCode();
            }
            else if (obs is PropertyDeclarationSyntax)
            {
                string s = (string)("PropertyDeclarationSyntax");
                d = s.GetHashCode();
            }
            else if (obs is FieldDeclarationSyntax)
            {
                string s = (string)("FieldDeclarationSyntax");
                d = s.GetHashCode();
            }
            else if (obs is EventDeclarationSyntax)
            {
                string s = (string)("EventDeclarationSyntax");
                d = s.GetHashCode();
            }
            int?[] e = { a, b, c, d };
            return computeHashCode(e);
        }

        private static int GetHashCode(int a, int b, int c, int d)
        {
            int?[] e = { a, b, c, d };
            return computeHashCode(e);
        }

        static public System.Windows.Media.ImageSource GetImageSource(object symbol, List<object> register = null)
        {
            System.Windows.Controls.Image s = null;
            int hc = GetHashCode(symbol);
            if (dict.ContainsKey(hc))
                s = dict[hc];
            else s = Load(WinExplorers.ve.Class_yellow_16x);

            if (register != null)
                if (!register.Contains(hc))
                    register.Add(hc);
            return s.Source;
        }

        static public System.Windows.Media.ImageSource GetImageSource(object symbol)
        {
            BitmapImage s = null;
            if (symbol is TypeKind)
            {
                TypeKind t = (TypeKind)symbol;
                switch (t)
                {
                    case TypeKind.Class:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_16x);
                        return s;

                    case TypeKind.Interface:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Interface_blue_16x);
                        return s;

                    case TypeKind.Enum:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Enumerator_orange_16x);
                        return s;

                    case TypeKind.Delegate:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Delegate_16x);
                        return s;

                    case TypeKind.Struct:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Structure_16x);
                        return s;

                    default:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Property_16x);
                        return s;
                }
            }
            else if (symbol is SymbolKind)
                switch (((SymbolKind)symbol))
                {
                    case SymbolKind.Namespace:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Namespace_16x);
                        return s;

                    case SymbolKind.Method:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Method_purple_16x);
                        return s;

                    case SymbolKind.Property:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Property_16x);
                        return s;

                    case SymbolKind.Field:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Field_blue_16x);
                        return s;

                    case SymbolKind.Event:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Event_orange_16x);
                        return s;

                    case SymbolKind.Local:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.LocalVariable_16x);
                        return s;

                    case SymbolKind.Parameter:
                        s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.LocalVariable_16x);
                        return s;
                }
            s = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Class_yellow_256x);
            return s;
        }

        private class Accesibillity
        {
        }
    }
}