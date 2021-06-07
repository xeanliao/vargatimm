// GPSReader.cs
//
// Copyright (C) 2003 JW Hedgehog, Inc.  All rights reserved
//
// JW Hedgehog, Inc
// http://www.jwhh.com
// 
// Direct questions to mailto:jimw@jwhh.com
// 
// This code, comments and information are provided "AS IS" with
// no warrenty of any kind, either expressed or implied, including
// but not limited to the implied warranties of merchentability and/or
// fitness for a particular purpose
// ---------------------------------------------------------------------

using System;
using System.Data;
using System.Runtime.InteropServices ;
using System.Text ;
using System.Collections ;
using System.Threading ;
using System.IO;
using System.Windows.Forms;

namespace GTUWS
{
    public class GPSReader : Control
	{
		// *************************************************************
		//   Constructors
		// *************************************************************

		/// <summary>
		/// Default constructor
		/// At a minimum, will need to set the PortName and BaudRate properties before calling StartRead 
		/// </summary>
		public GPSReader()
		{
		}

		/// <summary>
		/// Constructor - Accepts COMM port name (COMx:)
		///  Will need to set the BaudRate properties before calling StartRead
		/// </summary>
		/// <param name="portName"></param>
		public GPSReader(string portName)
			: this()
		{
			_portName = portName ;
		}

		/// <summary>
		/// Constructor - Accepts COMM port name (COMx:) and BaudRate
		///  If default COMM port settings (NoParity, 8 bits/byte and OneStopBit) are acceptable,
		///  can call StartRead without setting any of the configuration properties
		/// </summary>
		/// <param name="portName"></param>
		/// <param name="baudRate"></param>
		public GPSReader(string portName, int baudRate)
			: this(portName)
		{
			_baudRate = baudRate ;
		}

		/// <summary>
		/// Constructor - verbose
		/// Provides full control over all COMM port settings
		/// </summary>
		/// <param name="portName"></param>
		/// <param name="baudRate"></param>
		/// <param name="parity"></param>
		/// <param name="byteSize"></param>
		/// <param name="stopBits"></param>
		public GPSReader(string portName, int baudRate, ParitySetting parity, byte byteSize, StopBitsSetting stopBits)
			: this(portName, baudRate)
		{
			_parity = parity ;
			_byteSize = byteSize ;
			_stopBits = stopBits ;
		}

		// *************************************************************
		//   Events
		// *************************************************************

		/// <summary>
		/// Fires each time a GPS message is received
		/// </summary>
		public event GPSEventHandler OnGPSMessage ;

		/// <summary>
		/// Fires when the background thread begins the read process
		/// </summary>
		public event EventHandler OnGPSReadStart ;

		/// <summary>
		/// Fires when the background thread exits the read process
		/// </summary>
		public event EventHandler OnGPSReadStop ;

		// *************************************************************
		//   Start/Stop Reading
		// *************************************************************

		/// <summary>
		/// Initiate GPS Reading 
		///  Actual reading done on a background thread - this method returns immediatly
		///
		/// Throws an error if either PortName or BaudRate not set
		/// </summary>
		public void StartRead()
		{
			// Verify that we know the port name and baud rate
			if (_baudRate == baudRateNotSet || _portName == portNameNotSet)
				throw new ApplicationException("<GPSReader> Must set Baud Rate & Port Name before opening the port") ;

			//Cursor.Current = Cursors.WaitCursor ;
			_readData = true ;

			_gpsReadThread = new Thread(new ThreadStart(this.GPSReadLoop)) ;
			_gpsReadThread.Start() ;
			//Cursor.Current = Cursors.Default ;
		}

		/// <summary>
		/// Terminate GPS Reading
		/// Sets _readData to false which exits the underlying read loop
		///  Also closes the COMM port which aborts any pending COMM port operations
		/// </summary>
		public void StopRead()
		{
			//Cursor.Current = Cursors.WaitCursor ;
			_readData = false ;
			Thread.Sleep(500) ;		// Give thread time to finish any pending work

			ClosePort() ;
			//Cursor.Current = Cursors.Default ;
		}

		// *************************************************************
		//   Port Setup and configuration
		// *************************************************************

		/// <summary>
		/// Set Port Name (COMx:)
		/// </summary>
		public string PortName
		{
			get { return _portName ;}
			set {_portName = value ;}
		}

