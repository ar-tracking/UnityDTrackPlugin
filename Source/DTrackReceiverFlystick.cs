/* Unity DTrack Plugin: script DTrackReceiverFlystick
 *
 * Providing DTrack Flystick data to a Game Object
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
using DTrack;
using DTrack.DataObjects;
using DTrack.DataObjects.Body;
using DTrack.Events;
using UnityEngine;
using UnityEngine.Events;

namespace DTrack
{


    public class DTrackReceiverFlystick : DTrackReceiver
    {
        [Tooltip("Enter Flystick ID as seen in DTrack")]
        public int flystickId;
        [Tooltip("Current value of DTrack frame counter")]
        public long frame;

        [Tooltip("Update position of this flystick")]
        public bool applyPosition = true;
        [Tooltip("Update rotation of this flystick")]
        public bool applyRotation = true;

        [ Tooltip( "Number of controllers as reported by DTrack" ) ]
        public int numberOfControllers = 0;

        [Tooltip("Joystick orientation X (jx)")]
        public float controller1;
        [Tooltip("Joystick orientation X (jx) scale")]
        public float controllerSpeed1 = 0.5f;
        [Tooltip("Joystick orientation Y (jy)")]
        public float controller2;
        [Tooltip("Joystick orientation Y (jy) scale")]
        public float controllerSpeed2 = 0.5f;
        [ Tooltip( "Trigger (jt)" ) ]
        public float controller3;

        [Tooltip("Number of buttons as reported by DTrack")]
        public int numberOfButtons = 0;

        [Tooltip("Checked on pressed button 1 (b1)")]
        public bool button1 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent1 = null;
        [Tooltip("Checked on pressed button 2 (b2)")]
        public bool button2 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent2 = null;
        [Tooltip("Checked on pressed button 3 (b3)")]
        public bool button3 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent3 = null;
        [Tooltip("Checked on pressed button 4 (b4)")]
        public bool button4 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent4 = null;
        [Tooltip("Checked on pressed button 5 (b5)")]
        public bool button5 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent5 = null;
        [Tooltip("Checked on pressed button 6 (b6)")]
        public bool button6 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent6 = null;
        [ Tooltip( "Checked on pressed button 7 (b7)" ) ]
        public bool button7 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent7 = null;
        [ Tooltip( "Checked on pressed button 8 (b8)" ) ]
        public bool button8 = false;
        public Events.FlystickButtonPressEvent buttonPressEvent8 = null;

        // Start is called before the first frame update
        void Start()
        {
            if ( this.buttonPressEvent1 == null ) {
                this.buttonPressEvent1 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent1.ButtonId = 1;
            this.buttonPressEvent1.FlystickId = this.flystickId;
            this.buttonPressEvent1.AddListener( this.Alarm );

            if ( this.buttonPressEvent2 == null ) {
                this.buttonPressEvent2 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent2.ButtonId = 2;
            this.buttonPressEvent2.FlystickId = this.flystickId;
            this.buttonPressEvent2.AddListener( this.Alarm );

            if ( this.buttonPressEvent3 == null ) {
                this.buttonPressEvent3 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent3.ButtonId = 3;
            this.buttonPressEvent3.FlystickId = this.flystickId;
            this.buttonPressEvent3.AddListener( this.Alarm );

            if ( this.buttonPressEvent4 == null ) {
                this.buttonPressEvent4 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent4.ButtonId = 4;
            this.buttonPressEvent4.FlystickId = this.flystickId;
            this.buttonPressEvent4.AddListener( this.Alarm );

            if ( this.buttonPressEvent5 == null ) {
                this.buttonPressEvent5 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent5.ButtonId = 5;
            this.buttonPressEvent5.FlystickId = this.flystickId;
            this.buttonPressEvent5.AddListener( this.Alarm );

            if ( this.buttonPressEvent6 == null ) {
                this.buttonPressEvent6 = new Events.FlystickButtonPressEvent();
            }
            this.buttonPressEvent6.ButtonId = 6;
            this.buttonPressEvent6.FlystickId = this.flystickId;
            this.buttonPressEvent6.AddListener( this.Alarm );

            if ( this.buttonPressEvent7 == null )
                this.buttonPressEvent7 = new Events.FlystickButtonPressEvent();

            this.buttonPressEvent7.ButtonId = 7;
            this.buttonPressEvent7.FlystickId = this.flystickId;
            this.buttonPressEvent7.AddListener( this.Alarm );

            if ( this.buttonPressEvent8 == null )
                this.buttonPressEvent8 = new Events.FlystickButtonPressEvent();

            this.buttonPressEvent8.ButtonId = 8;
            this.buttonPressEvent8.FlystickId = this.flystickId;
            this.buttonPressEvent8.AddListener( this.Alarm );
        }

        void OnEnable()
        {
            this.Register();
        }

        void OnDisable()
        {
            this.Unregister();
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                var child = transform.GetChild(0);
                if (this.button1)
                {
                    child.localPosition = new Vector3(0,0,0);
                }
                else
                {
                    var localPosition = child.localPosition;
                    localPosition = new Vector3(localPosition.x + (this.controller1 * this.controllerSpeed1), localPosition.y, localPosition.z + (this.controller2 * this.controllerSpeed2));
                    child.localPosition = localPosition;
                }
            }
            catch (Exception)
            {
                // ignore
            }

            if (this.button1) {
                this.EventTrigger( this.buttonPressEvent1 );
            }
            if (this.button2) {
                this.EventTrigger( this.buttonPressEvent2 );
            }
            if (this.button3) {
                this.EventTrigger( this.buttonPressEvent3 );
            }
            if (this.button4) {
                this.EventTrigger( this.buttonPressEvent4 );
            }
            if (this.button5) {
                this.EventTrigger( this.buttonPressEvent5 );
            }
            if (this.button6) {
                this.EventTrigger( this.buttonPressEvent6 );
            }

            if ( this.button7 )
                this.EventTrigger( this.buttonPressEvent7 );

            if ( this.button8 )
                this.EventTrigger( this.buttonPressEvent8 );
        }

        public override void ReceiveDTrackPacket( Packet packet )
        {
           
            if (packet.Flystick == null)
            {
                return;
            }

            try
            {
                frame = packet.Frame;

                BodyFlystick flystick;
                if (packet.Flystick.TryGetValue(flystickId - 1, out flystick))
                {
                    if (flystick.IsTracked)
                    {
                        if (this.applyPosition)
                        {
                            transform.position = flystick.GetPosition();
                        }

                        if (this.applyRotation)
                        {
                            transform.rotation = flystick.GetRotation();
                        }
                    }

                    this.numberOfControllers = flystick.GetControllerCount();
                    this.controller1 = 0.0f;
                    this.controller2 = 0.0f;
                    this.controller3 = 0.0f;

                    if ( this.numberOfControllers > 0 )
                    {
                        this.controller1 = flystick.GetControllerPosition( 0 );

                        if ( this.numberOfControllers > 1 )
                        {
                            this.controller2 = flystick.GetControllerPosition( 1 );

                            if ( this.numberOfControllers > 2 )
                            {
                                this.controller3 = flystick.GetControllerPosition( 2 );
                            }
                        }
                    }

                    this.numberOfButtons = flystick.GetButtonCount();
                    this.button1 = false;
                    this.button2 = false;
                    this.button3 = false;
                    this.button4 = false;
                    this.button5 = false;
                    this.button6 = false;
                    this.button7 = false;
                    this.button8 = false;

                    if (this.numberOfButtons > 0) {
                        this.button1 = flystick.IsButtonPressed(0);
                        if (this.numberOfButtons > 1) {
                            this.button2 = flystick.IsButtonPressed(1);
                            if (this.numberOfButtons > 2) {
                                this.button3 = flystick.IsButtonPressed(2);
                                if (this.numberOfButtons > 3) {
                                    this.button4 = flystick.IsButtonPressed(3);
                                    if (this.numberOfButtons > 4) {
                                        this.button5 = flystick.IsButtonPressed(4);
                                        if (this.numberOfButtons > 5) {
                                            this.button6 = flystick.IsButtonPressed(5);

                                            if ( this.numberOfButtons > 6 )
                                            {
                                                this.button7 = flystick.IsButtonPressed( 6 );

                                                if ( this.numberOfButtons > 7 )
                                                {
                                                    this.button8 = flystick.IsButtonPressed( 7 );
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error while moving object: " + e);
            }
        }

        public void EventTrigger( Events.FlystickButtonPressEvent ev )
        {
//            ev.FrameId = this.frame;
//            ev.Position = this.transform.position;

//            ev.Invoke( ev );
            ev.Invoke( ev.FlystickId, ev.ButtonId );
        }

//        public void Alarm( Events.FlystickButtonPressEvent ev )
//        {
//            Debug.Log( "alarm: #flystick="+ev.FlystickId + " #button="+ev.ButtonId + " #frame="+ev.FrameId + " @"+ev.Position );
//        }
        public void Alarm( int f, int b )
        {
            Debug.Log( "alarm: #flystick=" + f + " #button=" + b );
        }
    }


}  // namespace DTrack

