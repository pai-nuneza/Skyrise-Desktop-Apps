using System;

namespace Payroll.CFC
{
	public class SelectionArea
	{
		
		#region Private members
		
		int m_selBegin;
		int m_selEnd;
		DateTime m_selBeginDate;
		DateTime m_selEndDate;
		Month m_month;

		#endregion

		#region Constructor
		
		public SelectionArea()
		{
			m_selBegin = -1;
			m_selEnd = -1;
		}

		public SelectionArea(int begin,int end)
		{
			m_selBegin = begin;
			m_selEnd = end;
		}

		public SelectionArea(int begin,int end, Month month)
		{
			m_selBegin = begin;
			m_selEnd = end;
			m_month = month;
		}

		public SelectionArea(Month month)
		{
			m_selBegin = -1;
			m_selEnd = -1;
			m_month = month;
		}
		
		#endregion

		#region properties
		
		public DateTime BeginDate
		{
			get
			{
				return m_selBeginDate;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return m_selEndDate;
			}
		}

		public int Begin
		{
			get
			{
				return m_selBegin;
			}
			set
			{
			    m_selBegin = value;		
				if (m_selBegin!=-1)
					m_selBeginDate = m_month.m_days[m_selBegin].Date; 
			}
		}

		public int End
		{
			get
			{
				
				return m_selEnd;
			}
			set
			{
				m_selEnd = value;		
				if (m_selEnd!=-1)
					m_selEndDate = m_month.m_days[m_selEnd].Date;
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
	
		#endregion

		#region Methods
		
		public new string ToString()
		{
			if ((m_selBegin==-1) || (m_selEnd==-1))
				return "Empty";
			else
				return m_selBegin.ToString()+":"+m_selEnd.ToString();
		}

		public string[] DaysInSelection()
		{
			string[] days = new string[0];
			
			

			return days;
		}

		#endregion

	}
}
