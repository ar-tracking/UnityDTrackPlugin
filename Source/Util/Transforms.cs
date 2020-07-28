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

using UnityEngine;

namespace DTrack.Util
{
    public class Position3
    {
        // Position p (unit m):
        //  p := [ x, y, z ]
        public float x;
        public float X => x;
        public float y;
        public float Y => y;
        public float z;
        public float Z => z;

        public Position3( float p0, float p1, float p2 )
        {
            this.x = p0 * Statics.MM_TO_M;
            this.y = p1 * Statics.MM_TO_M;
            this.z = p2 * Statics.MM_TO_M;
        }

        public Vector3 ToUnityPosition()
        {
            //swap axis as DTrack uses right-handed world space
            return new Vector3( this.x, this.z, this.y );
        }
    }
    public class Rotation3x3
    {
        // Rotation matrix R:
        //       [ r00, r01, r02 ]
        //  R := [ r10, r11, r12 ]
        //       [ r20, r21, r22 ]
        public float r00;
        public float r10;
        public float r20;
        public float r01;
        public float r11;
        public float r21;
        public float r02;
        public float r12;
        public float r22;

        public Rotation3x3( float r00, float r10, float r20, float r01, float r11, float r21, float r02, float r12, float r22 )
        {
            this.r00 = r00;
            this.r10 = r10;
            this.r20 = r20;
            this.r01 = r01;
            this.r11 = r11;
            this.r21 = r21;
            this.r02 = r02;
            this.r12 = r12;
            this.r22 = r22;
        }

// jd        public Quaternion ToUnityQuaternion()
// jd        {
// jd                float w  = -Mathf.Sqrt( Mathf.Max(0.0f, 1.0f + r00 + r11 + r22)) / 2.0f;
// jd
// jd                float x  =  Mathf.Sqrt( Mathf.Max(0.0f, 1.0f + r00 - r11 - r22)) / 2.0f;
// jd                x *= -Mathf.Sign( x * (r21 - r12));
// jd
// jd                float y  =  Mathf.Sqrt( Mathf.Max(0.0f, 1.0f - r00 + r11 - r22)) / 2.0f;
// jd                y *= -Mathf.Sign( y * (r02 - r20));
// jd
// jd                float z  =  Mathf.Sqrt( Mathf.Max(0.0f, 1.0f - r00 - r11 + r22)) / 2.0f;
// jd                z *= -Mathf.Sign( z * (r10 - r01));
// jd
// jd                return new Quaternion( x, z, y, -w );
// jd        }

        public Quaternion ToUnityQuaternion()
        {
            float t;
            float x, y, z, w;

            if ( r22 < 0.0f )
            {
                if ( r00 > r11 )
                {
                    t = 1.0f + r00 - r11 - r22;
                    x = t; y = r01 + r10; z = r20 + r02; w = r12 - r21;
                }
                else
                {
                    t = 1.0f - r00 + r11 - r22;
                    x = r01 + r10; y = t; z = r12 + r21; w = r20 - r02;
                }
            }
            else
            {
                if ( r00 < -r11 )
                {
                    t = 1.0f - r00 - r11 + r22;
                    x = r20 + r02; y = r12 + r21; z = t; w = r01 - r10;
                }
                else
                {
                    t = 1.0f + r00 + r11 + r22;
                    x = r12 - r21; y = r20 - r02; z = r01 - r10; w = t;
                }
            }

            float s = 0.5f / Mathf.Sqrt( Mathf.Max( 0.0f, t ) );
            return new Quaternion( s*x, s*z, s*y, s*w );
        }
    }
}

