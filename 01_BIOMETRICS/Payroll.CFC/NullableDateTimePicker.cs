using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Payroll.CFC
{
  /// <summary>
  /// Represents a Windows date time picker control. It enhances the .NET standard <b>DateTimePicker</b>
  /// control with the possibility to show empty values (null values).
  /// </summary>
  [ComVisible(false)]
  public class NullableDateTimePicker : System.Windows.Forms.DateTimePicker
  {
    #region Member variables
    // true, when no date shall be displayed (empty DateTimePicker)
    private bool _isNull;

    // If _isNull = true, this value is shown in the DTP
    private string _nullValue;

    // The format of the DateTimePicker control
    private DateTimePickerFormat _format = DateTimePickerFormat.Long;

    // The custom format of the DateTimePicker control
    private string _customFormat;

    // The format of the DateTimePicker control as string
    private string _formatAsString;
    #endregion

    #region Constructor
    /// <summary>
    /// Default Constructor
    /// </summary>
    public NullableDateTimePicker() : base()
    {
      base.Format = DateTimePickerFormat.Custom;
      NullValue = " ";
      Format = DateTimePickerFormat.Long;
    }
    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the date/time value assigned to the control.
    /// </summary>
    /// <value>The DateTime value assigned to the control
    /// </value>
    /// <remarks>
    /// <p>If the <b>Value</b> property has not been changed in code or by the user, it is set
    /// to the current date and time (<see cref="DateTime.Now"/>).</p>
    /// <p>If <b>Value</b> is <b>null</b>, the DateTimePicker shows 
    /// <see cref="NullValue"/>.</p>
    /// </remarks>
    [Bindable(true)]
    [Browsable(false)]
    public new Object Value 
    {
      get 
      {
        if (_isNull)
          return null;
        else
          return base.Value;
      }
      set 
      {
        if (value == null || value == DBNull.Value)
        {
          SetToNullValue();
        }
        else 
        {
          SetToDateTimeValue();
          base.Value = DateTime.Parse(value.ToString()); //20071217 FULBERT (DateTime)value
        }
      }
    }
      [Bindable(true)]
      [Browsable(false)]
      public Object ForDBValue
      {
          get
          {
              if (_isNull)
                  return DBNull.Value;
              else
                  return base.Value;
          }
      }


    /// <summary>
    /// Gets or sets the format of the date and time displayed in the control.
    /// </summary>
    /// <value>One of the <see cref="DateTimePickerFormat"/> values. The default is 
    /// <see cref="DateTimePickerFormat.Long"/>.</value>
    [Browsable(true)]
    [DefaultValue(DateTimePickerFormat.Long), TypeConverter(typeof(Enum))]
    public new DateTimePickerFormat Format
    {
      get { return _format; }
      set
      {
        _format = value;
        if (!_isNull)
          SetFormat();
        OnFormatChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets the custom date/time format string.
    /// <value>A string that represents the custom date/time format. The default is a null
    /// reference (<b>Nothing</b> in Visual Basic).</value>
    /// </summary>
    public new String CustomFormat
    {
      get { return _customFormat; }
      set { _customFormat = value; }
    }

    /// <summary>
    /// Gets or sets the string value that is assigned to the control as null value. 
    /// </summary>
    /// <value>The string value assigned to the control as null value.</value>
    /// <remarks>
    /// If the <see cref="Value"/> is <b>null</b>, <b>NullValue</b> is
    /// shown in the <b>DateTimePicker</b> control.
    /// </remarks>
    [Browsable(true)]
    [Category("Behavior")]
    [Description("The string used to display null values in the control")]
    [DefaultValue(" ")]
    public String NullValue
    {
      get { return _nullValue; }
      set { _nullValue = value; }
    }
    #endregion

    #region Private methods/properties
    /// <summary>
    /// Stores the current format of the DateTimePicker as string. 
    /// </summary>
    private string FormatAsString
    {
      get { return _formatAsString; }
      set 
      {
        _formatAsString = value;
        base.CustomFormat = value; }
    }

    /// <summary>
    /// Sets the format according to the current DateTimePickerFormat.
    /// </summary>
    private void SetFormat()
    {
      CultureInfo ci = Thread.CurrentThread.CurrentCulture;
      DateTimeFormatInfo dtf = ci.DateTimeFormat;
      switch (_format)
      {
        case DateTimePickerFormat.Long:
          FormatAsString = dtf.LongDatePattern;
          break;  
        case DateTimePickerFormat.Short:
          FormatAsString = dtf.ShortDatePattern;
          break;
        case DateTimePickerFormat.Time:
          FormatAsString = dtf.ShortTimePattern;
          break;
        case DateTimePickerFormat.Custom:
          FormatAsString = this.CustomFormat;
          break;
      }
    }

    /// <summary>
    /// Sets the <b>DateTimePicker</b> to the value of the <see cref="NullValue"/> property.
    /// </summary>
    private void SetToNullValue()
    {
      _isNull = true;
      base.CustomFormat = (_nullValue == null || _nullValue == String.Empty) ? " " : "'" + _nullValue + "'";
    }

    /// <summary>
    /// Sets the <b>DateTimePicker</b> back to a non null value.
    /// </summary>
    private void SetToDateTimeValue()
    {
      if (_isNull) 
      {
        SetFormat();
        _isNull = false;
        base.OnValueChanged(new EventArgs());
      }
    }
    #endregion

    #region Events
    /// <summary>
    /// This member overrides <see cref="Control.WndProc"/>.
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
      if (_isNull)
      {
        if (m.Msg == 0x4e)                         // WM_NOTIFY
        {
          NMHDR nm = (NMHDR)m.GetLParam(typeof(NMHDR));
          if (nm.Code == -746 || nm.Code == -722)  // DTN_CLOSEUP || DTN_?
            SetToDateTimeValue();
        }
      }
      base.WndProc(ref m);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NMHDR
    {
      public IntPtr HwndFrom;
      public int IdFrom;
      public int Code;
    }

    /// <summary>
    /// This member overrides <see cref="Control.OnKeyDown"/>.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
      {
        this.Value = null;
        OnValueChanged(EventArgs.Empty);
      }
      base.OnKeyUp(e);
    }

    protected override void OnValueChanged(EventArgs eventargs)
    {
      base.OnValueChanged(eventargs);
    }

    #endregion
  }
}
