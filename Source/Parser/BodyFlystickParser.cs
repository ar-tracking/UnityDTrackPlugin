/* Copyright (c) 2019, Advanced Realtime Tracking GmbH
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
        public static Dictionary<int, BodyFlystick> ParserFlystick(string raw)
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

                var metaSplit = sectionSplit[0].Split(Statics.NumberSplit);
                var threeSplit = sectionSplit[1].Split(Statics.NumberSplit);
                var matrixSplit = sectionSplit[2].Split(Statics.NumberSplit);
                var inputSplit = sectionSplit[3].Split(Statics.NumberSplit);

                var bodyId = Convert.ToInt32(metaSplit[0]);
                var confidence = Convert.ToSingle(metaSplit[1], CultureInfo.InvariantCulture);
                var buttonCount = Convert.ToInt32(metaSplit[2]);
                var controllerCount = Convert.ToInt32(metaSplit[3]);

                var posX = Convert.ToSingle(threeSplit[0], CultureInfo.InvariantCulture);
                var posY = Convert.ToSingle(threeSplit[1], CultureInfo.InvariantCulture);
                var posZ = Convert.ToSingle(threeSplit[2], CultureInfo.InvariantCulture);

                var m0 = Convert.ToSingle(matrixSplit[0], CultureInfo.InvariantCulture);
                var m1 = Convert.ToSingle(matrixSplit[1], CultureInfo.InvariantCulture);
                var m2 = Convert.ToSingle(matrixSplit[2], CultureInfo.InvariantCulture);
                var m3 = Convert.ToSingle(matrixSplit[3], CultureInfo.InvariantCulture);
                var m4 = Convert.ToSingle(matrixSplit[4], CultureInfo.InvariantCulture);
                var m5 = Convert.ToSingle(matrixSplit[5], CultureInfo.InvariantCulture);
                var m6 = Convert.ToSingle(matrixSplit[6], CultureInfo.InvariantCulture);
                var m7 = Convert.ToSingle(matrixSplit[7], CultureInfo.InvariantCulture);
                var m8 = Convert.ToSingle(matrixSplit[8], CultureInfo.InvariantCulture);

                int[] buttons = null;
                float[] controllers = null;

                var buttonValueCount = 0;

                if (buttonCount > 0)
                {
                    buttonValueCount = (int) Math.Ceiling((float) buttonCount / (float) 32);
                    buttons = new int[buttonCount];
                    for (var i = 0; i < buttonValueCount; i++)
                    {
                        buttons[i] = Convert.ToInt32(inputSplit[i]);
                    }
                }

                if (controllerCount > 0)
                {
                    controllers = new float[controllerCount];
                    for (int i = buttonValueCount; i < (controllerCount + buttonValueCount); i++)
                    {
                        controllers[i - buttonValueCount] = Convert.ToSingle(inputSplit[i], CultureInfo.InvariantCulture);
                    }
                }

                bodies.Add(bodyId,
                    new BodyFlystick(bodyId, confidence, posX, posY, posZ, m0, m1, m2, m3, m4, m5, m6,
                        m7, m8, buttons, buttonCount, controllers, controllerCount));
            }

            return bodies;
        }
    }
}
