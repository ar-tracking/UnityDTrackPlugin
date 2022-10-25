/* Unity DTrack Plugin: DTrack.cs
 *
 * Main script providing DTRACK tracking data to Unity.
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
 * Version v1.1.0
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
	public int listenPort = 5000;

	// Current value of DTRACK frame counter
	private long currentFrameCounter = 0;

	// Game objects receiving tracking data from DTRACK
	private GameObject[] receivers = new GameObject[ 0 ];

	private IDTrackReceiver[] receiversDR = new IDTrackReceiver[ 0 ];

	private DTrackSDK.DTrackSDK sdk = null;

	private Thread thread;
	private bool runReceiveThread = false;


	void Start()
	{
		this.sdk = new DTrackSDK.DTrackSDK( listenPort );
		if ( ! this.sdk.IsDataInterfaceValid )
		{
			Debug.Log( $"Cannot initialize SDK to receive DTRACK tracking data: {this.sdk.GetLastErrorMessage()}" );
			return;
		}

		this.thread = new Thread( new ThreadStart( ReceiveThread ) );

		this.runReceiveThread = true;
		this.thread.Start();
	}

	void OnDestroy()
	{
		this.runReceiveThread = false;
		this.thread.Abort();
		this.sdk = null;
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

				this.currentFrameCounter = frame.FrameCounter;

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