		/// <summary>
		/// Set Baud Rate - No Default 
		/// </summary>
		public int BaudRate
		{
			get { return _baudRate ;}
			set {_baudRate = value ;}
		}

		/// <summary>
		/// Set Port Parity - defaults to NoParity
		/// </summary>
		public ParitySetting Parity
		{
			get { return _parity ;}
			set {_parity = value ;}
		}

		/// <summary>
		/// Set Port StopBits - defaults to OneStopBit 
		/// </summary>
		public StopBitsSetting StopBits
		{
			get { return _stopBits ;}
			set {_stopBits = value ;}
		}

		/// <summary>
		/// Set Port Byte Size (in bits) - defaults to 8
		/// </summary>
		public byte ByteSize
		{
			get { return _byteSize ;}
			set {_byteSize = value ;}
		}

		// *************************************************************
		//   Port Reading
		// *************************************************************

		/// <summary>
		/// Main Read Loop
		/// After openning the COMM Port, repeatedly retrieves a GPS sentence.
		///  If the sentence appears correct (starts with $GP) the GPSMessage event is fired
		/// If an exception should occur, the COMM Port is closed and the exception is propagated
		/// </summary>
		private void GPSReadLoop()
		{
			EventHandler GPSMessageHandler   = new EventHandler(this.DispatchGPSMessage) ;
			EventHandler GPSReadStartHandler = new EventHandler(this.DispatchGPSReadStart) ;
			EventHandler GPSReadStopHandler  = new EventHandler(this.DispatchGPSReadStop) ;

			OpenPort() ;

			// Signal beginning of read process - event fired on UI thread
			this.Invoke(GPSReadStartHandler) ;

			try
			{
				gpsSentence = ReadSentence() ;
				while (gpsSentence != null)	// will only be null if StopRead() is called
				{
					// If appears to be valid message, Signal GPS Message received - event fired on UI thread
					// Invoke blocks this thread until the method wrapped by the GPSMessageHandler returns.
					//  If this code is ever changed to an asynchronous method of execution then the gpsSentence
					//  variable will have to be protected against simultaneous access.
					if (gpsSentence.StartsWith("$GP"))
						this.Invoke(GPSMessageHandler) ; 

					gpsSentence = ReadSentence() ;
				}
			}
			catch (Exception e)
			{   // If any exception is thrown, close the COMM port and propagate the exception
				ClosePort() ;
				throw e ;
			}
			// Signal end of read process - event fired on UI thread
			this.Invoke(GPSReadStopHandler) ;

		}

		/// <summary>
		/// Handoff to the appopriate ReadSentence based on active ReadMode
		/// </summary>
		/// <returns>GPS Sentence</returns>
		private string ReadSentence()
		{
			string returnVal = string.Empty ;
			if (_activeReadMode == ReadMode.Message)
				returnVal = ReadSentence_Message() ;
			else
				returnVal = ReadSentence_Character() ;
			return returnVal ;
		}

		/// <summary>
		/// Retrieves the sentence and translates from ASCII to Unicode
		/// </summary>
		/// <returns>GPS Sentence</returns>
		private string ReadSentence_Message()
		{
			Byte[] buffer;
			Byte[] temp = new Byte[1] ;
			int numBytesRead = 0 ;
			Encoding asciiConverter = Encoding.ASCII ;

			// keep reading until we are told to stop or we get something 
			//  looped read should only occur if read errors are encountered
			bool portReturnedData = ReadPort_Message(MAX_MESSAGE, out buffer, out numBytesRead) ;
			while (_readData && ! portReturnedData)
				portReturnedData = ReadPort_Message(MAX_MESSAGE, out buffer, out numBytesRead) ;

			// if still reading, Translate from ASCII to Unicode
			return _readData ? asciiConverter.GetString(buffer, 0, numBytesRead) : null ;
		}

		/// <summary>
		/// Builds the sentence by doing single character reads then translates from ASCII to Unicode
		/// Additional code is used to adjust whether translation starts at the beginning of the string
		/// or skips the first character.  This had to be added because one of the GPS simulators we were
		/// using would send an extra character after the carriage-return (\n).
		/// </summary>
		/// <returns>GPS Sentence</returns>
		public string ReadSentence_Character()
		{
			Byte[] data = new Byte[MAX_MESSAGE] ;
			Byte temp = 0 ;
			int pos = 0 ;
			Encoding asciiConverter = Encoding.ASCII ;

			// Build the sentence until carriage-return encountered
			while(_readData && temp != endOfGPSSentenceMarker) 
			{
				temp = ReadPort_Character() ;
				data[pos++] = temp ;
			} 
			
			// Translation adjustment to handle extra character sent by some simulation programs
			int translateStartPos = 0 ;
			int translateCount = pos ;
			if (data[0] != (Byte) '$')
			{
				translateStartPos++ ;
				translateCount-- ;
			}

			// Perform translation
			return _readData ? asciiConverter.GetString(data, translateStartPos, translateCount) : null;
		}

