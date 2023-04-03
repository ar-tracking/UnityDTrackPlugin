/* ART DTRACK Plugin for Unity Game Engine: ExampleFlystickListener.cs
 *
 * Example event listener routines to receive Flystick events.
 * For use in own scripts.
 *
 * Copyright (c) 2022-2023 Advanced Realtime Tracking GmbH & Co. KG
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

namespace DTrack.Examples
{


public class ExampleFlystickListener : MonoBehaviour
{
	private Color origColor;
	private Vector3 origLocalPosition;

	private float joystickX = 0.0f;
	private float joystickY = 0.0f;
	private float triggerValue = 0.0f;


	void Start()
	{
		origColor = GetComponent< Renderer >().material.color;
		origLocalPosition = this.transform.localPosition;
	}


	// Receive event on changed Flystick button.
	//
	// Event is invoked once, when button is pressed, and once, when it is released
	//
	// buttonId  : button ID (1 .. 8)
	// isPressed : button is pressed

	public void OnButtonChanged( int buttonId, bool isPressed )
	{
		if ( isPressed )
		{
			Color col;
			switch ( buttonId )
			{
				case 1 :
				case 3 :
					col = Color.black;
					break;
				case 2 :
					col = Color.red;
					break;
				case 4 :
					col = Color.blue;
					break;
				case 5 :
					col = Color.gray;
					break;
				default :
					col = Color.clear;
					break;
			}

			GetComponent< Renderer >().material.color = col;
		}
		else
		{
			GetComponent< Renderer >().material.color = origColor;
		}
	}


	// Receive event on changed Flystick joystick values.
	//
	// Event is invoked every time, when one of the joystick values changed
	//
	// joystickX : horizontal joystick value (-1.0 .. 1.0)
	// joystickY : vertical joystick value (-1.0 .. 1.0)

	public void OnJoystickChanged( float joystickX, float joystickY )
	{
		this.joystickX = joystickX;
		this.joystickY = joystickY;

		UpdateJoystickSphere();
	}


	// Receive event on changed other Flystick analog control values.
	//
	// Event is invoked every time, when the analog value changed
	//
	// analogId : analog value ID (3 .. )
	// val      : analog control value (-1.0 .. 1.0)

	public void OnTriggerChanged( int analogId, float val )
	{
		this.triggerValue = val;

		UpdateJoystickSphere();
	}


	private void UpdateJoystickSphere()
	{
		this.transform.localPosition = origLocalPosition +
				new Vector3( this.joystickX, this.triggerValue, this.joystickY );
	}
}


}  // namespace DTrack.Examples

