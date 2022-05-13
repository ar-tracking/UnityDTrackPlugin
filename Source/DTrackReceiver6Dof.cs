/* Unity DTrack Plugin: script DTrackReceiver6Dof
 *
 * Providing DTrack standard 6DOF data to a Game Object
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
using UnityEngine;

namespace DTrack
{


    public class DTrackReceiver6Dof : DTrackReceiver
    {
        [Tooltip("Enter Body ID as seen in DTrack")]
        public int bodyId;
        [Tooltip("Current value of DTrack frame counter")]
        public long frame;

        [Tooltip("Update position of this 6dof target")]
        public bool applyPosition = true;
        [Tooltip("Update rotation of this 6dof target")]
        public bool applyRotation = true;


        // Start is called before the first frame update
        void Start()
        {
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
        }

        public override void ReceiveDTrackPacket( Packet packet )
        {
            if (packet.Body6D == null)
            {
                return;
            }

            try
            {
                Body6Dof body;

                if (packet.Body6D.TryGetValue(bodyId - 1, out body))
                {
                    if (applyPosition)
                    {
                        transform.position = body.GetPosition();
                    }

                    if (applyRotation)
                    {
                        transform.rotation = body.GetRotation();
                    }
                }

                frame = packet.Frame;
            }
            catch (Exception e)
            {
                Debug.Log("Error while moving object: " + e);
            }
            
            
        }
    }


}  // namespace DTrack

