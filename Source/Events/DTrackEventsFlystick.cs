/* ART DTRACK Plugin for Unity Game Engine: DTrackEventsFlystick.cs
 *
 * Events sent by DTrackReceiverFlystick script.
 *
 * Copyright (c) 2019-2023 Advanced Realtime Tracking GmbH & Co. KG
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

using UnityEngine;
using UnityEngine.Events;

namespace DTrack.Events
{


// Event on changed Flystick button.
//
// Event is invoked once, when button is pressed, and once, when it is released
//
// int buttonId   : button ID (1 .. 8)
// bool isPressed : button is pressed

[ System.Serializable ]
public class FlystickButtonChangedEvent : UnityEvent< int, bool >
{}


// Event on changed Flystick joystick values.
//
// Event is invoked every time, when one of the joystick values changed
//
// float joystickX : horizontal joystick value (-1.0 .. 1.0)
// float joystickY : vertical joystick value (-1.0 .. 1.0)

[ System.Serializable ]
public class FlystickJoystickChangedEvent : UnityEvent< float, float >
{}


// Event on changed other Flystick analog control values.
//
// Event is invoked every time, when the analog value changed
//
// int analogId : analog value ID (3 .. )
// float val    : analog control value (-1.0 .. 1.0)

[ System.Serializable ]
public class FlystickAnalogChangedEvent : UnityEvent< int, float >
{}


// Event on pressed Flystick button (DEPRECATED).
//
// CAUTION: this event will be removed in some future version, don't use it
//
// int flystickId : Flystick ID (as seen in DTRACK)
// int buttonId   : button ID (1 .. 8)

[ System.Serializable ]
public class FlystickButtonPressedEvent : UnityEvent< int, int >
{}


}  // DTrack.Events

