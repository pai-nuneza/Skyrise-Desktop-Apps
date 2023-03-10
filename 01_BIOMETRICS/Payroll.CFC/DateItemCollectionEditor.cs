using System;
using System.Collections; 
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;   
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace Payroll.CFC
{

	
	/// <summary>
	/// A custom CollectionEditor for editing DateItemCollection
	/// </summary>
	public class DateItemCollectionEditor : CollectionEditor
	{
		#region private class member

		private MonthCalendar m_calendar;
		private ITypeDescriptorContext m_context;

		#endregion

		
		#region Constructor

		public DateItemCollectionEditor(Type type) : base(type)
		{
			
		}

		#endregion
		
		#region overrides
		
		protected override void DestroyInstance(object instance)
		{
			base.DestroyInstance (instance);
		
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			m_context = context;
			//MonthCalendar originalControl = (MonthCalendar) context.Instance;
			//m_calendar = originalControl;

			object returnObject = base.EditValue(context, provider, value);
			
			DateItemCollection collection = returnObject as DateItemCollection; 
			if (collection !=null)
			{
				collection.ModifiedEvent();
			}
			
			return returnObject;
		}
		

		protected override object CreateInstance(Type itemType)
		{
			object dateItem = base.CreateInstance(itemType);
			
			MonthCalendar originalControl = (MonthCalendar) m_context.Instance;
			m_calendar = originalControl;	
			
			((DateItem) dateItem).Date = DateTime.Today;
			((DateItem) dateItem).Calendar = m_calendar;
			return dateItem;
		}

		#endregion
	}

}
