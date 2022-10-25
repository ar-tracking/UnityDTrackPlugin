/* Unity DTrack Plugin: DTrackReceiverFlystick.cs
 *
 * Script providing DTRACK Flystick data to a GameObject.
 *
 * Copyright (c) 2020-2022 Advanced Realtime Tracking GmbH & Co. KG
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
 */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using DTrackSDK;
using DTrack;
using DTrack.Util;
using DTrack.Events;

namespace DTrack
{


public class DTrackReceiverFlystick : DTrackReceiver
{
	[ Tooltip( "Flystick ID as seen in DTRACK" ) ]
	public int flystickId;

	[ Tooltip( "Update position of this GameObject" ) ]
	public bool applyPosition = true;
	[ Tooltip( "Update rotation of this GameObject" ) ]
	public bool applyRotation = true;

	private int numberOfButtons = 0;  // number of buttons of this Flystick
	private bool[] isButtonPressed = new bool[ 0 ];  // current state of buttons of this Flystick

	private int numberOfAnalogs = 0;  // number of analog controls of this Flystick
	private float[] analogs = new float[ 0 ];  // current analog controls of this Flystick
	private bool[] isChangedAnalogs = new bool[ 0 ];

	[ Tooltip( "Events on changed button 1" ) ]
	public FlystickButtonChangedEvent button1ChangedEvent;
	[ Tooltip( "Events on changed button 2" ) ]
	public FlystickButtonChangedEvent button2ChangedEvent;
	[ Tooltip( "Events on changed button 3" ) ]
	public FlystickButtonChangedEvent button3ChangedEvent;
	[ Tooltip( "Events on changed button 4" ) ]
	public FlystickButtonChangedEvent button4ChangedEvent;
	[ Tooltip( "Events on changed button 5" ) ]
	public FlystickButtonChangedEvent button5ChangedEvent;
	[ Tooltip( "Events on changed button 6" ) ]
	public FlystickButtonChangedEvent button6ChangedEvent;
	[ Tooltip( "Events on changed button 7" ) ]
	public FlystickButtonChangedEvent button7ChangedEvent;
	[ Tooltip( "Events on changed button 8" ) ]
	public FlystickButtonChangedEvent button8ChangedEvent;

	[ Tooltip( "Events on changed joystick values" ) ]
	public FlystickJoystickChangedEvent joystickChangedEvent;
	[ Tooltip( "Events on changed trigger value" ) ]
	public FlystickAnalogChangedEvent analog3ChangedEvent;

	[ Tooltip( "Events on pressed button 1 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent1" ) ]
	public FlystickButtonPressedEvent button1PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 2 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent2" ) ]
	public FlystickButtonPressedEvent button2PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 3 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent3" ) ]
	public FlystickButtonPressedEvent button3PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 4 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent4" ) ]
	public FlystickButtonPressedEvent button4PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 5 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent5" ) ]
	public FlystickButtonPressedEvent button5PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 6 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent6" ) ]
	public FlystickButtonPressedEvent button6PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 7 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent7" ) ]
	public FlystickButtonPressedEvent button7PressedEventDeprecated;
	[ Tooltip( "Events on pressed button 8 (DEPRECATED)" ), FormerlySerializedAs( "buttonPressEvent8" ) ]
	public FlystickButtonPressedEvent button8PressedEventDeprecated;

	private FlystickButtonChangedEvent[] buttonChangedEvents;
	private FlystickButtonPressedEvent[] buttonPressedEventsDeprecated;


	void Start()
	{
		this.buttonChangedEvents = new FlystickButtonChangedEvent[ 8 ];
		this.buttonChangedEvents[ 0 ] = this.button1ChangedEvent;
		this.buttonChangedEvents[ 1 ] = this.button2ChangedEvent;
		this.buttonChangedEvents[ 2 ] = this.button3ChangedEvent;
		this.buttonChangedEvents[ 3 ] = this.button4ChangedEvent;
		this.buttonChangedEvents[ 4 ] = this.button5ChangedEvent;
		this.buttonChangedEvents[ 5 ] = this.button6ChangedEvent;
		this.buttonChangedEvents[ 6 ] = this.button7ChangedEvent;
		this.buttonChangedEvents[ 7 ] = this.button8ChangedEvent;

		this.buttonPressedEventsDeprecated = new FlystickButtonPressedEvent[ 8 ];
		this.buttonPressedEventsDeprecated[ 0 ] = this.button1PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 1 ] = this.button2PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 2 ] = this.button3PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 3 ] = this.button4PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 4 ] = this.button5PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 5 ] = this.button6PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 6 ] = this.button7PressedEventDeprecated;
		this.buttonPressedEventsDeprecated[ 7 ] = this.button8PressedEventDeprecated;

