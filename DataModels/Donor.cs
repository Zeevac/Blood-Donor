using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Blood_Donor.DataModels
{
    [Table("Donor")]
    public class Donor
    {
        [PrimaryKey, Column("id")]
        public int Id { get; set; }
        [MaxLength(250), Column("name")]
        public string Fullname { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("city")]
        public string City { get; set; }
        [Column("country")]
        public string Country { get; set; }
        [Column("bloodgroup")]
        public string BloodGroup { get; set; }
        [Column("latitude")]
        public double Latitude { get; set; }
        [Column("longitude")]
        public double Longitude { get; set; }

        public Donor(int ıd, string fullname, string phone, string email, string city, string country, string bloodGroup, double latitude, double longitude)
        {
            Id = ıd;
            Fullname = fullname;
            Phone = phone;
            Email = email;
            City = city;
            Country = country;
            BloodGroup = bloodGroup;
            Latitude = latitude;
            Longitude = longitude;
        }

        public Donor()
        {
        }

    }
}