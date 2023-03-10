using System;
using System.Drawing; 
using System.Windows.Forms;

namespace Payroll.CFC
{	
	public enum mcDayState {Normal = 0, Focus, Selected}

	#region Delegates

	public delegate void DayClickEventHandler(object sender, DayClickEventArgs e);
    public delegate void DayMouseMoveEventHandler(object sender, DayMouseMoveEventArgs e);
	public delegate void DayEventHandler(object sender, DayEventArgs e);
	public delegate void DaySelectedEventHandler(object sender, DaySelectedEventArgs e);
	public delegate void DayDragDropEventHandler(object sender, DayDragDropEventArgs e);
    public delegate void DayStateChangedEventHandler(object sender , DayStateChangedEventArgs e);

	#endregion

	/// <summary>
	/// Summary description for Day.
	/// </summary>
	internal class Day : IDisposable	
	{
		#region Private class members
		
		private bool disposed;
		private Rectangle m_rect;
		private Region m_region;
		private DateTime m_date;
		private MonthCalendar m_calendar;
		private Month m_month;
		private Image m_dayImage;
		private int m_selection;
		private bool m_userDrawn;
		private Rectangle m_imageRect;
    
		private mcDayState m_state;
		
		#endregion
          
        #region constructor

        public Day()
		{
			m_state = mcDayState.Normal; 
			m_selection = -1;
			m_userDrawn = false;
    	}	

		#endregion
	
		#region Dispose
		
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					m_region.Dispose();
					m_dayImage.Dispose();
				
				}
				// shared cleanup logic
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region properties

    	internal MonthCalendar Calendar
		{
			get
			{
				return m_calendar;
			}
			set
			{	
				m_calendar = value;
			}
		}

		internal Month Month
		{
			get
			{
				return m_month;
			}
			set
			{	
				m_month = value;
			}
		}
		
		public int SelectionArea
		{
			get
			{
				return m_selection;
			}
			set
			{	
				if (value!=m_selection)
				{
					m_selection = value;
				}
			}
		}

		public bool UserDrawn
		{
			get
			{
				return m_userDrawn;
			}
			set
			{	
				if (value!=m_userDrawn)
				{
					m_userDrawn = value;
				}
			}
		}

		public int Week
		{
			get
			{
				return m_calendar.WeeknumberCallBack(m_date);
			}
		}

		public DayOfWeek Weekday
		{
			get
			{
				return m_date.DayOfWeek;
			}	
		}

		public mcDayState State
		{
			get
			{
				return m_state;
			}
			set
			{	
				if (value!=m_state)
				{
                    m_state = value;
    			}
			}
		}
		
		public Rectangle Rectangle
		{
			get
			{
				return m_rect;
			}
			set
			{
				if (value!=m_rect)
				{
					m_rect = value;
					m_region = new Region(m_rect); 
				}
			}
		}

		public DateTime Date
		{
			get
			{
				return m_date;
			}
			set
			{
				if (m_date!=value)
				{
					m_date = value;
				}
			}
		}
		
		#endregion
		
		#region Methods
		
		private Image GetImage(int index)
		{
			// Check that an ImageList exists and that index is valid
			if (m_month.Calendar.ImageList!=null)
			{
				if ((index>=0) && (index <m_month.Calendar.ImageList.Images.Count))
				{
					return m_month.Calendar.ImageList.Images[index]; 
				}
				else return null;
			}
			else return null;
					
		}