		// *************************************************************
		//   Read mode related methods and properties
		// *************************************************************

		/// <summary>
		/// Indicates whether the COMM Port driver supports reading entire GPS messages at once
		/// The preferred way to read data is to let the driver signal when a carriage-return (\n)
		/// is received and then we read the whole message at once.  Experimentation has shown that 
		/// some of the GPS devices that simulate a serial port do not support this mode.  In that 
		/// case, we need to read the port data character-by-character.
		/// On the GPS devices tested, support for character notification can be verified by attempting
		/// to set the EvtChar (event character) member of the DCB structure using SetCommState and then
		/// reading the DCB back with GetCommState.  If character notification is supported the returned
		/// DCB will contain the EvtChar that was set with SetCommState.  If it is not supported, EvtChar
		/// will contain 0.  Because its not possible to test every GPS in existence there is no way to be
		/// 100% sure that this test will always work but on the devices tested it has been reliable.
		/// </summary>
		/// <returns></returns>
		public bool DriverSupportsMessageMode()
		{
			// Verify that we know the port name
			if (_portName == portNameNotSet)
				throw new ApplicationException("<DriverSupportsMessageMode> Must set Port Name before calling this method") ;

			uint localPortHandle = INVALID_FILE_HANDLE ;
			DCB dcb = new DCB() ;

			// Check to see if port is currently open
			bool openPortLocally = _portHandle == INVALID_FILE_HANDLE ;
			// If port not open then open it, otherwise use current handle
			localPortHandle = openPortLocally ? OpenPort_Raw(_portName) : _portHandle ;
			if (localPortHandle == INVALID_FILE_HANDLE)
				throw new ApplicationException("<DriverSupportsMessageMode> Invalid port: " + _portName) ;

			// Get current port settings
			GetCommState(localPortHandle, dcb) ;
			// Attempt to set event character
			dcb.EvtChar = endOfGPSSentenceMarker ;
			SetCommState(localPortHandle, dcb) ;
			// Read port settings back
			GetCommState(localPortHandle, dcb) ;

			// if port opened locally - close it
			if (openPortLocally)
				CloseHandle(localPortHandle) ;

			// Check to see if driver accepted event character
            return dcb.EvtChar == endOfGPSSentenceMarker ;
		}

		/// <summary>
		/// Preferred Read Mode to use - Defaults to Auto
		/// In Auto, reader will attempt message mode if driver supports it, otherwise uses character-by-character mode
		/// Undefined doesn't make sense in this usage, so if someone tries to set the value to Undefined, make it Auto
		/// </summary>
		public ReadMode PreferredReadMode
		{
			get {return _preferredReadMode;}
			set {_preferredReadMode = (value == ReadMode.Undefined) ? ReadMode.Auto : value ;	}
		}

		/// <summary>
		/// Read Mode that the GPSReader is actually using
		/// Set to Undefined until reading is started
		/// </summary>
		public ReadMode ActiveReadMode
		{
			get {return _activeReadMode;}
		}

		// *************************************************************
		//   Other COMM port related methods
		// *************************************************************

		/// <summary>
		/// This method doesn't actually have anything to do with GPS reading but is helpful for determining
		/// which ports are available on the device
		/// The code simply attempts to open (and immediatly close) all COMM ports between COM1: and COM9:, if it opens
		/// successfully, then it is added to the port list
		/// </summary>
		/// <returns>string array of available ports</returns>
		public static string[] GetPortList()
		{
			ArrayList portList = new ArrayList() ;
			uint hPort = INVALID_FILE_HANDLE ;

			// Walk list of possible ports
			for (int i = 1; i < 10; i++)
			{
				string port = "COM" + i.ToString() + ":" ;
				hPort = OpenPort_Raw(port) ;
				if (hPort != INVALID_FILE_HANDLE)
				{
					portList.Add(port) ;
					CloseHandle(hPort) ;
				}
			}

			// Convert to regular string array
			return (string []) portList.ToArray(typeof(string)) ;
		}

