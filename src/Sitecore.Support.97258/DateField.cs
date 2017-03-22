using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using Sitecore.Forms.Mvc.ViewModels;

namespace Sitecore.Support.Forms.Mvc.ViewModels.Fields
{
    public class DateField : ValuedFieldViewModel<string>
    {
        public object Day
        {
            get;
            set;
        }

        public object Month
        {
            get;
            set;
        }

        public object Year
        {
            get;
            set;
        }

        public string DayTitle
        {
            get;
            set;
        }

        public string MonthTitle
        {
            get;
            set;
        }

        public string YearTitle
        {
            get;
            set;
        }

        [DefaultValue("yyyy-MMMM-dd")]
        public string DateFormat
        {
            get;
            set;
        }

        public List<SelectListItem> Years
        {
            get;
            private set;
        }

        public List<SelectListItem> Months
        {
            get;
            private set;
        }

        public List<SelectListItem> Days
        {
            get;
            private set;
        }

        [TypeConverter(typeof(IsoDateTimeConverter))]
        public DateTime StartDate
        {
            get;
            set;
        }

        [TypeConverter(typeof(IsoDateTimeConverter))]
        public DateTime EndDate
        {
            get;
            set;
        }

        [ParameterName("SelectedDate")]
        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
                this.OnValueUpdated();
            }
        }

        public override string ResultParameters
        {
            get
            {
                return this.DateFormat;
            }
        }

        protected void OnValueUpdated()
        {
            if (!string.IsNullOrEmpty(this.Value))
            {
                DateTime dateTime = DateUtil.IsoDateToDateTime(this.Value);
                this.Day = dateTime.Day;
                this.Month = dateTime.Month;
                this.Year = dateTime.Year;
            }
            this.InitItems();
        }

        public override void Initialize()
        {
            if (string.IsNullOrEmpty(this.DateFormat))
            {
                this.DateFormat = "yyyy-MMMM-dd";
            }
            if (this.StartDate == DateTime.MinValue)
            {
                this.StartDate = DateUtil.IsoDateToDateTime("20000101T120000");
            }
            if (this.EndDate == DateTime.MinValue)
            {
                this.EndDate = DateTime.Now.AddYears(1).Date;
            }
            this.Years = new List<SelectListItem>();
            this.Months = new List<SelectListItem>();
            this.Days = new List<SelectListItem>();
            this.InitItems();
        }

        private void InitItems()
        {
            List<string> list = new List<string>(this.DateFormat.Split(new char[]
            {
                '-'
            }));
            list.Reverse();
            list.ForEach(new Action<string>(this.InitDate));
        }

        private void InitDate(string marker)
        {
            DateTime? current = string.IsNullOrEmpty(this.Value) ? null : new DateTime?(DateUtil.IsoDateToDateTime(this.Value));
            char c = marker.ToLower()[0];
            if (c == 'd')
            {
                this.InitDays(current);
                return;
            }
            if (c == 'm')
            {
                this.InitMonth(marker, current);
                return;
            }
            if (c != 'y')
            {
                return;
            }
            this.InitYears(marker, current);
        }

        private void InitYears(string marker, DateTime? current)
        {
            DateTime dateTime = new DateTime(this.StartDate.Year - 1, 1, 1);
            this.Years.Clear();
            for (int i = this.StartDate.Year; i <= this.EndDate.Year; i++)
            {
                dateTime = dateTime.AddYears(1);
                SelectListItem item = new SelectListItem
                {
                    Text = string.Format("{0:" + marker + "}", dateTime),
                    Value = i.ToString(CultureInfo.InvariantCulture),
                    Selected = (current.HasValue && current.Value.Year == i)
                };
                this.Years.Add(item);
            }
        }

        private void InitDays(DateTime? current)
        {
            this.Days.Clear();
            int num = current.HasValue ? DateTime.DaysInMonth(current.Value.Year, current.Value.Month) : 31;
            for (int i = 1; i <= 31; i++)
            {
                if (i <= num)
                {
                    this.Days.Add(new SelectListItem
                    {
                        Selected = (current.HasValue && current.Value.Day == i),
                        Text = i.ToString(CultureInfo.InvariantCulture),
                        Value = i.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
        }

        private void InitMonth(string marker, DateTime? current)
        {
            DateTime dateTime = default(DateTime);
            this.Months.Clear();
            for (int i = 1; i <= 12; i++)
            {
                this.Months.Add(new SelectListItem
                {
                    Selected = (current.HasValue && current.Value.Month == i),
                    Text = string.Format("{0:" + marker + "}", dateTime.AddMonths(i - 1)),
                    Value = i.ToString(CultureInfo.InvariantCulture)
                });
            }
        }
    }
}