		private StringFormat GetStringAlignment(mcItemAlign align)
		{
			StringFormat sf = new StringFormat();
 
			switch (align)
			{
				case mcItemAlign.LeftCenter:
				{
					sf.Alignment = StringAlignment.Near;   
					sf.LineAlignment = StringAlignment.Center;
					break;
				}
				case mcItemAlign.RightCenter:
				{
					sf.Alignment = StringAlignment.Far;   
					sf.LineAlignment = StringAlignment.Center;
					break;
				}
				case mcItemAlign.TopCenter:
				{
					sf.Alignment = StringAlignment.Center;   
					sf.LineAlignment = StringAlignment.Near;
					break;
				}
				case mcItemAlign.BottomCenter:
				{
					sf.Alignment = StringAlignment.Center;   
					sf.LineAlignment = StringAlignment.Far;
					break;
				}
				case mcItemAlign.TopLeft:
				{
					sf.Alignment = StringAlignment.Near;   
					sf.LineAlignment = StringAlignment.Near;
					break;
				}
				case mcItemAlign.TopRight:
				{
					sf.Alignment = StringAlignment.Far;   
					sf.LineAlignment = StringAlignment.Near;
					break;
				}
				case mcItemAlign.Center:
				{
					sf.Alignment = StringAlignment.Center;   
					sf.LineAlignment = StringAlignment.Center;
					break;
				}
				case mcItemAlign.BottomLeft:
				{
					sf.Alignment = StringAlignment.Near;   
					sf.LineAlignment = StringAlignment.Far;
					break;
				}
				case mcItemAlign.BottomRight:
				{
					sf.Alignment = StringAlignment.Far;   
					sf.LineAlignment = StringAlignment.Far;
					break;
				}
			}
			
			return sf;
		}

