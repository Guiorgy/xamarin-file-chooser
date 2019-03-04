# xamarin-file-chooser binding of android-file-chooser 1.1.16

### android-file-chooser
[![Android Arsenal](https://img.shields.io/badge/Android%20Arsenal-android--file--chooser-brightgreen.svg?style=flat)](https://android-arsenal.com/details/1/6982)
[![Download](https://api.bintray.com/packages/hedzr/maven/filechooser/images/download.svg)](https://bintray.com/hedzr/maven/filechooser/_latestVersion)
[![Release](https://jitpack.io/v/hedzr/android-file-chooser.svg)](https://jitpack.io/#hedzr/android-file-chooser)

### Snapshots

<table><tr><td>
<img src="https://raw.githubusercontent.com/hedzr/android-file-chooser/master/captures/choose_file.png" width="360"/>
</td><td>
<img src="https://raw.githubusercontent.com/hedzr/android-file-chooser/master/captures/choose_folder.png" width="360"/>
</td><td>
<img src="https://user-images.githubusercontent.com/27736965/53578348-f1d35880-3b90-11e9-9ef4-7ed0276ca603.gif" width="360"/>
</td></tr></table>

More images (beyond v1.1.16) can be found at [Gallery](https://github.com/hedzr/android-file-chooser/wiki/Gallery)

### Demo Application

A demo-app of the original can be installed from [Play Store](https://play.google.com/store/apps/details?id=com.obsez.android.lib.filechooser.demo).

<a href='https://play.google.com/store/apps/details?id=com.obsez.android.lib.filechooser.demo&pcampaignid=MKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1'><img alt='Get it on Google Play' width='240' src='https://play.google.com/intl/en_us/badges/images/generic/en_badge_web_generic.png'/></a>

## Usage

```cs
using Com.Obsez.Android.Lib.Filechooser;
using static Com.Obsez.Android.Lib.Filechooser.Listeners;

ChooserDialog chooserDialog = new ChooserDialog(context)
    .WithResources(dirOnly.Checked ? Resource.String.title_choose_folder : Resource.String.title_choose_file,
        Resource.String.title_choose, Resource.String.dialog_cancel)
    .EnableOptions(true)
    .DisplayPath(true)
    .WithChosenListener((dir, dirFile) =>
    {
        if (continueFromLast.Checked)
        {
            _path = dir;
        }
        Toast.MakeText(ctx, (dirFile.IsDirectory ? "FOLDER: " : "FILE: ") + dir, ToastLength.Short).Show();
    })
    .Build().Show();
```
 
For more information please refere to the [original repo](https://github.com/hedzr/android-file-chooser), and don't forget to give it a :star:!

## Licence

Standard Apache 2.0

Copyright 2015-2019 Hedzr Yeh

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

[android-file-chooser](https://github.com/hedzr/android-file-chooser) by [hedzr](https://github.com/hedzr)