/* DTrackSDK in C#: DTrackSDK.cs
 * 
 * Functions to receive and process DTRACK UDP packets (ASCII protocol), as
 * well as to exchange DTrack2/DTRACK3 TCP command strings.
 *
 * Copyright (c) 2019-2022 Advanced Realtime Tracking GmbH & Co. KG
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * Version v0.1.0
 *
 * Purpose:
 *  - receives DTRACK UDP packets (ASCII protocol) and converts them into easier to handle data
 *  - sends and receives DTrack2/DTRACK3 commands (TCP)
 *  - DTRACK network protocol according to:
 *    'DTrack2 User Manual, Technical Appendix' or 'DTRACK3 Programmer's Guide'
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using DTrackSDK.Parsers;

namespace DTrackSDK
{


public class DTrackSDK
{
	private int _dataPort = 0;
	private UdpClient _dataChannel = null;
	private IPEndPoint _dataEndPoint = null;

	private string _dataBuffer;
	private Frame _frame;

	private string _lastErrorMessage;


	// Constructor, use for pure listening mode. Using this constructor, only a UDP receiver to get
	// tracking data from the Controller will be established. Please start measurement manually.

	public DTrackSDK( int dataPort )
	{
		this.Initialize( dataPort );
	}

	private bool Initialize( int dataPort )
	{
		_dataPort = dataPort;
		_dataChannel = null;
		_dataEndPoint = null;

		if ( _dataPort != 0 )
		{
			try
			{
				var ep = new IPEndPoint( IPAddress.Any, _dataPort );
				_dataChannel = new UdpClient( ep );
			}
			catch ( Exception e )
			{
				_lastErrorMessage = Convert.ToString( e );

				_dataChannel = null;
				return false;
			}
		}

		_lastErrorMessage = "";
		return true;
	}


	// Destructor.

	~DTrackSDK()
	{
		if ( _dataChannel != null )
			_dataChannel.Close();
	}


	// Returns if UDP socket is open to receive tracking data on local machine.
	//
	// Needed to receive DTRACK UDP data, but does not guarantee this.
	// Especially in case no data is sent to this port.

	public bool IsDataInterfaceValid  => ( _dataChannel != null );


	// Get UDP data port where tracking data is received.

	public int DataPort  => _dataPort;


	// Receive and process one tracking data packet.
	//
	// This method waits until a data packet becomes available, but no longer
	// than the timeout. Updates internal data structures.

	public bool Receive()
	{
		_frame = null;
		_dataBuffer = null;

		if ( _dataChannel == null )  return false;

		byte[] packet = null;
		try
		{
			packet = _dataChannel.Receive( ref _dataEndPoint );

			_dataBuffer = Encoding.ASCII.GetString( packet );
			_frame = RawParser.Parse( _dataBuffer );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = Convert.ToString( e );
			return false;
		}

		return ( _frame != null );
	}


	// Receive and process one tracking data packet (asynchronous variant).
	//
	// This method waits until a data packet becomes available. Updates internal data structures.

	public async Task< bool > ReceiveAsync()
	{
		_frame = null;
		_dataBuffer = null;

		if ( _dataChannel == null )  return false;

		try
		{
			UdpReceiveResult res = await _dataChannel.ReceiveAsync();
			_dataEndPoint = res.RemoteEndPoint;

			_dataBuffer = Encoding.ASCII.GetString( res.Buffer );
			_frame = RawParser.Parse( _dataBuffer );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = Convert.ToString( e );
			return false;
		}

		return ( _frame != null );
	}


	// Get current frame of tracking data.

	public Frame GetFrame()
	{
		return _frame;
	}


	// Get current content of the UDP buffer.

	public string GetBuf()
	{
		return _dataBuffer;
	}


	// Get last error message.

	public string GetLastErrorMessage()
	{
		return _lastErrorMessage;
	}
}


}  // namespace DTrackSDK

