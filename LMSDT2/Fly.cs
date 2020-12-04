/*
 * Copyright (c) 2020, Mohamed Ammar <mamar452@gmail.com>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
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
using System.Linq;
using System.Text;
using UnityEngine;

namespace LMSDT2
{
    public enum ScreenPart
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Right = 4,
        Left = 8,

        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }

    public class Fly : Hack
    {
        bool m_IsSwipe = true;
        bool m_SwipeValidated;
        bool m_NFly;
        float m_TargetY;
        float m_NDownTime;

        public override int C => 2;
        public override int H => 0;

        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            if (o == 1)
                m_IsSwipe = v == 0;
        }

        ScreenPart ExtractPart(Vector2 pos)
        {
            ScreenPart part = ScreenPart.None;
            if (pos.x > Screen.width / 2f)
                part |= ScreenPart.Right;
            else part |= ScreenPart.Left;
            if (pos.y > Screen.height / 2f)
                part |= ScreenPart.Bottom;
            else part |= ScreenPart.Top;
            return part;
        }

        public override void OnGUI(Context context)
        {
            if (context.LocalPlayer == null)
                return;

        }

        public override void OnUpdate(Context context)
        {
            if (context.LocalPlayer == null)
                return;
        }
    }
}
