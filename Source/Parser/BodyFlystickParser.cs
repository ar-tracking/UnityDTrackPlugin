/* Unity DTrack Plugin: script BodyFlystickParser
 *
 * Parsing DTrack Flystick output data of one frame
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
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using DTrack.DataObjects.Body;
using DTrack.Util;
using UnityEngine;

namespace DTrack.Parser
{


    public static class BodyFlystickParser
    {
        public static Dictionary< int, BodyFlystick > Parse( string raw )
        {
            var bodyCountSplit = raw.Split(Statics.NumberSplit);
            var bodyCount = Convert.ToInt32(bodyCountSplit[1]);
            var countLength = 7 + bodyCountSplit[1].Length + bodyCountSplit[2].Length;

            if (bodyCount < 1)
            {
                return null;
            }

            var shortRaw = raw.Substring(countLength);
            var bodySplit = shortRaw.Split(Statics.BodySplit, StringSplitOptions.None);
            var bodies = new Dictionary<int, BodyFlystick>();

            foreach (var body in bodySplit)
            {
                var trimmedBody = body.Trim(Statics.TrimChars);
                var sectionSplit = trimmedBody.Split(Statics.SectionSplit, StringSplitOptions.None);

                string[] metaSection = sectionSplit[ 0 ].Split( Statics.NumberSplit );
                int bodyId = Convert.ToInt32( metaSection[ 0 ] );
                float confidence = Convert.ToSingle( metaSection[ 1 ], CultureInfo.InvariantCulture );
                int buttonCount = Convert.ToInt32( metaSection[ 2 ] );
                int controllerCount = Convert.ToInt32( metaSection[ 3 ] );

                string[] positionSection = sectionSplit[ 1 ].Split( Statics.NumberSplit );
                float posX = Convert.ToSingle( positionSection[ 0 ], CultureInfo.InvariantCulture );
                float posY = Convert.ToSingle( positionSection[ 1 ], CultureInfo.InvariantCulture );
                float posZ = Convert.ToSingle( positionSection[ 2 ], CultureInfo.InvariantCulture );

                string[] rotationSection = sectionSplit[ 2 ].Split( Statics.NumberSplit );
                float m0 = Convert.ToSingle( rotationSection[ 0 ], CultureInfo.InvariantCulture );
                float m1 = Convert.ToSingle( rotationSection[ 1 ], CultureInfo.InvariantCulture );
                float m2 = Convert.ToSingle( rotationSection[ 2 ], CultureInfo.InvariantCulture );
                float m3 = Convert.ToSingle( rotationSection[ 3 ], CultureInfo.InvariantCulture );
                float m4 = Convert.ToSingle( rotationSection[ 4 ], CultureInfo.InvariantCulture );
                float m5 = Convert.ToSingle( rotationSection[ 5 ], CultureInfo.InvariantCulture );
                float m6 = Convert.ToSingle( rotationSection[ 6 ], CultureInfo.InvariantCulture );
                float m7 = Convert.ToSingle( rotationSection[ 7 ], CultureInfo.InvariantCulture );
                float m8 = Convert.ToSingle( rotationSection[ 8 ], CultureInfo.InvariantCulture );

                string[] inputSection = null;
                if ( ( buttonCount > 0 ) || ( controllerCount > 0 ) )
                {
                    inputSection = sectionSplit[ 3 ].Split( Statics.NumberSplit );
                }

                int[] buttons = null;
                float[] controllers = null;

                int buttonValueCount = 0;
                if (buttonCount > 0)
                {
                    buttonValueCount = (int) Math.Ceiling((float) buttonCount / (float) 32);
                    buttons = new int[buttonCount];
                    for (var i = 0; i < buttonValueCount; i++)
                    {
                        buttons[ i ] = Convert.ToInt32( inputSection[ i ] );
                    }
                }

                if (controllerCount > 0)
                {
                    controllers = new float[controllerCount];
                    for (int i = buttonValueCount; i < (controllerCount + buttonValueCount); i++)
                    {
                        controllers[ i - buttonValueCount ] = Convert.ToSingle( inputSection[ i ], CultureInfo.InvariantCulture );
                    }
                }

                bodies.Add(bodyId,
                    new BodyFlystick(bodyId, confidence, posX, posY, posZ, m0, m1, m2, m3, m4, m5, m6,
                        m7, m8, buttons, buttonCount, controllers, controllerCount));
            }

            return bodies;
        }
    }


}  // namespace DTrack.Parser

