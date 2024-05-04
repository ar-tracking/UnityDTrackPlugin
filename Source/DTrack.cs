/* ART DTRACK Plugin for Unity Game Engine: DTrack.cs
 *
 * Main script providing DTRACK tracking data to Unity.
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
 * Version v1.1.3
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using DTrackSDK;
using DTrack.Util;

namespace DTrack
{


public class DTrack : MonoBehaviour
{
	[ Tooltip( "UDP port for incoming DTRACK tracking data" ) ]
	public int listenPort = 0;  // final default see OnValidate()

	[ Tooltip( "Optional hostname/IP of DTRACK Controller to enable UDP connection through stateful firewall" ) ]
	public string controllerHost;

	[ Serializable ]
	public enum DTrackCoordinates
	{
		[ InspectorName( "Powerwall" ) ]
		powerwall = 0,
		[ InspectorName( "Normal" ) ]
		normal = 1
	}

	[ Tooltip( "Room coordinates as used in DTRACK" ) ]
	public DTrackCoordinates dTrackCoordinates = DTrackCoordinates.normal;  // final default see OnValidate()

#if UNITY_EDITOR
	// Current value of DTRACK frame counter (just for debugging)
	private long currentFrameCounter = 0;
#endif

	// Game objects receiving tracking data from DTRACK
	private GameObject[] receivers = new GameObject[ 0 ];

	private IDTrackReceiver[] receiversDR = new IDTrackReceiver[ 0 ];

	private DTrackSDK.DTrackSDK sdk = null;

	private Thread thread;
	private bool runReceiveThread = false;


	void OnValidate()
	{
		if ( this.listenPort == 0 )  // first usage at all: change defaults
		{
			this.listenPort = 5000;
			this.dTrackCoordinates = DTrackCoordinates.powerwall;
		}
	}

	void Awake()
	{
		Converter.SetDefaultCoordinates();
		if ( this.dTrackCoordinates == DTrackCoordinates.normal )  Converter.SetOldCoordinates();
	}

	void Start()
	{
		this.sdk = new DTrackSDK.DTrackSDK( this.listenPort );
		if ( ! this.sdk.IsDataInterfaceValid )
		{
			Debug.Log( $"Cannot initialize SDK to receive DTRACK tracking data: {this.sdk.GetLastErrorMessage()}" );
			return;
		}

		if ( ! String.IsNullOrEmpty( this.controllerHost ) )
		{
			if ( ! this.sdk.EnableStatefulFirewallConnection( this.controllerHost ) )
				Debug.Log( $"Cannot send UDP packet to {this.controllerHost}" );
		}

		this.thread = new Thread( new ThreadStart( ReceiveThread ) );

		this.runReceiveThread = true;
		this.thread.Start();
	}

	void OnDestroy()
	{
		this.runReceiveThread = false;
		if ( this.thread != null )  this.thread.Abort();

		if ( this.sdk != null )
		{
			this.sdk.Close();
			this.sdk = null;
		}
	}


	// Register a DTrackReceiver game object to receive DTRACK tracking data.

	public void RegisterTarget( GameObject receiver )
	{
		var receiverslist = new List< GameObject >( this.receivers );
		receiverslist.Add( receiver );
		this.receivers = receiverslist.ToArray();

		updateReceiversDR();
	}

	// Unregister a DTrackReceiver game object.

	public void UnregisterTarget( GameObject receiver )
	{
		var receiverslist = new List< GameObject >( this.receivers );
		receiverslist.Remove( receiver );
		this.receivers = receiverslist.ToArray();

		updateReceiversDR();
	}

	// Update list of registered IDTrackReceiver components.

	private void updateReceiversDR()
	{
		var recs = new List< IDTrackReceiver >();

		foreach ( GameObject rec in this.receivers )
		{
			recs.Add( rec.GetComponent< IDTrackReceiver >() );
		}

		this.receiversDR = recs.ToArray();
	}


	// Thread to receive tracking data from DTRACK and to forward it to DTrackReceiver game objects.

	private async void ReceiveThread()
	{
		while ( this.runReceiveThread )
		{
			bool ok = await this.sdk.ReceiveAsync();
			if ( ok )
			{
				DTrackSDK.Frame frame = this.sdk.GetFrame();  // returns a new Frame object after every ReceiveAsync()

#if UNITY_EDITOR
				this.currentFrameCounter = frame.FrameCounter;
#endif

				foreach ( IDTrackReceiver rec in this.receiversDR )
				{
					if ( rec != null )
					{
						try
						{
							rec.ReceiveDTrackFrame( frame );
						}
						catch ( Exception e )
						{
							Debug.Log( $"Error handling frame: {e}" );
						}
					}
				}
			}
		}
	}
}


}  // namespace DTrack