		#region Constants
		private const uint            GENERIC_READ				= 0x80000000;
		private const uint            OPEN_EXISTING				= 3;
		private const uint            INVALID_FILE_HANDLE		= 0xFFFFFFFF ;
		private const uint            EV_RXFLAG					= 0x0002 ;

		private const int             baudRateNotSet			= 0 ;
		private const string          portNameNotSet			= "PortNotSet" ;
		private const ParitySetting   defaultParity				= ParitySetting.NoParity;
		private const StopBitsSetting defaultStopBits			= StopBitsSetting.OneStopBit ;
		private const byte            defaultByteSize			= 8 ;

		private const sbyte			  endOfGPSSentenceMarker	= (sbyte)'\n' ;
		private const int             MAX_MESSAGE				= 256 ;
		#endregion

		#region Private fields

		private string _portName = portNameNotSet ;				// COMM Port Name - must end with ":"
		private int _baudRate = baudRateNotSet ;				// COMM Port Baud Rate
		private ParitySetting _parity = defaultParity ;			// COMM Port Parity - Defaults to NoParity
		private StopBitsSetting _stopBits = defaultStopBits ;	// COMM Port Stop Bits - Defaults to OneStopBit
		private byte _byteSize = defaultByteSize ;				// COM Port Byte Size - Defaults to 8 bits/byte

		private uint _portHandle = INVALID_FILE_HANDLE ;		// COMM Port File Handle
		private Thread _gpsReadThread ;							// Reader Thread
		private bool _readData = false ;						// Indicates whether read loop should continue

		// Read mode indicates how messages should be read.  The most efficient is to read whole messages
		//  from the COMM port but not all GPS drivers support this mode.  The alternative is to manually
		//  build the messages by reading a character at a time from the driver.
		private ReadMode _preferredReadMode = ReadMode.Auto ;	// COMM Port Preferred Read Mode - Auto lets the reader decide
		private ReadMode _activeReadMode = ReadMode.Undefined ; // COMM Port Read Mode being used - Undefined until reading starts

		// gpsSentence is populated by the GPS read thread and consumed by the UI Thread.  No lock
		//  is currently required because the GPS read thread uses Control.Invoke, which is synchronous, to signal the UI thread.
		// If the code is ever changed to use an asynchronous notification then gpsSentence will need to be protected from simulaneous
		//  access.  
		private string gpsSentence ;							// Represents the current GPS Sentence
		
		#endregion

		#region Cleanup
        ///// <summary>
        ///// Terminates GPS reading as part of dispose process
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        ClosePort() ;
        //    GC.SuppressFinalize(this) ;
        //}

		/// <summary>
		/// Catch all cleanup - if StopRead was never called and Dispose wasn't called then
		/// force shutdown as part of Garbage Collection - hopefully this method is never used
		/// </summary>
		~GPSReader()
		{
			ClosePort() ;
		}
		#endregion

		#region Event Dispatch Helper Methods
		/// <summary>
		/// Raise Events back to listeners
		/// These are helper methods to raise events to the UI layer.  Because updating UI elements from
		///  background threads is considered unsafe, these methods are initiated from the background reader
		///  thread using this.Invoke which causes these methods to run on the same thread that the GPSReader
		///  class was originally created on (usually the same thread as the application UI).  These methods
		///  now do a regular event fire to notify the UI of the actual event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DispatchGPSMessage(object sender, EventArgs e)
		{
			GPSEventArgs arg = new GPSEventArgs(this.gpsSentence) ;

			if (OnGPSMessage != null)
				OnGPSMessage(this, arg) ;
		}

		private void DispatchGPSReadStart(object sender, EventArgs e)
		{
			if (OnGPSReadStart != null)
				OnGPSReadStart(this, EventArgs.Empty) ;
		}

		private void DispatchGPSReadStop(object sender, EventArgs e)
		{
			if (OnGPSReadStop != null)
				OnGPSReadStop(this, EventArgs.Empty) ;
		}
		#endregion

		#region COMM Port Housekeeping Methods

