using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Com.Obsez.Android.Lib.Filechooser;
using Java.IO;
using System.Collections.Generic;
using static Com.Obsez.Android.Lib.Filechooser.ChooserDialog;

namespace App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private CheckBox disableTitle;
        private CheckBox EnableOptions;
        private CheckBox titleFollowsDir;
        private CheckBox EnableMultiple;
        private CheckBox displayPath;
        private CheckBox dirOnly;
        private CheckBox allowHidden;
        private CheckBox continueFromLast;
        private CheckBox filterImages;
        private CheckBox displayIcon;
        private CheckBox dateFormat;

        private static string _path = null;
        private TextView _tv;
        private ImageView _iv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _tv = FindViewById<TextView>(Resource.Id.textView);
            _tv.Text = BuildConfig.VersionName;
            _iv = FindViewById<ImageView>(Resource.Id.imageView);

            disableTitle = FindViewById<CheckBox>(Resource.Id.checkbox_disable_title);
            EnableOptions = FindViewById<CheckBox>(Resource.Id.checkbox_enable_options);
            titleFollowsDir = FindViewById<CheckBox>(Resource.Id.checkbox_title_follow_dir);
            EnableMultiple = FindViewById<CheckBox>(Resource.Id.checkbox_enable_multiple);
            displayPath = FindViewById<CheckBox>(Resource.Id.checkbox_display_path);
            dirOnly = FindViewById<CheckBox>(Resource.Id.checkbox_dir_only);
            allowHidden = FindViewById<CheckBox>(Resource.Id.checkbox_allow_hidden);
            continueFromLast = FindViewById<CheckBox>(Resource.Id.checkbox_continue_from_last);
            filterImages = FindViewById<CheckBox>(Resource.Id.checkbox_filter_images);
            displayIcon = FindViewById<CheckBox>(Resource.Id.checkbox_display_icon);
            dateFormat = FindViewById<CheckBox>(Resource.Id.checkbox_date_format);

            FindViewById(Resource.Id.btn_show_dialog).Click += OnClick;

            displayPath.Checked = true;
        }

        class DismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            private Context ctx;
            private List<File> files;

            public DismissListener(Context ctx, List<File> files)
            {
                this.ctx = ctx;
                this.files = files;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                if (files.Count == 0) return;

                List<string> paths = new List<string>();
                foreach (File file in files)
                {
                    paths.Add(file.AbsolutePath);
                }

                new Android.App.AlertDialog.Builder(ctx)
                    .SetTitle(files.Count + " files selected:")
                    .SetAdapter(new ArrayAdapter<string>(ctx, Android.Resource.Layout.SimpleExpandableListItem1, paths), (IDialogInterfaceOnClickListener) null)
                    .Create()
                    .Show();
            }
        }

        class LastBackListener : Java.Lang.Object, IOnBackPressedListener
        {
            private List<File> files;

            public LastBackListener(List<File> files)
            {
                this.files = files;
            }

            public void OnBackPressed(Android.App.AlertDialog dialog)
            {
                files.Clear();
                dialog.Dismiss();
            }
        }

        class NegativeClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            private List<File> files;

            public NegativeClickListener(List<File> files)
            {
                this.files = files;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                files.Clear();
                dialog.Dismiss();
            }
        }

        class Result : Java.Lang.Object, IResult
        {
            private ChooserDialog dialog;
            private bool continueFromLast;
            private List<File> files;

            public Result(ChooserDialog dialog, bool continueFromLast, List<File> files)
            {
                this.dialog = dialog;
                this.continueFromLast = continueFromLast;
                this.files = files;
            }

            public void OnChoosePath(string dir, File dirFile)
            {
                if (continueFromLast)
                {
                    _path = dir;
                }
                if (dirFile.IsDirectory)
                {
                    dialog.Dismiss();
                    return;
                }
                if (!files.Remove(dirFile))
                {
                    files.Add(dirFile);
                }
            }
        }

        public void OnClick(object sender, System.EventArgs e)
        {
            //choose a file
            Context ctx = this;
            List<File> files = new List<File>();

            ChooserDialog chooserDialog = new ChooserDialog(ctx)
                .WithResources(dirOnly.Checked ? Resource.String.title_choose_folder : Resource.String.title_choose_file,
                    Resource.String.title_choose, Resource.String.dialog_cancel)
                .WithOptionResources(Resource.String.option_create_folder, Resource.String.options_delete,
                    Resource.String.new_folder_cancel, Resource.String.new_folder_ok)
                .DisableTitle(disableTitle.Checked)
                .EnableOptions(EnableOptions.Checked)
                .TitleFollowsDir(titleFollowsDir.Checked)
                .DisplayPath(displayPath.Checked);

            if (filterImages.Checked)
            {
                // Most common image file extensions (source: http://preservationtutorial.library.cornell
                // .edu/presentation/table7-1.html)
                chooserDialog.WithFilter(dirOnly.Checked,
                    allowHidden.Checked,
                    "tif", "tiff", "gif", "jpeg", "jpg", "jif", "jfif",
                    "jp2", "jpx", "j2k", "j2c", "fpx", "pcd", "png", "pdf");
            }
            else
            {
                chooserDialog.WithFilter(dirOnly.Checked, allowHidden.Checked);
            }

            if (EnableMultiple.Checked)
            {
                chooserDialog.EnableMultiple(true);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                {
                    chooserDialog
                    .WithOnDismissListener(new DismissListener(ctx, files))
                    .WithOnLastBackPressedListener(new LastBackListener(files))
                    .WithNegativeButtonListener(new NegativeClickListener(files))
                    .WithChosenListener(new Result(chooserDialog, continueFromLast.Checked, files));
                }
                else
                {
                    // OnDismissListener is not supported, so we simulate something similar anywhere where the
                    // dialog might be dismissed.
                    void OnDismis()
                    {
                        if (files.Count == 0) return;

                        List<string> paths = new List<string>();
                        foreach (File file in files)
                        {
                            paths.Add(file.AbsolutePath);
                        }

                        new Android.App.AlertDialog.Builder(ctx)
                            .SetTitle(files.Count + " files selected:")
                            .SetAdapter(new ArrayAdapter<string>(ctx, Android.Resource.Layout.SimpleExpandableListItem1, paths), (IDialogInterfaceOnClickListener)null)
                            .Create()
                            .Show();
                    };

                    chooserDialog
                        .WithOnLastBackPressedListener(dialog =>
                        {
                            files.clear();
                            dialog.dismiss();
                            onDismiss.run();
                        })
                    .WithNegativeButtonListener((dialog, which) =>
                    {
                        files.clear();
                        dialog.dismiss();
                        onDismiss.run();
                    })
                    .WithChosenListener((dir, dirFile) =>
                    {
                        if (continueFromLast.Checked)
                        {
                            _path = dir;
                        }
                        if (dirFile.isDirectory())
                        {
                            chooserDialog.dismiss();
                            onDismiss.run();
                            return;
                        }
                        if (!files.remove(dirFile))
                        {
                            files.add(dirFile);
                        }
                    });
                }
            }
            else
            {
                chooserDialog.WithChosenListener((dir, dirFile) =>
                {
                    if (continueFromLast.Checked)
                    {
                        _path = dir;
                    }
                    Toast.makeText(ctx, (dirFile.isDirectory() ? "FOLDER: " : "FILE: ") + dir,
                        Toast.LENGTH_SHORT).show();
                    _tv.setText(dir);
                    if (dirFile.isFile()) _iv.setImageBitmap(ImageUtil.decodeFile(dirFile));
                });
            }
            if (continueFromLast.Checked && _path != null)
            {
                chooserDialog.WithStartFile(_path);
            }
            if (displayIcon.Checked)
            {
                chooserDialog.WithIcon(Resource.Mipmap.ic_launcher);
            }
            if (dateFormat.Checked)
            {
                chooserDialog.WithDateFormat("dd MMMM yyyy");
            }

            chooserDialog.Build().Show();
        }
    }
}