using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Blood_Donor
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : FragmentActivity, IOnMapReadyCallback
    {
        private GoogleMap mMap;
        LocationManager locationManager;
        public static int ACCESS_FINE_LOCATION_REQUEST_CODE = 1;

        public string TAG { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_map);
            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            Console.WriteLine("OnMapReady called");
            locationManager = (LocationManager)GetSystemService(Context.LocationService);
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation }, ACCESS_FINE_LOCATION_REQUEST_CODE);
            }
            else
            {
                double latitude = Intent.GetDoubleExtra("latitude", 0.0);
                double longitude = Intent.GetDoubleExtra("longitude", 0.0);
                string name = Intent.GetStringExtra("name");
                LatLng location = new LatLng(latitude, longitude);
                Console.WriteLine(latitude + " " + longitude);
                mMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle(name));
                mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(location, 15));

                /*CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);

                CameraPosition cameraPosition = builder.Build();

                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                mMap.MoveCamera(cameraUpdate);*/
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == ACCESS_FINE_LOCATION_REQUEST_CODE)
            {
                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    double latitude = Intent.GetDoubleExtra("latitude", 0.0);
                    double longitude = Intent.GetDoubleExtra("longitude", 0.0);
                    string name = Intent.GetStringExtra("name");
                    LatLng location = new LatLng(latitude, longitude);
                    Console.WriteLine(latitude + " " + longitude);
                    mMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle(name));
                    mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(location, 15));

                    /*CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(location);
                    builder.Zoom(18);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    
                    mMap.MoveCamera(cameraUpdate);*/
                }
                else
                {
                    Log.Info(TAG, "Location permission was NOT granted.");
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
    }
}