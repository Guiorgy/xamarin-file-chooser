using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Obsez.Android.Lib.Filechooser.Tool;
using Java.IO;
using System;
using static Com.Obsez.Android.Lib.Filechooser.ChooserDialog;
using static Com.Obsez.Android.Lib.Filechooser.Tool.DirAdapter;
using Object = Java.Lang.Object;

namespace Com.Obsez.Android.Lib.Filechooser
{
    public static class Listeners
    {
        public class Result : Object, IResult
        {
            protected Action<string, File> result;

            public Result(Action<string, File> result)
            {
                this.result = result;
            }

            public void OnChoosePath(string dir, File dirFile)
            {
                result(dir, dirFile);
            }
        }

        public static ChooserDialog WithChosenListener(this ChooserDialog dialog, Action<string, File> result)
        {
            return dialog.WithChosenListener(new Result(result));
        }

        public class AdapterSetter : Object, IAdapterSetter
        {
            protected Action<DirAdapter> apply;

            public AdapterSetter(Action<DirAdapter> apply)
            {
                this.apply = apply;
            }

            public void Apply(DirAdapter adapter)
            {
                apply(adapter);
            }
        }

        public static ChooserDialog WithAdapterSetter(this ChooserDialog dialog, Action<DirAdapter> apply)
        {
            return dialog.WithAdapterSetter(new AdapterSetter(apply));
        }

        public class GetView : Object, IGetView
        {
            protected Func<File, bool, bool, View, ViewGroup, LayoutInflater, View> getView;

            public GetView(Func<File, bool, bool, View, ViewGroup, LayoutInflater, View> getView)
            {
                this.getView = getView;
            }

            View IGetView.GetView(File file, bool isSelected, bool isFocused, View convertView, ViewGroup parent, LayoutInflater inflater)
            {
                return getView(file, isSelected, isFocused, convertView, parent, inflater);
            }
        }

        public static void OverrideGetView(this DirAdapter adapter, Func<File, bool, bool, View, ViewGroup, LayoutInflater, View> getView)
        {
            adapter.OverrideGetView(new GetView(getView));
        }

        public class CanNavigateUp : Object, ICanNavigateUp
        {
            protected Predicate<File> canUpTo;

            public CanNavigateUp(Predicate<File> canUpTo)
            {
                this.canUpTo = canUpTo;
            }

            public bool CanUpTo(File file)
            {
                return canUpTo(file);
            }
        }

        public static ChooserDialog WithNavigateUpTo(this ChooserDialog dialog, Predicate<File> canUpTo)
        {
            return dialog.WithNavigateUpTo(new CanNavigateUp(canUpTo));
        }

        public class CanNavigateTo : Object, ICanNavigateTo
        {
            protected Predicate<File> canNavigate;

            public CanNavigateTo(Predicate<File> canNavigate)
            {
                this.canNavigate = canNavigate;
            }

            public bool CanNavigate(File file)
            {
                return canNavigate(file);
            }
        }

        public static ChooserDialog WithNavigateTo(this ChooserDialog dialog, Predicate<File> canNavigate)
        {
            return dialog.WithNavigateTo(new CanNavigateTo(canNavigate));
        }

        public class OnBackPressed : Object, IOnBackPressedListener
        {
            protected Action<AlertDialog> onBackPressed;

            public OnBackPressed(Action<AlertDialog> onBackPressed)
            {
                this.onBackPressed = onBackPressed;
            }

            void IOnBackPressedListener.OnBackPressed(AlertDialog alertDialog)
            {
                onBackPressed(alertDialog);
            }
        }

        public static ChooserDialog WithOnBackPressedListener(this ChooserDialog dialog, Action<AlertDialog> onBackPressed)
        {
            return dialog.WithOnBackPressedListener(new OnBackPressed(onBackPressed));
        }

        public static ChooserDialog WithOnLastBackPressedListener(this ChooserDialog dialog, Action<AlertDialog> onBackPressed)
        {
            return dialog.WithOnLastBackPressedListener(new OnBackPressed(onBackPressed));
        }

        public class CustomizePathViewImp : Object, ICustomizePathView
        {
            protected Action<TextView> customize;

            public CustomizePathViewImp(Action<TextView> customize)
            {
                this.customize = customize;
            }

            public void Customize(TextView textView)
            {
                customize(textView);
            }
        }

        public static ChooserDialog CustomizePathView(this ChooserDialog dialog, Action<TextView> customize)
        {
            return dialog.CustomizePathView(new CustomizePathViewImp(customize));
        }

        public class NegativeButtonListener : Object, IDialogInterfaceOnClickListener
        {
            protected Action<IDialogInterface, int> onClick;

            public NegativeButtonListener(Action<IDialogInterface, int> onClick)
            {
                this.onClick = onClick;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                onClick(dialog, which);
            }
        }

        public static ChooserDialog WithNegativeButton(this ChooserDialog dialog, int cancelTitle, Action<IDialogInterface, int> onClick)
        {
            return dialog.WithNegativeButton(cancelTitle, new NegativeButtonListener(onClick));
        }

        public static ChooserDialog WithNegativeButtonListener(this ChooserDialog dialog, Action<IDialogInterface, int> onClick)
        {
            return dialog.WithNegativeButtonListener(new NegativeButtonListener(onClick));
        }

        public class CancellListener : Object, IDialogInterfaceOnCancelListener
        {
            protected Action<IDialogInterface> onCancel;

            public CancellListener(Action<IDialogInterface> onCancel)
            {
                this.onCancel = onCancel;
            }

            public void OnCancel(IDialogInterface dialog)
            {
                onCancel(dialog);
            }
        }

        public static ChooserDialog WithOnCancelListener(this ChooserDialog dialog, Action<IDialogInterface> onCancel)
        {
            return dialog.WithOnCancelListener(new CancellListener(onCancel));
        }

        public class DismissListener : Object, IDialogInterfaceOnDismissListener
        {
            protected Action<IDialogInterface> onDismiss;

            public DismissListener(Action<IDialogInterface> onDismiss)
            {
                this.onDismiss = onDismiss;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                onDismiss(dialog);
            }
        }

        public static ChooserDialog WithOnDismissListener(this ChooserDialog dialog, Action<IDialogInterface> onDismiss)
        {
            return dialog.WithOnDismissListener(new DismissListener(onDismiss));
        }
    }
}