		/// <summary>
		/// Open COMM Port
		/// Configures the Port communication values and timeouts
		/// </summary>
		private void OpenPort()
		{
			if (_portHandle == INVALID_FILE_HANDLE)  // Only open if not yet opened
			{
				_portHandle = OpenPort_Raw(_portName) ;		
				if (_portHandle == INVALID_FILE_HANDLE)
					throw new ApplicationException("<OpenPort> Unable to Open Port: " + _portName) ;

				System.Threading.Thread.Sleep(100) ;	// Some experiments showed problems when the state
														//  was set without a short pause after Open

				_activeReadMode = DetermineActiveReadMode() ;  // Determine Read Mode to use

				ConfigurePort(_baudRate, _parity, _byteSize, _stopBits) ;
				SetReadTimeOuts() ;
			}
		}

		/// <summary>
		/// Determine the active read mode to use based on the preferred read mode and driver capability
		/// Defaults to character mode because works with all drivers
		/// Will attempt message mode if Message or Auto preferred - will downgrade to Character if not supported
		/// </summary>
		/// <returns>ReadMode to use</returns>
		private ReadMode DetermineActiveReadMode()
		{
			ReadMode returnVal = ReadMode.Character ;	// Default to character - everyone supports it

			// If Message mode or Auto preferred - Set active to Message Mode if the driver supports it
			if (_preferredReadMode == ReadMode.Message || _preferredReadMode == ReadMode.Auto)
			{
				if (DriverSupportsMessageMode())
					returnVal = ReadMode.Message ;
			}

			return returnVal ;
		}

		/// <summary>
		/// Close COMM port
		/// </summary>
		private void ClosePort()
		{
			if (_portHandle != INVALID_FILE_HANDLE)
			{
				CloseHandle(_portHandle) ;
				_portHandle = INVALID_FILE_HANDLE ;
			}
		}

		/// <summary>
		/// Set COMM port configuration values
		///  Baud Rate, Parity (default: NoParity), Bits per Byte (default: 8) and Stop Bits (default: OneStopBit)
		///  If using MessageMode, the event character is set to carriage-return (\n) 
		/// </summary>
		/// <param name="baudRate"></param>
		/// <param name="parity"></param>
		/// <param name="byteSize"></param>
		/// <param name="stopBits"></param>
		private void ConfigurePort(int baudRate, ParitySetting parity, byte byteSize, StopBitsSetting stopBits)
		{
			DCB dcb = new DCB() ;
	
			dcb.BaudRate = (uint) baudRate ;
			dcb.Parity = parity ;
			dcb.ByteSize = byteSize;
			dcb.StopBits = stopBits;
			dcb.EvtChar = _activeReadMode == ReadMode.Message ? (sbyte)'\n' : (sbyte)0 ;

			SetCommState (_portHandle, dcb); 
		}

		/// <summary>
		/// Sets COMM Port read timeout values
		/// Hands off to the proper SetReadTimeOuts method based on the active read mode
		/// </summary>
		private void SetReadTimeOuts()
		{
			if (_activeReadMode == ReadMode.Message)
				SetReadTimeOuts_MessageMode() ;
			else
				SetReadTimeOuts_CharacterMode() ;
		}

		/// <summary>
		/// Sets COMM Port read timeout values
		/// Using minimal timeout values because we don't try to read from the COMM port
		///  until we are signaled that a carriage-return ('\n') has been recevied.  As a
		///  result we already know that we have all of the data so no need to wait to see what
		///  comes.
		/// </summary>
		private void SetReadTimeOuts_MessageMode()
		{
			COMMTIMEOUTS timeOuts = new COMMTIMEOUTS();

			timeOuts.ReadIntervalTimeout = 10 ; 
			timeOuts.ReadTotalTimeoutMultiplier = 0 ; 
			timeOuts.ReadTotalTimeoutConstant = 0 ; 
			timeOuts.WriteTotalTimeoutMultiplier = 0 ;
			timeOuts.WriteTotalTimeoutConstant = 0 ;
			SetCommTimeouts(_portHandle, timeOuts) ;
		}

		/// <summary>
		/// Use long timeout multiplier so that we wait efficiently between GPS messages.  The
		/// interval timeout doesn't matter for us because we read the data one character at a
		/// time in character mode.
		/// </summary>
		private void SetReadTimeOuts_CharacterMode()
		{
			COMMTIMEOUTS timeOuts = new COMMTIMEOUTS();

			timeOuts.ReadIntervalTimeout = 0 ; 
			timeOuts.ReadTotalTimeoutMultiplier = 2000 ; 
			timeOuts.ReadTotalTimeoutConstant = 0 ; 
			timeOuts.WriteTotalTimeoutMultiplier = 0 ;
			timeOuts.WriteTotalTimeoutConstant = 0 ;
			SetCommTimeouts(_portHandle, timeOuts) ;
		}
		#endregion