#if UNITY_EDITOR
		for ( int i = 0; i < 8; i++ )
		{
			if ( this.buttonChangedEvents[ i ] == null )
				this.buttonChangedEvents[ i ] = new FlystickButtonChangedEvent();

			this.buttonChangedEvents[ i ].AddListener( this.TestButtonAlarm );
		}

		if ( this.joystickChangedEvent == null )  this.joystickChangedEvent = new FlystickJoystickChangedEvent();
		this.joystickChangedEvent.AddListener( this.TestJoystickAlarm );

		if ( this.analog3ChangedEvent == null )  this.analog3ChangedEvent = new FlystickAnalogChangedEvent();
		this.analog3ChangedEvent.AddListener( this.TestAnalogAlarm );
#endif
	}

	void OnEnable()
	{
		this.Register();
	}

	void OnDisable()
	{
		this.Unregister();
	}


	void Update()
	{
		DTrackSDK.Frame frame = GetDTrackFrame();  // ensures data integrity against DTrack class
		if ( frame == null )  return;  // no new tracking data
		if ( frame.Flysticks == null )  return;

		try
		{
			DTrackSDK.DTrackFlystick dtFlystick;
			if ( frame.Flysticks.TryGetValue( flystickId - 1, out dtFlystick ) )
			{
				if ( dtFlystick.IsTracked )
				{
					if ( this.applyPosition )
					{
						this.transform.position = ConvertPosition.ToUnity( dtFlystick.Loc );
					}

					if ( this.applyRotation )
					{
						this.transform.rotation = ConvertRotation.ToUnity( dtFlystick.Quaternion );
					}
				}

				// Process buttons:

				this.numberOfButtons = dtFlystick.NumButtons;

				if ( this.isButtonPressed.Length != this.numberOfButtons )
					this.isButtonPressed = new bool[ this.numberOfButtons ];

				for ( int i = 0; i < this.numberOfButtons; i++ )
				{
					bool b = dtFlystick.Buttons[ i ];

					if ( b != this.isButtonPressed[ i ] )
					{
						FlystickButtonChangedEvent ev = this.buttonChangedEvents[ i ];

						if ( ev != null )  ev.Invoke( i + 1, b );
					}

					if ( b )  // for deprecated events
					{
						FlystickButtonPressedEvent ev = this.buttonPressedEventsDeprecated[ i ];

						if ( ev != null )  ev.Invoke( this.flystickId, i + 1 );
					}

					this.isButtonPressed[ i ] = b;
				}

				// Process analogs:

				this.numberOfAnalogs = dtFlystick.NumAnalogs;

				if ( this.analogs.Length != this.numberOfAnalogs )
				{
					this.analogs = new float[ this.numberOfAnalogs ];
					this.isChangedAnalogs = new bool[ this.numberOfAnalogs ];
				}

				for ( int i = 0; i < this.numberOfAnalogs; i++ )
				{
					float a = dtFlystick.Analogs[ i ];

					this.isChangedAnalogs[ i ] = ( a != this.analogs[ i ] );
					this.analogs[ i ] = a;
				}

				if ( ( this.joystickChangedEvent != null ) && ( this.numberOfAnalogs >= 2 ) )
				{
					if ( this.isChangedAnalogs[ 0 ] || this.isChangedAnalogs[ 1 ] )
						this.joystickChangedEvent.Invoke( this.analogs[ 0 ], this.analogs[ 1 ] );
				}

				if ( ( this.analog3ChangedEvent != null ) && ( this.numberOfAnalogs >= 3 ) )
				{
					if ( this.isChangedAnalogs[ 2 ] )
						this.analog3ChangedEvent.Invoke( 3, this.analogs[ 2 ] );
				}
			}
		}
		catch ( Exception e )
		{
			Debug.Log( $"Error while moving object: {e}" );
		}
	}


#if UNITY_EDITOR
	private void TestButtonAlarm( int buttonId, bool isPressed )
	{
		Debug.Log( $"DTrackReceiverFlystick: button {buttonId} changed to {isPressed}" );
	}

	private void TestJoystickAlarm( float jx, float jy )
	{
		Debug.Log( $"DTrackReceiverFlystick: joystick changed {jx} {jy}" );
	}

	private void TestAnalogAlarm( int analogId, float jt )
	{
		Debug.Log( $"DTrackReceiverFlystick: analog {analogId} changed {jt}" );
	}
#endif
}


}  // namespace DTrack

