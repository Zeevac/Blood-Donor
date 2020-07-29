using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Blood_Donor.DataModels;
using SQLite;
using Java.Lang;
using System.Linq;
using System.Globalization;

namespace Blood_Donor.Adapters
{
    public class DonorsAdapter : RecyclerView.Adapter, IFilterable
    {
        public event EventHandler<DonorsAdapterClickEventArgs> ItemClick;
        public event EventHandler<DonorsAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<DonorsAdapterClickEventArgs> CallClick;
        public event EventHandler<DonorsAdapterClickEventArgs> EmailClick;
        public event EventHandler<DonorsAdapterClickEventArgs> DeleteClick;
        public event EventHandler<DonorsAdapterClickEventArgs> MapClick;
        private SQLiteConnection conn;
        public List<Donor> _originalData;
        public List<Donor> DonorsList;
        public Filter Filter { get; private set; }

        public DonorsAdapter(List<Donor> list)
        {
            conn = MyConnectionFactory.Instance;
            DonorsList = list;
            Filter = new DonorFilter(this);
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.donor_row, parent, false);

            var vh = new DonorsAdapterViewHolder(itemView, OnClick, OnLongClick, OnCallClick, OnEmailClick, OnDeletClick, OnMapClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var donor = DonorsList[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as DonorsAdapterViewHolder;
            holder.donorNameTextView.Text = donor.Fullname;
            holder.donorLocationTextView.Text = donor.City + ", " + donor.Country;

            // Assign appropriate Images to Donors Blood Group
            if (donor.BloodGroup == "O+")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.o_ppositive);
            }
            else if (donor.BloodGroup == "O-")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.o_negative);
            }
            else if (donor.BloodGroup == "AB-")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.ab_negative);
            }
            else if (donor.BloodGroup == "AB+")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.ab_positive);
            }
            else if (donor.BloodGroup == "B-")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.b_negative);
            }
            else if (donor.BloodGroup == "B+")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.b_positive);
            }
            else if (donor.BloodGroup == "A-")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.a_negative);
            }
            else if (donor.BloodGroup == "A+")
            {
                holder.bloodGroupImageView.SetImageResource(Resource.Drawable.a_positive);
            }

        }

        public override int ItemCount => DonorsList.Count;

        void OnClick(DonorsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(DonorsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        void OnCallClick(DonorsAdapterClickEventArgs args) => CallClick?.Invoke(this, args);
        void OnEmailClick(DonorsAdapterClickEventArgs args) => EmailClick?.Invoke(this, args);
        void OnDeletClick(DonorsAdapterClickEventArgs args) => DeleteClick?.Invoke(this, args);
        void OnMapClick(DonorsAdapterClickEventArgs args) => MapClick?.Invoke(this, args);

        private class DonorFilter : Filter
        {
            private readonly DonorsAdapter _adapter;
            public DonorFilter(DonorsAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Donor>();
                if (_adapter._originalData == null)
                    _adapter._originalData = _adapter.DonorsList;

                if (constraint == null) return returnObj;

                if (_adapter._originalData != null && _adapter._originalData.Any())
                {
                    // Compare constraint to all names lowercased. 
                    // It they are contained they are added to results.
                    results.AddRange(
                        _adapter._originalData.Where(
                            donor => donor.City.ToLower(new CultureInfo("tr-TR", false)).Contains(constraint.ToString())));
                }

                // Nasty piece of .NET to Java wrapping, be careful with this!
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter.DonorsList = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<Donor>()).ToList();

                _adapter.NotifyDataSetChanged();

                // Don't do this and see GREF counts rising
                constraint.Dispose();
                results.Dispose();
            }
        }

    }

    public class DonorsAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }

        public TextView donorNameTextView;
        public TextView donorLocationTextView;
        public ImageView bloodGroupImageView;
        public RelativeLayout callLayout;
        public RelativeLayout emailLayout;
        public RelativeLayout deleteLayout;
        public LinearLayout mapLayout;

        public DonorsAdapterViewHolder(View itemView, Action<DonorsAdapterClickEventArgs> clickListener,
                            Action<DonorsAdapterClickEventArgs> longClickListener, Action<DonorsAdapterClickEventArgs> callClickListener,
                            Action<DonorsAdapterClickEventArgs> emailClickListener, Action<DonorsAdapterClickEventArgs> deleteClickListener, Action<DonorsAdapterClickEventArgs> mapClickListener) : base(itemView)
        {
            //TextView = v;
            donorNameTextView = (TextView)itemView.FindViewById(Resource.Id.donorNameTextView);
            donorLocationTextView = (TextView)itemView.FindViewById(Resource.Id.donorLocationTextView);
            bloodGroupImageView = (ImageView)itemView.FindViewById(Resource.Id.bloodGroupImageView);
            callLayout = (RelativeLayout)itemView.FindViewById(Resource.Id.callLayout);
            emailLayout = (RelativeLayout)itemView.FindViewById(Resource.Id.emailLayout);
            deleteLayout = (RelativeLayout)itemView.FindViewById(Resource.Id.deleteLayout);
            mapLayout = (LinearLayout)itemView.FindViewById(Resource.Id.mapLayout);


            itemView.Click += (sender, e) => clickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            callLayout.Click += (sender, e) => callClickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            emailLayout.Click += (sender, e) => emailClickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            deleteLayout.Click += (sender, e) => deleteClickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            mapLayout.Click += (sender, e) => mapClickListener(new DonorsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

        }



    }

    public class DonorsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }


}