		#region Wrappers over Win32 Methods
		/// <summary>
		/// Wrapper method to simplify the opening of the COMM port
		///  Port name must be of the form COMx: - the colon is required
		/// </summary>
		/// <param name="portName"></param>
		/// <returns></returns>
		private static uint OpenPort_Raw(string portName)
		{
			return CreateFile(portName, GENERIC_READ, 0, 0, OPEN_EXISTING, 0, IntPtr.Zero);
		}

		/// <summary>
		/// Wrapper method to simplify reading a GPS sentence from the COMM port
		///  Method blocks until the COMM port driver receives a CR ('\n') or the COMM port is closed
		///  Return value indicates if any data was read - A 'false' return value occurs in one of 3 scenarios
		///  1. COMM port was closed, which usually indicates that StopRead has been called
		///  2. An error has occurred on the COMM port
		///  3. The read has timed out - this should never happen because we don't read until
		///     were notified by the COMM port driver that the carriage-return ('\n') has arrived
		/// </summary>
		/// <param name="hPort"></param>
		/// <param name="numBytes"></param>
		/// <param name="buffer"></param>
		/// <param name="numBytesRead"></param>
		/// <returns></returns>
		private bool ReadPort_Message(int numBytes, out Byte[] buffer, out int numBytesRead)
		{
			numBytesRead	= 0;
			buffer = new Byte[numBytes];
			uint eventMask = 0 ;
			int readRetVal = 0 ;
			bool retVal = false;

			SetCommMask(_portHandle, EV_RXFLAG) ;					// Indicate that we want to wait for the '\n'
			WaitCommEvent(_portHandle, out eventMask, IntPtr.Zero) ;// Wait...

			if ((eventMask & EV_RXFLAG) != 0)						// If received '\n' - do the read
			{														//  only other reason we'd be here is if the port was closed
				readRetVal = ReadFile(_portHandle, buffer, numBytes, ref numBytesRead, IntPtr.Zero) ;
				retVal = readRetVal != 0 && numBytesRead > 0 ;		// success only if non-zero return value and 
			}														// at least one byte read

			return retVal ;
		}

		/// <summary>
		/// Perform a single character read
		/// Internally manages timeouts & null characters
		/// Only returns 0 when StopRead is called.  Unline ReadPort_Message, it is reasonable to 
		/// expect that the ReadFile may occasionally timeout if there is a long delay between
		/// GPS messages.
		/// </summary>
		/// <returns>the character read or 0 if _readData is set to false</returns>
		private byte ReadPort_Character()
		{
			int		numBytesRead	= 0;
			Byte[]	buffer = new Byte[1];

			int retVal = ReadFile(_portHandle, buffer, 1, ref numBytesRead, IntPtr.Zero) ;
			while ( _readData && (ReadTimedOut(retVal, numBytesRead) || buffer[0] == 0) )
			{
				retVal = ReadFile(_portHandle, buffer, 1, ref numBytesRead, IntPtr.Zero) ;
			}

			return _readData ? buffer[0] : (byte)0 ;
		}

		/// <summary>
		/// Checks to see if ReadFile returned due to a timeout
		/// a ReadFile return value of non-zero (true) with number of bytes read of zero indicates timeout
		/// This method is used only by ReadPortCharacter
		/// </summary>
		/// <param name="readRetVal"></param>
		/// <param name="numBytesRead"></param>
		/// <returns></returns>
		private static bool ReadTimedOut(int readRetVal, int numBytesRead)
		{
			return readRetVal != 0 && numBytesRead == 0 ;
		}

		#endregion

		#region DllImports for Win32 methods
		/// <summary>
		/// Win32 method used to determine the COMM port's current settings
		/// </summary>
		/// <param name="hCommDev"></param>
		/// <param name="lpDCB"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		private static extern int GetCommState(uint hCommDev, DCB lpDCB) ;

		/// <summary>
		/// Win32 method used update the COMM portsettings 
		/// </summary>
		/// <param name="hCommDev"></param>
		/// <param name="lpDCB"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		private static extern int SetCommState(uint hCommDev, DCB lpDCB);

