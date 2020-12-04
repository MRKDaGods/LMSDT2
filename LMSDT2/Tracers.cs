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
    public class Tracers : Hack
    {
        enum TracerStartPosition
        {
            w_2_0 = 0,
            w_2_h_2 = 1,
            w_2_h = 2
        }

        TracerStartPosition m_StartPosition = TracerStartPosition.w_2_h;
        ColorMode m_ColorMode;
        bool m_HighlightAutomationTarget;
        ColorModifierRainbow m_ColorModifierRainbow = new ColorModifierRainbow();
        ColorModifierAnimatedGradient m_ColorModifierAnimatedGradient = new ColorModifierAnimatedGradient(2);

        public override int C => 1;
        public override int H => 1;

        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            if (o == 1)
                m_StartPosition = (TracerStartPosition)v;
            else
                if (o == 2)
                m_ColorMode = (ColorMode)v;
        }

        public override void ReceiveMessageB(int o, bool b)
        {
            base.ReceiveMessageB(o, b);
            if (o == 3)
                m_HighlightAutomationTarget = b;
        }

        public override void OnUpdate(Context context)
        {
            if (m_ColorMode == ColorMode.Rainbow)
                m_ColorModifierRainbow.Update();
            else
                if (m_ColorMode == ColorMode.Gradient)
                m_ColorModifierAnimatedGradient.Update();
        }

        public override void OnGUI(Context context)
        {
            Color color = Color.white;
            switch (m_ColorMode)
            {
                case ColorMode.Fixed:
                    color = Color.red;
                    break;
                case ColorMode.Rainbow:
                    color = m_ColorModifierRainbow.Current;
                    break;
            }
            Vector2 startPos = m_StartPosition == TracerStartPosition.w_2_0 ? new Vector2(Screen.width / 2f, 0f) : m_StartPosition == TracerStartPosition.w_2_h ?
                    new Vector2(Screen.width / 2f, Screen.height) : new Vector2(Screen.width / 2f, Screen.height / 2f);
            foreach (AgentHumanDT agentHumanDT in context.LivingEnitites)
            {
                if (m_ColorMode == ColorMode.Visibility)
                    color = context.VisibleEntities.Contains(agentHumanDT) ? Color.green : Color.red;
                Vector3 tPos = Camera.main.WorldToScreenPoint(agentHumanDT.transform.position);
                if (tPos.z > 0f)
                {
                    Vector2 gPos = GUIUtility.ScreenToGUIPoint(tPos);
                    gPos.y = Screen.height - gPos.y;
                    if (m_ColorMode == ColorMode.Gradient)
                        GLDraw.DrawLine(startPos, gPos, m_ColorModifierAnimatedGradient.Colors[0, 0], m_ColorModifierAnimatedGradient.Colors[1, 0], 2f);
                    else
                        GLDraw.DrawLine(startPos, gPos, color, 2f);
                }
            }
        }
    }
}
