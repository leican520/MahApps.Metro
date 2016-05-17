﻿namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    /// <summary>
    ///     Represents a control that allows the user to select a date and a time.
    /// </summary>
    [TemplatePart(Name = ElementCalendar, Type = typeof(System.Windows.Controls.Calendar))]
    [DefaultEvent("SelectedDateChanged")]
    public class DateTimePicker : TimePartPickerBase<DateTime?>
    {
        public static readonly DependencyProperty DisplayDateEndProperty = DatePicker.DisplayDateEndProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateProperty = DatePicker.DisplayDateProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateStartProperty = DatePicker.DisplayDateStartProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DatePicker.FirstDayOfWeekProperty.AddOwner(typeof(DateTimePicker));

        public static readonly DependencyProperty IsClockVisibleProperty = DependencyProperty.Register(
            "IsClockVisible",
            typeof(bool),
            typeof(DateTimePicker),
            new PropertyMetadata(true, OnClockVisibilityChanged));
      
        public static readonly DependencyProperty IsTodayHighlightedProperty = DatePicker.IsTodayHighlightedProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(DateTimePicker),
            new PropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));
        public static readonly RoutedEvent SelectedDateChangedEvent = DatePicker.SelectedDateChangedEvent.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty SelectedDateProperty = DatePicker.SelectedDateProperty.AddOwner(typeof(DateTimePicker), new PropertyMetadata(OnSelectedDateChanged));
      
        private const string ElementCalendar = "PART_Calendar";
        private System.Windows.Controls.Calendar _calendar;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            //VerticalContentAlignmentProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            //HorizontalContentAlignmentProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            //LanguageProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(OnCultureChanged));
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedDate" /> property is changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        /// <summary>
        ///     Gets or sets the date to display
        /// </summary>
        /// <returns>
        ///     The date to display. The default is <see cref="DateTime.Today" />.
        /// </returns>
        public DateTime? DisplayDate
        {
            get { return (DateTime?)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the last date to be displayed.
        /// </summary>
        /// <returns>The last date to display.</returns>
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the first date to be displayed.
        /// </summary>
        /// <returns>The first date to display.</returns>
        public DateTime? DisplayDateStart
        {
            get { return (DateTime?)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        /// <returns>
        ///     A <see cref="DayOfWeek" /> that represents the beginning of the week. The default is the
        ///     <see cref="System.Globalization.DateTimeFormatInfo.FirstDayOfWeek" /> that is determined by the current culture.
        /// </returns>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the clock of this control is visible in the user interface (UI). This is a
        ///     dependency property.
        /// </summary>
        /// <remarks>
        ///     If this value is set to false then <see cref="Orientation" /> is set to
        ///     <see cref="System.Windows.Controls.Orientation.Vertical" />
        /// </remarks>
        /// <returns>
        ///     true if the clock is visible; otherwise, false. The default value is true.
        /// </returns>
        [Category("Appearance")]
        public bool IsClockVisible
        {
            get { return (bool)GetValue(IsClockVisibleProperty); }
            set { SetValue(IsClockVisibleProperty, value); }
        }



        /// <summary>
        ///     Gets or sets a value that indicates whether the current date will be highlighted.
        /// </summary>
        /// <returns>true if the current date is highlighted; otherwise, false. The default is true. </returns>
        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value that indicates the dimension by which calendar and clock are stacked.
        /// </summary>
        /// <returns>
        ///     The <see cref="System.Windows.Controls.Orientation" /> of the calendar and clock. The default is
        ///     <see cref="System.Windows.Controls.Orientation.Horizontal" />.
        /// </returns>
        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the currently selected date.
        /// </summary>
        /// <returns>
        ///     The date currently selected. The default is null.
        /// </returns>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

      

        public override void OnApplyTemplate()
        {
            _calendar = GetTemplateChild(ElementCalendar) as System.Windows.Controls.Calendar;
            base.OnApplyTemplate();
            _calendar = GetTemplateChild(ElementCalendar) as System.Windows.Controls.Calendar;

        }


        protected virtual void OnSelectedDateChanged()
        {
            RaiseEvent(new RoutedEventArgs(SelectedDateChangedEvent));
        }

        private static object CoerceOrientation(DependencyObject d, object basevalue)
        {
            if (((DateTimePicker)d).IsClockVisible)
            {
                return basevalue;
            }
            return Orientation.Vertical;
        }

       


        private static void OnClockVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(OrientationProperty);
        }


        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimePicker = (DateTimePicker)d;
            var dt = (DateTime?)e.NewValue;
            dateTimePicker.OnSelectedDateChanged(dt);
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();

            if (_calendar != null)
            {
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateProperty, GetBinding(DisplayDateProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateStartProperty, GetBinding(DisplayDateStartProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateEndProperty, GetBinding(DisplayDateEndProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.FirstDayOfWeekProperty, GetBinding(FirstDayOfWeekProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.IsTodayHighlightedProperty, GetBinding(IsTodayHighlightedProperty));
                _calendar.SetBinding(FlowDirectionProperty, GetBinding(FlowDirectionProperty));
            }
        }

        protected sealed override void ApplyCulture()
        {
            base.ApplyCulture();

            if (_calendar != null)
            {
                _calendar.Language = XmlLanguage.GetLanguage(SpecificCultureInfo.IetfLanguageTag);
            }

            FirstDayOfWeek = this.SpecificCultureInfo.DateTimeFormat.FirstDayOfWeek;
        }

        protected override void OnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            base.OnRangeBaseValueChanged(sender, e);
            
            SetDatePartValues();
        }

        private void OnSelectedDateChanged(DateTime? toDate)
        {
            if (toDate.HasValue)
            {
                SetHourPartValues(toDate.Value.TimeOfDay);
            }
            else
            {
                SetDefaultTimeOfDayValues();
            }
            OnSelectedDateChanged();
        }

        private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var dt = (DateTime)e.AddedItems[0];
                dt = dt.Add(GetTimeOfDay());
                SelectedDate = dt;
            }
        }


        private DateTime GetCorrectDateTime()
        {
            return SelectedDate.GetValueOrDefault(DateTime.Today).Date + GetTimeOfDay();
        }


        private void SetDatePartValues()
        {
            var dateTime = GetCorrectDateTime();
            DisplayDate = dateTime != DateTime.MinValue ? dateTime : DateTime.Today;
            if (SelectedDate != DisplayDate || _popup != null && _popup.IsOpen)
            {
                SelectedDate = DisplayDate;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            if (_calendar != null)
            {
                _calendar.SelectedDatesChanged += OnSelectedDatesChanged;
            }
        }


        protected override void UnSubscribeEvents()
        {
            base.UnSubscribeEvents();

            if (_calendar != null)
            {
                _calendar.SelectedDatesChanged -= OnSelectedDatesChanged;
            }
        }

    }
}