		/// <summary>
		/// Win32 method to read data from the COMM port
		/// </summary>
		/// <param name="hFile"></param>
		/// <param name="Buffer"></param>
		/// <param name="nNumberOfBytesToRead"></param>
		/// <param name="lpNumberOfBytesRead"></param>
		/// <param name="notUsedPassZero"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		static extern int ReadFile(uint hFile, Byte[] Buffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr notUsedPassZero ) ;

		/// <summary>
		/// Win32 method to open the COMM port
		/// </summary>
		/// <param name="lpFileName"></param>
		/// <param name="dwDesiredAccess"></param>
		/// <param name="dwShareMode"></param>
		/// <param name="lpSecurityAttributes"></param>
		/// <param name="dwCreationDisposition"></param>
		/// <param name="dwFlagsAndAttributes"></param>
		/// <param name="notUsedPassZero"></param>
		/// <returns></returns>
		[DllImport("coredll.dll",  EntryPoint="CreateFile")] 
		static extern uint CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr notUsedPassZero) ;

		/// <summary>
		/// Win32 method to close the COMM port
		/// </summary>
		/// <param name="hObject"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		static extern int CloseHandle(uint hObject);

		/// <summary>
		/// Win32 method used to sets read timeouts
		/// </summary>
		/// <param name="hFile"></param>
		/// <param name="lpCommTimeouts"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		static extern int SetCommTimeouts(uint hFile, COMMTIMEOUTS lpCommTimeouts);

		/// <summary>
		/// Win32 method used to set COMM port event masks
		///  In our code it is specifically used to identify that we want to be notified when
		///  the '\n' character is received
		/// </summary>
		/// <param name="hFile"></param>
		/// <param name="dwEvtMask"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		static extern int SetCommMask(uint hFile, uint dwEvtMask);

		/// <summary>
		/// Win32 method used to wait for COMM port events to signal
		///  In our code it is used to wait for the arrival of the '\n' character
		/// </summary>
		/// <param name="hFile"></param>
		/// <param name="lpEvtMask"></param>
		/// <param name="notUsedPassZero"></param>
		/// <returns></returns>
		[DllImport("coredll.dll")] 
		static extern int WaitCommEvent(uint hFile, out uint lpEvtMask, IntPtr notUsedPassZero);