		internal void Draw(Graphics e, DateItem queryInfo)
		{
											
			StringFormat dateAlign = new StringFormat();
			StringFormat textAlign = new StringFormat();
           	Font boldFont = new Font(m_month.DateFont.Name,m_month.DateFont.Size,m_month.DateFont.Style | FontStyle.Bold);
            Color bgColor1 = m_month.Colors.Days.BackColor1;
            Color bgColor2 = m_month.Colors.Days.BackColor2;
            mcGradientMode gradientMode = m_month.Colors.Days.GradientMode;
            Color textColor = m_month.Colors.Days.Text;
            Color dateColor = m_month.Colors.Days.Date;
            Brush dateBrush = new SolidBrush(dateColor);
            Brush textBrush = new SolidBrush(textColor);
            Brush bgBrush = new SolidBrush(bgColor1);   
		            
            string dateString;
			m_imageRect = new Rectangle(); 
			string text = "";
			bool drawDay = false;
			bool enabled = true;
            Image bgImage = null;
			
			int i = -1;

			bool boldedDate = false;
 
			DateItem[] info;
			m_dayImage = null;
	
			dateAlign = GetStringAlignment(m_month.DateAlign); 
			textAlign = GetStringAlignment(m_month.TextAlign);							
			
			if ((m_month.SelectedMonth.Month == m_date.Month) || (m_month.Calendar.ShowTrailingDates))
				drawDay = true;
			
			if ( ((m_date.DayOfWeek == DayOfWeek.Saturday) && (m_month.Colors.Weekend.Saturday)) ||
                 ((m_date.DayOfWeek == DayOfWeek.Sunday) && (m_month.Colors.Weekend.Sunday)) )
			{
			    bgColor1 = m_month.Colors.Weekend.BackColor1;
                bgColor2 = m_month.Colors.Weekend.BackColor2;
                dateColor= m_month.Colors.Weekend.Date;
				textColor= m_month.Colors.Weekend.Text;
                gradientMode = m_month.Colors.Weekend.GradientMode;  
			}			
			
			if (m_month.SelectedMonth.Month  != m_date.Month)
			{
				bgColor1 =  m_month.Colors.Trailing.BackColor1;
                bgColor2 = m_month.Colors.Trailing.BackColor2;
                gradientMode = m_month.Colors.Trailing.GradientMode;
                dateColor = m_month.Colors.Trailing.Date; 
				textColor = m_month.Colors.Trailing.Text; 
			}
				
			// Check if formatting should be applied
			if ((m_month.FormatTrailing) || (m_month.SelectedMonth.Month  == m_date.Month)) 
			{
				// check of there is formatting for this day
                if (queryInfo != null)
                {
                    info = new DateItem[1];
                    info[0] = queryInfo;
                }
                else
                    info = m_calendar.GetDateInfo(this.Date);
				if (info.Length > 0)
					i = 0;
				// go through the available dateitems
				while ((i<info.Length) && (drawDay))
				{
					if (info.Length>0)
					{
						DateItem dateInfo = info[i];
				
						if (dateInfo.BackColor1!=Color.Empty)  
							bgColor1 = dateInfo.BackColor1;
                        if (dateInfo.BackColor2 != Color.Empty)
                            bgColor2 = dateInfo.BackColor2;
						gradientMode = dateInfo.GradientMode; 
                        if (dateInfo.DateColor!=Color.Empty)  
							dateColor = dateInfo.DateColor;
						if (dateInfo.TextColor!=Color.Empty)  
							textColor = dateInfo.TextColor;
						text = dateInfo.Text; 
				
						if (dateInfo.Weekend)
						{
							bgColor1 = m_month.Colors.Weekend.BackColor1;
                            bgColor2 = m_month.Colors.Weekend.BackColor2;
                            gradientMode = m_month.Colors.Weekend.GradientMode;  
                            dateColor = m_month.Colors.Weekend.Date;
							textColor = m_month.Colors.Weekend.Text;
						}
						boldedDate = dateInfo.BoldedDate; 
						enabled = dateInfo.Enabled;
						if (!dateInfo.Enabled)
						{
							bgColor1 = m_month.Colors.Disabled.BackColor1;
                            bgColor2 = m_month.Colors.Disabled.BackColor2;
                            gradientMode = m_month.Colors.Disabled.GradientMode;     
							dateColor = m_month.Colors.Disabled.Date;
							textColor = m_month.Colors.Disabled.Text;
						}
 						
						m_dayImage = dateInfo.Image;  	
									
						if (m_dayImage!=null)
							m_imageRect = ImageRect(m_month.ImageAlign);
                        
                        bgImage = dateInfo.BackgroundImage; 
                    }

					if (m_state == mcDayState.Selected)
					{
						dateColor = m_month.Colors.Selected.Date; 
						textColor = m_month.Colors.Selected.Text;
					}
					if ((m_state == mcDayState.Focus) && (m_month.Calendar.ShowFocus))  
					{
						dateColor = m_month.Colors.Focus.Date; 
						textColor = m_month.Colors.Focus.Text;
					}
                    

                    if (bgImage != null)
                        e.DrawImage(bgImage, m_rect);
                    else
                    {
                        if (gradientMode == mcGradientMode.None)
                        {
                            if (bgColor1 != Color.Transparent)
                            {
                                bgBrush = new SolidBrush(Color.FromArgb(m_month.Transparency.Background, bgColor1));
                                e.FillRectangle(bgBrush, m_rect);
                            }
                        }
                        else
                            m_calendar.DrawGradient(e, Rectangle, bgColor1, bgColor2, gradientMode);
                    }

					
                    ControlPaint.DrawBorder(e,m_rect, m_month.Colors.Days.Border,m_month.BorderStyles.Normal);
					if (m_dayImage!=null)
					{
						if (enabled)
							e.DrawImageUnscaled(m_dayImage,m_imageRect);
						else
							ControlPaint.DrawImageDisabled(e,m_dayImage,m_imageRect.X,m_imageRect.Y,m_month.Colors.Disabled.BackColor1);   
					}
            						
					// Check if we should append month name to date
					if ((m_month.ShowMonthInDay) &&
						((m_date.AddDays(-1).Month != m_date.Month) ||
						(m_date.AddDays(1).Month != m_date.Month)))							
						dateString = m_date.Day.ToString()+" "+m_calendar.m_dateTimeFormat.GetMonthName(m_date.Month);  
					else
						dateString = m_date.Day.ToString();

                    if (dateColor != Color.Transparent)
                    {
                        dateBrush = new SolidBrush(Color.FromArgb(m_month.Transparency.Text, dateColor));

                        // Should date be bolded ?
                        if (!boldedDate)
                            e.DrawString(dateString, m_month.DateFont, dateBrush, m_rect, dateAlign);
                        else
                            e.DrawString(dateString, boldFont, dateBrush, m_rect, dateAlign);
					}
                    if ((text.Length > 0) && (textColor != Color.Transparent))
                    {
                        textBrush = new SolidBrush(Color.FromArgb(m_month.Transparency.Text, textColor));
                        
                        e.DrawString(text, m_month.TextFont, textBrush, m_rect, textAlign);
                    }
					i++;	
				}
			} 
								
			dateBrush.Dispose();
			bgBrush.Dispose();
			textBrush.Dispose();
			boldFont.Dispose();
			dateAlign.Dispose();
			textAlign.Dispose();
			
		}

