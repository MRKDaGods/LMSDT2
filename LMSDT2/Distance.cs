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
    public class Distance : Hack
    {
        ColorMode m_ColorMode;
        ColorModifierRainbow m_ColorModifierRainbow = new ColorModifierRainbow();
        ColorModifierAnimatedGradient m_ColorModifierAnimatedGradient = new ColorModifierAnimatedGradient(2);
        GUIStyle m_Style;

        public override int C => 1;
        public override int H => 2;

        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            if (o == 1)
                m_ColorMode = (ColorMode)v;
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
            if (m_Style == null)
                m_Style = new GUIStyle
                {
                    richText = true,
                    font = Font.CreateDynamicFontFromOSFont("Roboto-Regular", 27),
                    fontStyle = FontStyle.Bold,
                    fontSize = 27
                };
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
            foreach (AgentHumanDT agentHumanDT in context.LivingEnitites)
            {
                if (m_ColorMode == ColorMode.Visibility)
                    color = context.VisibleEntities.Contains(agentHumanDT) ? Color.green : Color.red;
                Vector3 spos = Camera.main.WorldToScreenPoint(agentHumanDT.transform.position + new Vector3(0f, 1.9f));
                if (spos.z > 0f)
                {
                    Vector2 gPos = GUIUtility.ScreenToGUIPoint(spos);
                    gPos.y = Screen.height - gPos.y;
                    string distStr = ((int)Vector3.Distance(context.LocalPlayer.transform.position, agentHumanDT.transform.position)).ToString() + "m";
                    if (m_ColorMode == ColorMode.Gradient)
                    {
                        string str = "<b><size=27>";
                        for (int i = 0; i < distStr.Length; i++)
                        {
                            Color now = Color.Lerp(m_ColorModifierAnimatedGradient.Colors[0, 0], m_ColorModifierAnimatedGradient.Colors[1, 0], (i + 1f) / distStr.Length);
                            str += string.Format("<color=#{0}>{1}</color>", now.ToHexString(), distStr[i]);
                        }
                        str += "</size></b>";
                        GUI.Label(new Rect(gPos.x, gPos.y, 200f, 200f), str, m_Style);
                    }
                    else GUI.Label(new Rect(gPos.x, gPos.y, 200f, 200f), string.Format("<b><color=#{0}><size=24>{1}</size></color></b>", color.ToHexString(),
                        distStr));
                }
            }
        }
    }
}
