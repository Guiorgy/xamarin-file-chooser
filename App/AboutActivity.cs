using Android.App;
using Android.OS;

namespace App
{
    [Activity(Label = "@string/title_activity_about", Theme = "@style/AppTheme.NoActionBar", Icon = "@mipmap/ic_launcher", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "App.MainActivity")]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}