﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content.Res;
using System.IO;
using Linphone;
using Org.Linphone.Mediastream.Video;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Droid
{
    [Activity(Label = "Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Org.Linphone.Mediastream.Video.Display.GL2JNIView captureCamera;

        protected override void OnCreate(Bundle bundle)
        {
            Java.Lang.JavaSystem.LoadLibrary("c++_shared");
            Java.Lang.JavaSystem.LoadLibrary("bctoolbox");
            Java.Lang.JavaSystem.LoadLibrary("ortp");
            Java.Lang.JavaSystem.LoadLibrary("mediastreamer_base");
            Java.Lang.JavaSystem.LoadLibrary("mediastreamer_voip");
            Java.Lang.JavaSystem.LoadLibrary("linphone");

            // This is mandatory for Android
            LinphoneAndroid.setAndroidContext(JNIEnv.Handle, this.Handle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            AssetManager assets = Assets;
            string path = FilesDir.AbsolutePath;
            string rc_path = path + "/default_rc";
            using (var br = new BinaryReader(Application.Context.Assets.Open("linphonerc_default")))
            {
                using (var bw = new BinaryWriter(new FileStream(rc_path, FileMode.Create)))
                {
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);
            App app = new App(); // Do not add an arg to App constructor
            app.ConfigFilePath = rc_path;

            captureCamera = new Org.Linphone.Mediastream.Video.Display.GL2JNIView(this);
            captureCamera.Holder.SetFixedSize(1920, 1080);
            AndroidVideoWindowImpl androidView = new AndroidVideoWindowImpl(captureCamera, null, null);
            app.Core.NativeVideoWindowId = androidView.Handle;
            app.Core.VideoDisplayEnabled = true;
            app.getLayoutView().Children.Add(captureCamera.ToView());

            LoadApplication(app);
        }
    }
}

