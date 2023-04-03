/* ART DTRACK Plugin for Unity Game Engine: ExampleCameraNavigator.cs
 *
 * Example event listener routine to navigate a camera using a Flystick joystick.
 * For use in own scripts.
 *
 * Copyright (c) 2023 Advanced Realtime Tracking GmbH & Co. KG
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


public class ExampleCameraNavigator : MonoBehaviour
{
	private const float speedLoc = 4.0f;  // maximum speed to move in meter per second
	private const float speedRot = 90.0f;  // maximum speed to rotate in degree per second

	private float joystickX = 0.0f;
	private float joystickY = 0.0f;


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
	}


	void FixedUpdate()
	{
		if ( this.joystickY != 0.0f )
		{
			float ds = this.joystickY * speedLoc * Time.fixedDeltaTime;

			this.transform.position += ds * Vector3.ProjectOnPlane( this.transform.rotation * Vector3.forward, Vector3.up );
		}

		if ( this.joystickX != 0.0f )
		{
			float da = this.joystickX * speedRot * Time.fixedDeltaTime;

			this.transform.rotation = Quaternion.Euler( 0.0f, da, 0.0f ) * this.transform.rotation;
		}
	}
}


}  // namespace DTrack.Examples

