using System;
using System.Runtime.InteropServices;  
using System.Reflection;
using System.Windows.Forms;

namespace Payroll.CFC
{
	
	#region Delegates

	internal delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

	#endregion
	
	/// <summary>
	/// Summary description for GlobalHook.
	/// </summary>
	internal class GlobalHook : IDisposable
	{
				

		#region class members

		private bool disposed;
		private int m_keyboardHook;
 
		private HookProc m_keyboardHookProcedure;
		
		#endregion

		#region Events

		public event KeyEventHandler KeyUp;
		public event KeyEventHandler KeyDown;
		public event KeyPressEventHandler KeyPress;

		#endregion
		
		#region Constructor

		public GlobalHook()
		{

		}

		#endregion

		#region IDisposable Members
		
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					RemoveKeyboardHook();		
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
	

		#region public methods
		
		public void InstallKeyboardHook()
		{
			try
			{
				// install Keyboard hook 
				if(m_keyboardHook == 0)
				{
					m_keyboardHookProcedure = new HookProc(KeyboardHookProc);
					m_keyboardHook = NativeMethods.SetWindowsHookEx( NativeMethods.WH_KEYBOARD_LL,
						m_keyboardHookProcedure, 
						Marshal.GetHINSTANCE(
						Assembly.GetExecutingAssembly().GetModules()[0]),
						0);
				}
			}
			catch(Exception)
			{

			}
		}

		public void RemoveKeyboardHook()
		{
			bool retKeyboard = true;
			
			try
			{

				if(m_keyboardHook != 0)
				{
					retKeyboard = NativeMethods.UnhookWindowsHookEx(m_keyboardHook);
					m_keyboardHook = 0;
				}
			}
			catch(Exception)
			{

			}
		}


		#endregion
		
		#region private methods

		private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
		{
			// it was ok and someone listens to events
			if ((nCode >= 0) && (KeyDown!=null || KeyUp!=null || KeyPress!=null))
			{
				NativeMethods.KeyboardHookStruct MyKeyboardHookStruct = (NativeMethods.KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(NativeMethods.KeyboardHookStruct));
				// KeyDown
				if (  (KeyDown!=null) && ( wParam ==NativeMethods.WM_KEYDOWN || wParam==NativeMethods.WM_SYSKEYDOWN ))
				{
					Keys keyData=(Keys)MyKeyboardHookStruct.vkCode;
					KeyEventArgs e = new KeyEventArgs(keyData);
					this.KeyDown(this, e);
				}
				
				// KeyPress
				if ( (KeyPress!=null) && (wParam ==NativeMethods.WM_KEYDOWN) )
				{
					byte[] keyState = new byte[256];
					NativeMethods.GetKeyboardState(keyState);

					byte[] inBuffer= new byte[2];
					if (NativeMethods.ToAscii(MyKeyboardHookStruct.vkCode,
						MyKeyboardHookStruct.scanCode,
						keyState,
						inBuffer,
						MyKeyboardHookStruct.flags)==1) 
					{
						KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
						KeyPress(this, e);
					}
				}
				
				// KeyUp
				if ( (KeyUp!=null) && ( wParam ==NativeMethods.WM_KEYUP || wParam==NativeMethods.WM_SYSKEYUP ))
				{
					Keys keyData=(Keys)MyKeyboardHookStruct.vkCode;
					KeyEventArgs e = new KeyEventArgs(keyData);
					KeyUp(this, e);
				}

			}
			return NativeMethods.CallNextHookEx(m_keyboardHook, nCode, wParam, lParam); 
		}	


		#endregion
	}
}