		private Rectangle ImageRect(mcItemAlign align)
		{
			Rectangle imageRect = new Rectangle(0,0,m_rect.Width,m_rect.Height);
 
			switch (align)
			{
				
				case mcItemAlign.LeftCenter:
				{
					imageRect.X = m_rect.X + 2;
					imageRect.Y = m_rect.Top +((m_rect.Height/2) - (m_dayImage.Height/2));
					break;
				}
				case mcItemAlign.RightCenter:
				{
					imageRect.X = m_rect.Right - 2 - m_dayImage.Width;
					imageRect.Y = m_rect.Top +((m_rect.Height/2) - (m_dayImage.Height/2));
					break;
				}
				case mcItemAlign.TopCenter:
				{
					imageRect.X = m_rect.X +((m_rect.Width/2) - (m_dayImage.Width/2));
					imageRect.Y = m_rect.Y + 2;
					break;
				}
				case mcItemAlign.BottomCenter:
				{
					imageRect.X = m_rect.X +((m_rect.Width/2) - (m_dayImage.Width/2));
					imageRect.Y = m_rect.Bottom -2 - m_dayImage.Height;
					break;
				}
				
				case mcItemAlign.TopLeft:
				{
					imageRect.X = m_rect.X + 2;
					imageRect.Y = m_rect.Y + 2;
					break;
				}
				case mcItemAlign.TopRight:
				{
					imageRect.X = m_rect.Right - 2 - m_dayImage.Width;
					imageRect.Y = m_rect.Y + 2;
					break;
				}
				case mcItemAlign.Center:
				{
					imageRect.X = m_rect.X +((m_rect.Width/2) - (m_dayImage.Width/2));
					imageRect.Y = m_rect.Top +((m_rect.Height/2) - (m_dayImage.Height/2));
					break;
				}
				case mcItemAlign.BottomLeft:
				{
					imageRect.X = m_rect.X + 2;
					imageRect.Y = m_rect.Bottom -2 - m_dayImage.Height;
					break;
				}
				case mcItemAlign.BottomRight:
				{
					imageRect.X = m_rect.Right - 2 - m_dayImage.Width;
					imageRect.Y = m_rect.Bottom -2 - m_dayImage.Height;	
					break;
				}
			}
			
			imageRect.Height = m_dayImage.Height;
			imageRect.Width = m_dayImage.Width;
			return imageRect;
		}
		
		internal bool ImageHitTest(Point p)
		{
			
			bool status = false;
			if ((!m_userDrawn) && (m_dayImage!=null) && (Month.EnableImageClick))
			{				  
				Region r = new Region(m_imageRect);
				if (r.IsVisible(p))
					status = true;
				else
					status = false;
				r.Dispose();
			}

			return status;

		}

		internal bool HitTest(Point p)
		{
						
			if (ImageHitTest(p))
				m_calendar.Cursor = Cursors.Hand;
			else
				m_calendar.Cursor =Cursors.Arrow; 
			
			if (m_region.IsVisible(p))
				return true;
			else
				return false;
		}

