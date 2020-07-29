using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Blood_Donor.DataModels;
using FR.Ganfra.Materialspinner;
using SQLite;

namespace Blood_Donor.Fragments
{
    public class NewDonorFragment : Android.Support.V4.App.DialogFragment
    {
        TextInputLayout fullnameText;
        TextInputLayout emailText;
        TextInputLayout phoneText;
        TextInputLayout cityText;
        TextInputLayout countryText;
        TextInputLayout latitudeText;
        TextInputLayout longitudeText;
        Button saveButton;
        MaterialSpinner materialSpinner;
        private SQLiteConnection conn;
        List<string> bloodGroupsList;
        ArrayAdapter<string> spinnerAdapter;
        string selectedBloodGroup;

        public event EventHandler<DonorDetailsEventArgs> OnDonorRegistered;
        public class DonorDetailsEventArgs : EventArgs
        {
            public Donor Donor { get; set; }
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            conn = MyConnectionFactory.Instance;
            conn.CreateTable<Donor>();
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.addnew, container, false);
            ConnectViews(view);
            SetupSpinner();
            return view;
        }

        void ConnectViews(View view)
        {
            fullnameText = (TextInputLayout)view.FindViewById(Resource.Id.fullnameText);
            emailText = (TextInputLayout)view.FindViewById(Resource.Id.emailText);
            phoneText = (TextInputLayout)view.FindViewById(Resource.Id.phoneText);
            cityText = (TextInputLayout)view.FindViewById(Resource.Id.cityText);
            countryText = (TextInputLayout)view.FindViewById(Resource.Id.countryText);
            latitudeText = (TextInputLayout)view.FindViewById(Resource.Id.latitudeText);
            longitudeText = (TextInputLayout)view.FindViewById(Resource.Id.longitudeText);
            materialSpinner = (MaterialSpinner)view.FindViewById(Resource.Id.materialSpinner);
            saveButton = (Button)view.FindViewById(Resource.Id.saveButton);

            saveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string fullname, email, phone, city, country, latitude, longitude;

            fullname = fullnameText.EditText.Text;
            email = emailText.EditText.Text;
            phone = phoneText.EditText.Text;
            city = cityText.EditText.Text;
            country = countryText.EditText.Text;
            latitude = latitudeText.EditText.Text;
            longitude = longitudeText.EditText.Text;
            latitude = latitude.Replace(".", ",");
            longitude = longitude.Replace(".", ",");

            if (fullname.Length < 5)
            {
                Toast.MakeText(Activity, "Please provide a valid name", ToastLength.Short).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Toast.MakeText(Activity, "Please provide a valid email address", ToastLength.Short).Show();
                return;
            }
            else if (phone.Length < 10)
            {
                Toast.MakeText(Activity, "Please provide a valid phone number", ToastLength.Short).Show();
                return;
            }
            else if (city.Length < 2)
            {
                Toast.MakeText(Activity, "Please provide a valid city", ToastLength.Short).Show();
                return;
            }
            else if (country.Length < 2)
            {
                Toast.MakeText(Activity, "Please provide a valid country", ToastLength.Short).Show();
                return;
            }
            else if (selectedBloodGroup.Length < 2)
            {
                Toast.MakeText(Activity, "Please select a blood group", ToastLength.Short).Show();
                return;
            }
            else if (latitude.Length < 1)
            {
                Toast.MakeText(Activity, "Please provide a valid latitude", ToastLength.Short).Show();
                return;
            }
            else if (longitude.Length < 1)
            {
                Toast.MakeText(Activity, "Please provide a valid longitude", ToastLength.Short).Show();
                return;
            }
            Console.WriteLine(latitude);
            Console.WriteLine(Convert.ToDouble(latitude));
            var totalDonors = conn.Table<Donor>().ToList().Count;
            Donor donor = new Donor(totalDonors, fullname, phone, email, city, country, selectedBloodGroup, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            conn.Insert(donor);
            OnDonorRegistered?.Invoke(this, new DonorDetailsEventArgs { Donor = donor });
        }

        void SetupSpinner()
        {
            bloodGroupsList = new List<string>();

            bloodGroupsList.Add("O+");
            bloodGroupsList.Add("O-");
            bloodGroupsList.Add("AB-");
            bloodGroupsList.Add("AB+");
            bloodGroupsList.Add("A+");
            bloodGroupsList.Add("A-");
            bloodGroupsList.Add("A+");
            bloodGroupsList.Add("B+");
            bloodGroupsList.Add("B-");

            spinnerAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, bloodGroupsList);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            materialSpinner.Adapter = spinnerAdapter;
            materialSpinner.ItemSelected += MaterialSpinner_ItemSelected;
        }

        private void MaterialSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position != -1)
            {
                selectedBloodGroup = bloodGroupsList[e.Position];
                Console.WriteLine(selectedBloodGroup);
            }
        }
    }
}