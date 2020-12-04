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
    public class Health : Hack
    {
        enum HealthLayout
        {
            Vertical = 0,
            Horizontal = 1
        }

        ColorMode m_ColorMode = ColorMode.Gradient;
        HealthLayout m_Layout;
        ColorModifierRainbow m_ColorModifierRainbow = new ColorModifierRainbow();
        ColorModifierAnimatedGradient m_ColorModifierAnimatedGradient = new ColorModifierAnimatedGradient(2);

        public override int C => 1;
        public override int H => 3;


        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            if (o == 1)
                m_ColorMode = (ColorMode)v;
            else
                if (o == 2)
                    m_Layout = (HealthLayout)v;
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
            Camera cam = Camera.main;
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
                Vector3 pos = agentHumanDT.transform.position;
                if (!((Boxes)HackManager.Instance.GetHack(1, 0)).Is2D)
                    pos.z += 0.8f;
                Vector3 bsPos = cam.WorldToScreenPoint(pos);
                pos.y += 1.7f;
                Vector3 tsPos = cam.WorldToScreenPoint(pos);
                if (bsPos.z > 0f && tsPos.z > 0f)
                {
                    Vector2 bgPos = GUIUtility.ScreenToGUIPoint(bsPos);
                    bgPos.y = Screen.height - bgPos.y;
                    Vector2 tgPos = GUIUtility.ScreenToGUIPoint(tsPos);
                    tgPos.y = Screen.height - tgPos.y;
                    float height = Mathf.Abs(bgPos.y - tgPos.y);
                    float width = height / 2f;
                    float distanceFromPivot = width / 2f;
                    if (m_Layout == HealthLayout.Horizontal)
                    {
                        GLDraw.DrawLine(new Vector2(bgPos.x - distanceFromPivot, bgPos.y + distanceFromPivot / 2f), new Vector2(bgPos.x + distanceFromPivot, bgPos.y + distanceFromPivot / 2f), new Color32(32, 32, 32, 255), distanceFromPivot / 4f);
                        if (m_ColorMode == ColorMode.Gradient)
                            GLDraw.DrawLine(new Vector2(bgPos.x - distanceFromPivot, bgPos.y + distanceFromPivot / 2f), new Vector2(bgPos.x - distanceFromPivot + 2f
                            * distanceFromPivot * (agentHumanDT.BlackBoard.Health / agentHumanDT.BlackBoard.RealMaxHealth), bgPos.y + distanceFromPivot / 2f), 
                            m_ColorModifierAnimatedGradient.Colors[0, 0], m_ColorModifierAnimatedGradient.Colors[1, 0], distanceFromPivot / 4f);
                        else GLDraw.DrawLine(new Vector2(bgPos.x - distanceFromPivot, bgPos.y + distanceFromPivot / 2f), new Vector2(bgPos.x - distanceFromPivot + 2f
                            * distanceFromPivot * (agentHumanDT.BlackBoard.Health / agentHumanDT.BlackBoard.RealMaxHealth), bgPos.y + distanceFromPivot / 2f), color, distanceFromPivot / 4f);
                        continue;
                    }
                    GLDraw.DrawLine(new Vector2(bgPos.x - 1.5f * distanceFromPivot, bgPos.y), new Vector2(tgPos.x - 1.5f * distanceFromPivot, tgPos.y), new Color32(32, 32, 32, 255), distanceFromPivot / 4f);
                    if (m_ColorMode == ColorMode.Gradient)
                        GLDraw.DrawLine(new Vector2(bgPos.x - 1.5f * distanceFromPivot, bgPos.y), new Vector2(tgPos.x - 1.5f * distanceFromPivot, bgPos.y - height * (agentHumanDT.BlackBoard.Health / agentHumanDT.BlackBoard.RealMaxHealth)),
                        m_ColorModifierAnimatedGradient.Colors[0, 0], m_ColorModifierAnimatedGradient.Colors[1, 0], distanceFromPivot / 4f);
                    else GLDraw.DrawLine(new Vector2(bgPos.x - 1.5f * distanceFromPivot, bgPos.y), new Vector2(tgPos.x - 1.5f * distanceFromPivot, bgPos.y - height * (agentHumanDT.BlackBoard.Health / agentHumanDT.BlackBoard.RealMaxHealth)),
                        color, distanceFromPivot / 4f);
                }
            }
        }
    }
}
