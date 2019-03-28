using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Obsez.Android.Lib.Filechooser;
using Java.IO;
using System.Collections.Generic;
using static Com.Obsez.Android.Lib.Filechooser.Listeners;

namespace App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Icon = "@mipmap/ic_launcher", MainLauncher = true)]
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
        private CheckBox darkTheme;
        private CheckBox customLayout;

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
            customLayout = FindViewById<CheckBox>(Resource.Id.checkbox_custom_layout);
            darkTheme = FindViewById<CheckBox>(Resource.Id.checkbox_dark_theme);

            titleFollowsDir.CheckedChange += ((s, e) =>
            {
                if (e.IsChecked)
                {
                    customLayout.Checked = false;
                }
            });

            displayPath.CheckedChange += ((s, e) =>
            {
                if (e.IsChecked)
                {
                    customLayout.Checked = false;
                }
            });

            dateFormat.CheckedChange += ((s, e) =>
            {
                if (e.IsChecked)
                {
                    customLayout.Checked = false;
                }
            });

            customLayout.CheckedChange += ((s, e) =>
            {
                if (e.IsChecked)
                {
                    dateFormat.Checked = false;
                    darkTheme.Checked = false;
                    titleFollowsDir.Checked = false;
                    displayPath.Checked = false;
                }
            });

            FindViewById(Resource.Id.btn_show_dialog).Click += OnClick;

            displayPath.Checked = true;
        }

        public void OnClick(object sender, System.EventArgs e)
        {
            //choose a file
            Context ctx = this;
            if (ctx == null)
            {
                return;
            }

            List<File> files = new List<File>();

            ChooserDialog chooserDialog;
            if (darkTheme.Checked)
            {
                chooserDialog = new ChooserDialog(ctx, Resource.Style.FileChooserStyle_Dark);
            }
            else
            {
                chooserDialog = new ChooserDialog(ctx);
            }


            chooserDialog
                .WithResources(dirOnly.Checked ? Resource.String.title_choose_folder : Resource.String.title_choose_file,
                    Resource.String.title_choose, Resource.String.dialog_cancel)
                .WithOptionResources(Resource.String.option_create_folder, Resource.String.options_delete,
                    Resource.String.new_folder_cancel, Resource.String.new_folder_ok)
                // Optionally, you can use Strings instead:
                /*.WithStringResources(dirOnly.isChecked() ? "Choose a folder" : "Choose a file",
                    "Choose", "Cancel")
                .WithOptionStringResources("New folder",
                    "Delete", "Cancel", "Ok")*/
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
                    .WithOnDismissListener((AlertDialog) =>
                    {
                        if (files.Count == 0)
                        {
                            return;
                        }

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
                    })
                    .WithOnLastBackPressedListener((alertDialog) =>
                    {
                        files.Clear();
                        chooserDialog.Dismiss();
                    })
                    .WithNegativeButtonListener((alertDialog, which) =>
                    {
                        files.Clear();
                        chooserDialog.Dismiss();
                    })
                    .WithChosenListener((dir, dirFile) =>
                    {
                        if (continueFromLast.Checked)
                        {
                            _path = dir;
                        }
                        if (dirFile.IsDirectory)
                        {
                            chooserDialog.Dismiss();
                            return;
                        }
                        if (!files.Remove(dirFile))
                        {
                            files.Add(dirFile);
                        }
                    });
                }
                else
                {
                    // OnDismissListener is not supported, so we simulate something similar anywhere where the
                    // dialog might be dismissed.
                    void OnDismis()
                    {
                        if (files.Count == 0)
                        {
                            return;
                        }

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
                            files.Clear();
                            dialog.Dismiss();
                            OnDismis();
                        })
                    .WithNegativeButtonListener((dialog, which) =>
                    {
                        files.Clear();
                        dialog.Dismiss();
                        OnDismis();
                    })
                    .WithChosenListener((dir, dirFile) =>
                    {
                        if (continueFromLast.Checked)
                        {
                            _path = dir;
                        }
                        if (dirFile.IsDirectory)
                        {
                            chooserDialog.Dismiss();
                            OnDismis();
                            return;
                        }
                        if (!files.Remove(dirFile))
                        {
                            files.Add(dirFile);
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
                    Toast.MakeText(ctx, (dirFile.IsDirectory ? "FOLDER: " : "FILE: ") + dir,
                        ToastLength.Short).Show();
                    _tv.Text = dir;
                    if (dirFile.IsFile)
                    {
                        // Not implemented
                        //_iv.SetImageBitmap(ImageUtil.decodeFile(dirFile));
                    }
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            MenuInflater.Inflate(Resource.Menu.menu_choose_file, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle action bar item clicks here. The action bar will
            // automatically handle clicks on the Home/Up button, so long
            // as you specify a parent activity in AndroidManifest.xml.
            if (item.ItemId == Resource.Id.action_about)
            {
                StartActivity(new Intent(this, typeof(AboutActivity)));
                return true;
            }
            if (item.ItemId == Resource.Id.action_gh)
            {
                StartActivity(
                    new Intent(Intent.ActionView, Uri.Parse("https://github.com/hedzr/android-file-chooser")));
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}