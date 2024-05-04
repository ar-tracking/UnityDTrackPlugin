/* DTrackSDK in C#: DTrackSDK.cs
 * 
 * Functions to receive and process DTRACK UDP packets (ASCII protocol).
 *
 * Copyright (c) 2019-2024 Advanced Realtime Tracking GmbH & Co. KG
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
 * Version v0.2.0
 *
 * Purpose:
 *  - receives DTRACK UDP packets (ASCII protocol) and converts them into easier to handle data
 *  - sends DTRACK3 feedback data commands (UDP)
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
	private int _dataTimeout = Statics.DEFAULT_UDP_TIMEOUT;
	private IPEndPoint _dataEndPoint = null;
	private IPEndPoint _udpSenderEndPoint = null;

	private Parser _parser = new Parser();

	private string _dataBuffer;
	private Frame _frame;

	private string _lastErrorMessage;

	private UdpClient _feedbackChannel = null;


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
		_udpSenderEndPoint = null;
		_feedbackChannel = null;

		if ( _dataPort != 0 )
		{
			try
			{
				_dataChannel = new UdpClient();
				_dataChannel.ExclusiveAddressUse = true;

				var ep = new IPEndPoint( IPAddress.Any, _dataPort );
				_dataChannel.Client.Bind( ep );
				_dataChannel.Client.ReceiveTimeout = _dataTimeout;
				_dataChannel.Client.SendTimeout = _dataTimeout;
			}
			catch ( Exception e )
			{
				_lastErrorMessage = e.Message;
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
		this.Close();
	}


	// Close connection to Controller, especially close all UDP sockets.

	public bool Close()
	{
		if ( _dataChannel != null )
		{
			_dataChannel.Close();
			_dataChannel = null;
		}

		if ( _feedbackChannel != null )
		{
			_feedbackChannel.Close();
			_feedbackChannel = null;
		}

		return true;
	}


	// Returns if UDP socket is open to receive tracking data on local machine.
	//
	// Needed to receive DTRACK UDP data, but does not guarantee this.
	// Especially in case no data is sent to this port.

	public bool IsDataInterfaceValid  => ( _dataChannel != null );


	// Get UDP data port where tracking data is received.

	public int DataPort  => _dataPort;


	// UDP timeout for receiving tracking data (in microseconds).

	public int DataTimeoutUS
	{
		get
		{ return _dataTimeout; }
		set
		{
			if ( value <= 0 )
			{
				_dataTimeout = Statics.DEFAULT_UDP_TIMEOUT;
			} else {
				_dataTimeout = value / 1000;
			}

			if ( _dataChannel != null )
			{
				_dataChannel.Client.ReceiveTimeout = _dataTimeout;
				_dataChannel.Client.SendTimeout = _dataTimeout;
			}

			if ( _feedbackChannel != null )  _feedbackChannel.Client.SendTimeout = _dataTimeout;
		}
	}


	// Enable UDP connection through a stateful firewall.
	//
	// In order to enable UDP traffic through a stateful firewall. Just necessary for listening modes, will be done
	// automatically for communicating mode. Default port is working just for DTrack3 v3.1.1 or newer.

	public bool EnableStatefulFirewallConnection( string senderHost, int senderPort = Statics.CONTROLLER_PORT_UDPSENDER )
	{
		try
		{
			IPAddress[] addrlist = Dns.GetHostAddresses( senderHost );
			if ( addrlist.Length == 0 )  return false;

			foreach ( IPAddress addr in addrlist )
			{
				if ( addr.AddressFamily == AddressFamily.InterNetwork )
				{
					_udpSenderEndPoint = new IPEndPoint( addr, senderPort );
					break;
				}
			}
		}
		catch ( Exception e )
		{
			_lastErrorMessage = e.Message;
			return false;
		}

		this.SendStatefulFirewallPacket();  // try enabling UDP connection

		return true;
	}


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
			_frame = _parser.Parse( _dataBuffer );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = e.Message;
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
			_frame = _parser.Parse( _dataBuffer );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = e.Message;
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


	// Send dummy UDP packet for stateful firewall.
	//
	// Sends a packet to the Controller, in order to enable UDP traffic through a stateful firewall.

	private bool SendStatefulFirewallPacket()
	{
		if ( _dataChannel == null )  return false;

		if ( _udpSenderEndPoint == null )  return false;

		int sent = 0;
		byte[] msg = Encoding.ASCII.GetBytes( "fw4dtsdkcs" );
		try
		{
			sent = _dataChannel.Send( msg, msg.Length, _udpSenderEndPoint );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = e.Message;
			return false;
		}

		return ( sent == msg.Length );
	}


	// Send tactile Fingertracking command to set feedback on a specific finger of a specific hand.

	public bool TactileFinger( int handId, int fingerId, float strength )
	{
		strength = Math.Max( 0.0f, Math.Min( strength, 1.0f ) );

		return this.SendFeedbackCommand( $"tfb 1 [{handId} {fingerId} 1.0 {strength}]\0" );
	}


	// Send tactile Fingertracking command to set feedback on all fingers of a specific hand.

	public bool TactileHand( int handId, float[] strength )
	{
		string s = $"tfb {strength.Length} ";

		for ( int i = 0; i < strength.Length; i++ )
		{
			float st = Math.Max( 0.0f, Math.Min( strength[ i ], 1.0f ) );
			s += $"[{handId} {i} 1.0 {st}]";
		}

		return this.SendFeedbackCommand( s + "\0" );
	}


	// Send tactile Fingertracking command to turn off tactile feedback on all fingers of a specific hand.

	public bool TactileHandOff( int handId, int numFinger )
	{
		float[] strength = new float[ numFinger ];

		for ( int i = 0; i < strength.Length; i++ )
			strength[ i ] = 0.0f;

		return this.TactileHand( handId, strength );
	}


	// Send Flystick feedback command to start a beep on a specific Flystick.

	public bool FlystickBeep( int flystickId, float durationMs, float frequencyHz )
	{
		return this.SendFeedbackCommand( $"ffb 1 [{flystickId} {( int )durationMs} {( int )frequencyHz} 0 0][]\0" );
	}


	// Send Flystick feedback command to start a vibration pattern on a specific Flystick.

	public bool FlystickVibration( int flystickId, int vibrationPattern )
	{
		return this.SendFeedbackCommand( $"ffb 1 [{flystickId} 0 0 {vibrationPattern} 0][]\0" );
	}


	// Send feedback command via UDP.

	private bool SendFeedbackCommand( string command )
	{
		if ( _feedbackChannel == null )
		{
			if ( _dataEndPoint == null )  return false;

			try
			{
				_feedbackChannel = new UdpClient( _dataEndPoint.Address.ToString(), Statics.CONTROLLER_PORT_FEEDBACK );
				                                  // establishes default remote host
				_feedbackChannel.Client.SendTimeout = _dataTimeout;
			}
			catch ( Exception e )
			{
				_lastErrorMessage = e.Message;
				_feedbackChannel = null;
				return false;
			}
		}

		int sent = 0;
		byte[] msg = Encoding.ASCII.GetBytes( command );
		try
		{
			sent = _feedbackChannel.Send( msg, msg.Length );
		}
		catch ( Exception e )
		{
			_lastErrorMessage = e.Message;
			return false;
		}

		return ( sent == msg.Length );
	}
}


}  // namespace DTrackSDK