		#endregion
		
	}

	#region DayEventArgs
	
	public class DayEventArgs : EventArgs
	{
		#region Class Data
		
		string m_date;
		
		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the DayClickEventArgs class with default settings
		/// </summary>
		public DayEventArgs()
		{
			m_date = DateTime.Today.ToShortDateString();
		}

		public DayEventArgs(string date)
		{
			this.m_date = date;
		}

		#endregion


		#region Properties

		public string Date
		{
			get
			{
				return this.m_date; 
			}
		}

		#endregion
	}


	#endregion

	#region DayClickEventArgs
	
	public class DayClickEventArgs : EventArgs
	{
		#region Class Data
			
		private string m_date;
		private MouseButtons m_button;
        private int m_dayX;
        private int m_dayY;
        private int m_x;
        private int m_y;
        private Rectangle m_dayRect;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the DayClickEventArgs class with default settings
		/// </summary>
		public DayClickEventArgs()
		{
			m_date = DateTime.Today.ToShortDateString();
			m_button = MouseButtons.Left;
		}

		public DayClickEventArgs(string date, MouseButtons button,int dayX,int dayY,int x,int y,Rectangle rect)
		{
			m_date =date;
			m_button = button;
            m_dayX = dayX;
            m_dayY = dayY;
            m_x = x;
            m_y = y;
            m_dayRect = rect;
		}

		#endregion


		#region Properties

		public string Date
		{
			get
			{
				return this.m_date;
			}
		}

        public Rectangle DayRectangle
        {
            get
            {
                return m_dayRect;
            }
        }

		public MouseButtons Button
		{
			get
			{
				return this.m_button; 
			}
		}

        public int DayX
        {
            get
            {
                return m_dayX;
            }
        }

        public int DayY
        {
            get
            {
                return m_dayY;
            }
        }

        public int X
        {
            get
            {
                return m_x;
            }
        }

        public int Y
        {
            get
            {
                return m_y;
            }
        }



		#endregion
	}


	#endregion

	#region DaySelectedEventArgs
	
	public class DaySelectedEventArgs : EventArgs
	{
		#region Class Data
		
		private string[] m_days;
		
		#endregion

		#region Constructor
	

		public DaySelectedEventArgs(string[] days)
		{
			this.m_days = days;
		}

		#endregion


		#region Properties

		public string[] Days
		{
			get
			{
				return this.m_days; 
			}
		}

		#endregion
	}


	#endregion

	#region DayDragDropEventArgs
	
	public class DayDragDropEventArgs : EventArgs
	{

		#region Class Data
			
		private string m_date;
		private int m_keyState;
		private IDataObject m_data;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the DayClickEventArgs class with default settings
		/// </summary>
		public DayDragDropEventArgs(IDataObject data, int keystate , string date)
		{
			m_date = date;
			m_data = data;
			m_keyState = keystate;	
		}
		
		#endregion


		#region Properties

		public string Date
		{
			get
			{
				return this.m_date;
			}
		}

		public int KeyState
		{
			get
			{
				return this.m_keyState;
			}
		}

		public IDataObject Data
		{
			get
			{
				return this.m_data;
			}
		}


		#endregion


	}


	#endregion

    #region DayStateChangedEventArgs

    public class DayStateChangedEventArgs : EventArgs
    {
        #region Class Data

        private string m_date;
        private mcDayState m_oldState;
        private mcDayState m_newState;
        private bool m_cancel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DayClickEventArgs class with default settings
        /// </summary>

        public DayStateChangedEventArgs(string date, mcDayState oldState, mcDayState newState)
        {
            m_date = date;
            m_newState = newState;
            m_oldState = oldState;
            m_cancel = false;
        }

        #endregion


        #region Properties

        public string Date
        {
            get
            {
                return this.m_date;
            }
        }

        public mcDayState OldState
        {
            get
            {
                return this.m_oldState;
            }
        }

        public mcDayState NewState
        {
            get
            {
                return this.m_newState;
            }
        }

        public bool Cancel
        {
            get
            {
                return m_cancel;
            }
            set
            {
                m_cancel = value;        
            }
        }

    


        #endregion
    }


    #endregion



    #region DayMouseMoveEventArgs

    public class DayMouseMoveEventArgs : EventArgs
    {
        #region Class Data

        private string m_date;
        private int m_dayX;
        private int m_dayY;
        private int m_x;
        private int m_y;
        private Rectangle m_dayRect;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DayClickEventArgs class with default settings
        /// </summary>
        public DayMouseMoveEventArgs()
        {
            m_date = DateTime.Today.ToShortDateString();
        }

        public DayMouseMoveEventArgs(string date, int dayX, int dayY, int x, int y, Rectangle rect)
        {
            m_date = date;
            m_dayX = dayX;
            m_dayY = dayY;
            m_x = x;
            m_y = y;
            m_dayRect = rect;
        }

        #endregion


        #region Properties

        public string Date
        {
            get
            {
                return this.m_date;
            }
        }

        public Rectangle DayRectangle
        {
            get
            {
                return m_dayRect;
            }
        }

        public int DayX
        {
            get
            {
                return m_dayX;
            }
        }

        public int DayY
        {
            get
            {
                return m_dayY;
            }
        }

        public int X
        {
            get
            {
                return m_x;
            }
        }

        public int Y
        {
            get
            {
                return m_y;
            }
        }



        #endregion
    }


    #endregion


}
