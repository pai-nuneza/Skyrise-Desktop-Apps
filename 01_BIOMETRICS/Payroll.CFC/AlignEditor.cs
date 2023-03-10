using System;
using System.Windows.Forms;
using System.Windows.Forms.Design; 
using System.Drawing.Design; 
using System.Drawing;

namespace Payroll.CFC
{
	/// <summary>
	/// Summary description for ImageEditor.
	/// </summary>
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	internal class AlignEditor : System.Drawing.Design.UITypeEditor, IDisposable   
	{
		
		#region private members
		
		private bool disposed;

		#endregion

		#region properties
		
		private IWindowsFormsEditorService wfes;
		private mcItemAlign m_selectedAlign = mcItemAlign.Center;
		private AlignControl m_alignCtrl;
	
		#endregion
		
		#region constructor

		public AlignEditor()
		{
			m_alignCtrl = new AlignControl(); 
		}

		#endregion

		#region Methods

		#endregion
		
		#region overrides

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			wfes = (IWindowsFormsEditorService)	provider.GetService(typeof(IWindowsFormsEditorService));
			if((wfes == null) || (context == null))
				return null ;
			
			m_alignCtrl.Default = (mcItemAlign)value;
			// add listner for event
			m_alignCtrl.AlignChanged+=new AlignEventHandler(m_alignCtrl_AlignChanged);
			
			m_selectedAlign = (mcItemAlign)value;

			// show the popup as a drop-down
			wfes.DropDownControl(m_alignCtrl) ;
			
			// return the selection (or the original value if none selected)
			return m_selectedAlign;
		}

		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if(context != null && context.Instance != null ) 
			{
				return UITypeEditorEditStyle.DropDown ;
			}
			return base.GetEditStyle (context);
		}
		

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return false;
		}

		#endregion

		#region EventHandlers

		private void m_alignCtrl_AlignChanged(object sender, AlignEventArgs e)
		{
			m_selectedAlign = e.Align; 
			
			//remove listner
			m_alignCtrl.AlignChanged-=new AlignEventHandler(m_alignCtrl_AlignChanged);
			
			// close the drop-dwon, we are done
			wfes.CloseDropDown();
		}

		#endregion

		#region IDisposable Members
		
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					m_alignCtrl.Dispose();
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
	}
}
