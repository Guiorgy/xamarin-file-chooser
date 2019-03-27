using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;
using String = Java.Lang.String;

namespace App
{
    [Activity(Label = "@string/title_activity_about", Theme = "@style/AppTheme.NoActionBar", Icon = "@mipmap/ic_launcher", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "App.MainActivity")]
    public class AboutActivity : AppCompatActivity
    {
        private Toolbar toolbar;
        private FloatingActionButton fab;
        private RecyclerView recyclerView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_about);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (s, e) =>
            {
                Snackbar.Make(s as View, "Replace with your own action", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
            };

            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(linearLayoutManager);
            recyclerView.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Horizontal));
            recyclerView.SetItemAnimator(new DefaultItemAnimator());
            //recyclerView.SetAdapter(MainAdapter(this@AboutActivity, aboutItems));
        }

        private class MainAdapter : RecyclerView.Adapter
        {
            private readonly AppCompatActivity context;
            private readonly List<Items.Item> plainItems = new List<Items.Item>();

            internal MainAdapter(AppCompatActivity context, List<Items> items)
            {
                this.context = context;
                foreach (Items it in items)
                {
                    if (it.items.IsNotEmpty())
                    {
                        it.items[0].catalog = it.title;
                    }
                    plainItems.AddRange(it.items);
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                return new ViewHolder(LayoutInflater.From(context).Inflate(Resource.Layout.li_about_item, parent, false),
                    (view, holder) =>
                    {
                        if (holder.mValueView.Tag != null && (holder.mValueView.Tag is String))
                        {
                            string link = (holder.mValueView.Tag as String).ToString();
                            if (link.StartsWith("mailto:"))
                            {
                                context.StartActivity(new Intent(Intent.ActionSendto, Uri.Parse(link)));
                            }
                            else if (link.StartsWith("tel:"))
                            {
                                context.StartActivity(new Intent(Intent.ActionDial, Uri.Parse(link)));
                            }
                            else if (link.StartsWith("market:"))
                            {
                                Intent intent = new Intent(Intent.ActionDial, Uri.Parse(link));
                                intent.AddFlags(ActivityFlags.NoHistory | ActivityFlags.NewDocument | ActivityFlags.MultipleTask);
                                try
                                {
                                    context.StartActivity(intent);
                                }
                                catch (ActivityNotFoundException)
                                {
                                    context.StartActivity(new Intent(Intent.ActionView,
                                        Uri.Parse("http://play.google.com/store/apps/details?id=" + context.PackageName)));
                                }
                            }
                            else
                            {
                                context.StartActivity(new Intent(Intent.ActionView, Uri.Parse(link)));
                            }
                        }
                    });
            }

            public override int ItemCount => plainItems.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var it = plainItems[position];
                var vh = holder as ViewHolder;

                vh.mTitleView.Text = it.title;
                vh.mSubTitleView.Text = it.subTitle;
                vh.mValueView.Text = it.value;

                if (it.subTitle.IsBlank())
                {
                    vh.mSubTitleView.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.mSubTitleView.Visibility = ViewStates.Visible;
                }

                if (it.catalog == null || it.catalog.IsBlank())
                {
                    vh.mCatalogView.Text = it.catalog;
                    vh.mCatalogView.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.mCatalogView.Visibility = ViewStates.Gone;
                }

                vh.mValueView.Tag = it.valueLink;
            }

            private class ViewHolder : RecyclerView.ViewHolder
            {

                internal TextView mTitleView;
                internal TextView mSubTitleView;
                internal TextView mValueView;
                internal TextView mCatalogView;
                internal ImageView mIconView;

                public ViewHolder(View view, Action<View, ViewHolder> click) : base(view)
                {
                    mTitleView = view.FindViewById<TextView>(Resource.Id.title);
                    mSubTitleView = view.FindViewById<TextView>(Resource.Id.sub_title);
                    mValueView = view.FindViewById<TextView>(Resource.Id.value);
                    mCatalogView = view.FindViewById<TextView>(Resource.Id.catalog);
                    mIconView = view.FindViewById<ImageView>(Resource.Id.icon);

                    View row = view.FindViewById<View>(Resource.Id.row);
                    if (row != null)
                    {
                        row.Click += (s, e) =>
                        {
                            click(view, this);
                        };
                    }
                }
            }
        }

        private static readonly List<Items> aboutItems = new List<Items>()
        {
            new Items("Information", new List<Items.Item>()
            {
                new Items.Item("Homepage", "Goto", "https://github.com/hedzr/android-file-chooser"),
                new Items.Item("Issues", "Report to us", "https://github.com/hedzr/android-file-chooser/issues/new"),
                new Items.Item("License", "Apache 2.0", "https://github.com/hedzr/android-file-chooser/blob/master/LICENSE"),
                new Items.Item("Rate me", "Like!", "market://details?id=" + "com.obsez.android.lib.filechooser"),
            }),
            new Items("Credits", new List<Items.Item>()
            {
                new Items.Item("Hedzr Yeh", "Email", "mailto:hedzrz@gmail.com"),
                new Items.Item("Guiorgy Potskhishvili", "Email", "mailto:guiorgy123@gmail.com"),
                new Items.Item("More Contributors", "Goto", "https://github.com/hedzr/android-file-chooser#Acknowledges", "and supporters"),
            }),
        };

        private class Items
        {
            internal string title;
            internal List<Item> items = new List<Item>();

            internal Items(string title, List<Item> items)
            {
                this.title = title;
                this.items.AddRange(items);
            }

            internal class Item
            {
                internal string title;
                internal string value;
                internal string valueLink;
                internal string subTitle;
                internal string catalog;

                public Item(string title, string value, string valueLink = "", string subTitle = "", string catalog = null)
                {
                    this.title = title;
                    this.value = value;
                    this.valueLink = valueLink;
                    this.subTitle = subTitle;
                    this.catalog = catalog;
                }
            }
        }
    }

    public static class Extension
    {
        public static bool IsNotEmpty<T>(this IList<T> list)
        {
            return list.Count != 0;
        }

        public static bool IsBlank(this string str)
        {
            return str.Trim().Length == 0;
        }
    }
}