		#endregion
	}

	/// <summary>
	/// GPS Message Structure
	///  Sent each time a GPS Message is received
	/// </summary>
	public class GPSEventArgs : EventArgs
	{
		// ** GPGGA Message Info **
		// Raw Message
		public readonly string MessageText  ;
		// Message Identifier (GPGGA, etc..)
		public readonly string MessageType ;
		// GMT Time
		public readonly double Time ;
		// Lat Stuff
		public readonly double Lat ;
		public readonly double LatOriginal ;
		public readonly string LatDirection ;
		// Lon Stuff
		public readonly double Lon ;
		public readonly double LonOriginal ;
		public readonly string LonDirection ;
		// Quality
		public readonly int Quality ;
		// Sat Count
		public readonly int NumSats ;

		// ** GPGSA Message Info **
		public readonly int FixType ;

		// ** GPRMC & GPVTG Message Info **
		public readonly double Bearing ;

		#region Sentence Parsing Methods
		/// <summary>
		/// Internal constructor which automatically parses the GPS sentence
		/// and populates the appropriate fields
		/// </summary>
		/// <param name="messageText"></param>
		internal GPSEventArgs(string messageText)
		{
			MessageText = messageText ;
			
			// Parse Message
			string[] tokens = MessageText.Split(new char[]{','}) ;
			if (tokens.Length > 0)
			{
				MessageType = tokens[0].Substring(1) ; // Strip leading $
				if (MessageType == "GPGGA")
				{
					// Time
					if (tokens[1].Length > 0)
						Time = Convert.ToDouble(tokens[1]) ;
					// Lat Stuff
					if (tokens[2].Length > 0)
						LatOriginal = Convert.ToDouble(tokens[2]) ;
					LatDirection = tokens[3].ToUpper() ;
					Lat = LonLatToDegrees(LatOriginal, LatDirection) ;
					// Lon Stuff
					if (tokens[4].Length > 0)
						LonOriginal = Convert.ToDouble(tokens[4]) ;
					LonDirection = tokens[5].ToUpper() ;
					Lon = LonLatToDegrees(LonOriginal, LonDirection) ;
					// Quality
					if (tokens[6].Length > 0)
						Quality = Convert.ToInt32(6) ;
					// Sat Count
					if (tokens[7].Length > 0)
						NumSats = Convert.ToInt32(tokens[7]) ;
				}
				else if (MessageType == "GPGSA")
				{
					if (tokens[2].Length > 0)
						FixType = Convert.ToInt32(tokens[2]) ;
				}
				else if (MessageType == "GPRMC")
				{
					if (tokens[8].Length > 0)
						Bearing = Convert.ToDouble(tokens[8]) ;
				}
				else if (MessageType == "GPVTG")
				{
					if (tokens[1].Length > 0)
						Bearing = Convert.ToDouble(tokens[1]) ;
				}
			}


		}

		/// <summary>
		/// Converts Longitude and Latitude to Degrees
		/// </summary>
		/// <param name="original"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		private double LonLatToDegrees(double original, string direction)
		{
			//	 y = m_dLatOrig / 100
			//   m_dLat = (((y - Int(y)) * 100) / 60) + Int(y)
			double temp = original / 100 ;
			int tempAsInt = (int) temp ;
			double result = (((temp - tempAsInt) * 100) / 60) + tempAsInt ;

			return direction == "S" || direction == "W" ? -result : result ;
		}

		#endregion
	}

	#region Port Configuration Enums - ParitySetting, StopBitsSetting and ReadMode
	/// <summary>
	/// Used to identify valid COMM port Parity Settings
	/// </summary>
	public enum ParitySetting
	{
		NoParity = 0,
		OddParity = 1,
		EvenParity = 2,
		MarkParity = 3,
		SpaceParity = 4
	}

	/// <summary>
	/// Used to identify COMM port Stop Bit Settings
	/// </summary>
	public enum StopBitsSetting
	{
		OneStopBit = 0,
		One5StopBits = 1,
		TwoStopBits = 2
	}

	/// <summary>
	/// Used to identify GPS driver read mode.  Used for both to indicate to the GPSReader
	/// the preferred read mode and also for the GPSReader to indicate what read mode it 
	/// is using.
	/// In the implementation setting a PreferredReadMode of Auto amd Message are basically
	/// equivalent because the GPSReader class will always verify that MessageMode is supported
	/// before attempting to use it. 
	/// </summary>
	public enum ReadMode
	{
		Undefined = 0,	// Only used to indicate that the GPSReader has not yet determined which mode to use
		Message,		// Indicates to read entire messages from the driver, if supported
		Character,		// Indicates to build messages character-by-character
		Auto			// Indicates the GPSReader should choose the read mode based on driver ability
	}
	#endregion

	#region GPSEventHandler decleration
	/// <summary>
	/// Delegate definition for the GPSMessage event
	/// </summary>
	public delegate void GPSEventHandler(object sender, GPSEventArgs arg) ;
	#endregion

	#region COMMTIMEOUTS definition
	[StructLayout(LayoutKind.Sequential)]
	internal class COMMTIMEOUTS
	{
		public uint ReadIntervalTimeout = 0 ;
		public uint ReadTotalTimeoutMultiplier = 0 ;
		public uint ReadTotalTimeoutConstant = 0 ;
		public uint WriteTotalTimeoutMultiplier = 0 ;
		public uint WriteTotalTimeoutConstant = 0 ;
	}
	#endregion

	#region DCB definition
	[StructLayout(LayoutKind.Sequential)]
	internal class DCB 
	{
		public DCB()
		{
			// Initialize the length of the structure.
			this.DCBlength = (uint)Marshal.SizeOf(this);
		}

		public   uint DCBlength = 0 ;
		public   uint BaudRate = 0 ;
		public   uint Control = 0 ;
		public   ushort wReserved = 0 ;
		public   ushort XonLim = 0 ;
		public   ushort XoffLim = 0 ;
		public   byte   ByteSize = 0 ;
		private  byte   _parity = 0 ;
		private  byte   _stopBits = 0 ;
		public   sbyte  XonChar = 0 ;
		public   sbyte  XoffChar = 0 ;
		public   sbyte  ErrorChar = 0 ;
		public   sbyte  EofChar = 0 ;
		public   sbyte  EvtChar = 0 ;
		public   ushort wReserved1 = 0 ;

		public ParitySetting Parity
		{
			get {return (ParitySetting) _parity;}
			set {_parity = (byte) value;}
		}

		public StopBitsSetting StopBits
		{
			get {return (StopBitsSetting) _stopBits;}
			set {_stopBits = (byte) value;}
		}
	}
	#endregion

}
