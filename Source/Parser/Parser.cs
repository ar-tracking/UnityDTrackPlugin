/* Unity DTrack Plugin: script Parser
 *
 * Parsing DTrack output data of one frame
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
using DTrack.DataObjects;
using DTrack.Util;
using UnityEngine;

namespace DTrack.Parser
{


    public static class RawParser
    {
        public static Packet Parse( string raw )
        {
            var packet = new Packet();
            var lines = raw.Split( Statics.LineSplit, StringSplitOptions.None );
            foreach ( var line in lines )
            {
                try
                {
                    if ( line.StartsWith( Statics.FormatFrame, StringComparison.CurrentCulture ) )
                    {
                        packet.Frame = FrameParser.Parse( line );
                    }
                    else if ( line.StartsWith( Statics.FormatBody6Dof, StringComparison.CurrentCulture ) )
                    {
                        packet.Body6D = Body6DofParser.Parse( line );
                    }
                    else if ( line.StartsWith( Statics.FormatFlystick, StringComparison.CurrentCulture ) )
                    {
                        packet.Flystick = BodyFlystickParser.Parse( line );
                    }
                }
                catch ( Exception e )
                {
                    Debug.LogError( "Error parsing line: " + line + Environment.NewLine + "Exception: " + e );
                }
            }

            return packet;
        }
    }


}  // namespace DTrack.Parser

