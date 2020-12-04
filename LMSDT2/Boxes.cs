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
    public class Boxes : Hack
    {
        bool m_Is2D = true;
        ColorMode m_ColorMode;
        bool m_HighlightAutomationTarget;
        ColorModifierRainbow m_ColorModifierRainbow = new ColorModifierRainbow();
        ColorModifierAnimatedGradient m_ColorModifierAnimatedGradient = new ColorModifierAnimatedGradient();

        public override int C => 1;
        public override int H => 0;
        public bool Is2D => m_Is2D;

        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            switch (o)
            {
                case 1:
                    m_Is2D = v == 0;
                    break;
                case 2:
                    m_ColorMode = (ColorMode)v;
                    break;
            }
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
            Camera cam = Camera.main;
            foreach (AgentHumanDT agentHumanDT in context.LivingEnitites)
            {
                if (m_ColorMode == ColorMode.Visibility)
                    color = context.VisibleEntities.Contains(agentHumanDT) ? Color.green : Color.red;
                if (m_Is2D)
                {
                    Vector3 pos = agentHumanDT.transform.position;
                    Vector3 bsPoint = cam.WorldToScreenPoint(pos);
                    pos.y += 1.7f;
                    Vector3 tsPoint = cam.WorldToScreenPoint(pos);
                    if (bsPoint.z > 0f && tsPoint.z > 0f)
                    {
                        Vector3 bgPoint = GUIUtility.ScreenToGUIPoint(bsPoint);
                        bgPoint.y = Screen.height - bgPoint.y;
                        Vector3 tgPoint = GUIUtility.ScreenToGUIPoint(tsPoint);
                        tgPoint.y = Screen.height - tgPoint.y;
                        float width = Mathf.Abs(bgPoint.y - tgPoint.y) / 2f; //height dif / 2
                        float distFromPivot = width / 2f;
                        Vector2 bleft = new Vector2(bgPoint.x - distFromPivot, bgPoint.y);
                        Vector2 bright = new Vector2(bgPoint.x + distFromPivot, bgPoint.y);
                        Vector2 tleft = new Vector2(tgPoint.x - distFromPivot, tgPoint.y);
                        Vector2 tright = new Vector2(tgPoint.x + distFromPivot, tgPoint.y);
                        if (m_ColorMode == ColorMode.Gradient)
                        {
                            Color[,] c = m_ColorModifierAnimatedGradient.Colors;
                            GLDraw.DrawLine(bleft, bright, c[0, 0], c[0, 1], 2f);
                            GLDraw.DrawLine(bleft, tleft, c[1, 0], c[1, 1], 2f);
                            GLDraw.DrawLine(tleft, tright, c[2, 0], c[2, 1], 2f);
                            GLDraw.DrawLine(tright, bright, c[3, 0], c[3, 1], 2f);
                            continue;
                        }
                        GLDraw.DrawLine(bleft, bright, color, 2f);
                        GLDraw.DrawLine(bleft, tleft, color, 2f);
                        GLDraw.DrawLine(tleft, tright, color, 2f);
                        GLDraw.DrawLine(tright, bright, color, 2f);
                    }
                }
                else
                {
                    //           ____
                    //2d view   |    |
                    //          | .  |
                    //          |____|
                    Vector3 pos = agentHumanDT.transform.position;

                    Vector3 bfrPos = cam.WorldToScreenPoint(new Vector3(pos.x + 0.8f, pos.y, pos.z + 0.8f));
                    Vector3 bflPos = cam.WorldToScreenPoint(new Vector3(pos.x - 0.8f, pos.y, pos.z + 0.8f));

                    Vector3 bbrPos = cam.WorldToScreenPoint(new Vector3(pos.x + 0.8f, pos.y, pos.z - 0.8f));
                    Vector3 bblPos = cam.WorldToScreenPoint(new Vector3(pos.x - 0.8f, pos.y, pos.z - 0.8f));

                    pos.y += 1.7f;

                    Vector3 tfrPos = cam.WorldToScreenPoint(new Vector3(pos.x + 0.8f, pos.y, pos.z + 0.8f));
                    Vector3 tflPos = cam.WorldToScreenPoint(new Vector3(pos.x - 0.8f, pos.y, pos.z + 0.8f));

                    Vector3 tbrPos = cam.WorldToScreenPoint(new Vector3(pos.x + 0.8f, pos.y, pos.z - 0.8f));
                    Vector3 tblPos = cam.WorldToScreenPoint(new Vector3(pos.x - 0.8f, pos.y, pos.z - 0.8f));

                    if (bfrPos.z > 0f && bflPos.z > 0f && bbrPos.z > 0f && bblPos.z > 0f && tfrPos.z > 0f && tflPos.z > 0f && tbrPos.z > 0f && tblPos.z > 0f)
                    {
                        Vector2 _bfrPos = GUIUtility.ScreenToGUIPoint(bfrPos);
                        _bfrPos.y = Screen.height - _bfrPos.y;
                        Vector2 _bflPos = GUIUtility.ScreenToGUIPoint(bflPos);
                        _bflPos.y = Screen.height - _bflPos.y;

                        Vector2 _bbrPos = GUIUtility.ScreenToGUIPoint(bbrPos);
                        _bbrPos.y = Screen.height - _bbrPos.y;
                        Vector2 _bblPos = GUIUtility.ScreenToGUIPoint(bblPos);
                        _bblPos.y = Screen.height - _bblPos.y;

                        Vector2 _tfrPos = GUIUtility.ScreenToGUIPoint(tfrPos);
                        _tfrPos.y = Screen.height - _tfrPos.y;
                        Vector2 _tflPos = GUIUtility.ScreenToGUIPoint(tflPos);
                        _tflPos.y = Screen.height - _tflPos.y;

                        Vector2 _tbrPos = GUIUtility.ScreenToGUIPoint(tbrPos);
                        _tbrPos.y = Screen.height - _tbrPos.y;
                        Vector2 _tblPos = GUIUtility.ScreenToGUIPoint(tblPos);
                        _tblPos.y = Screen.height - _tblPos.y;

                        if (m_ColorMode == ColorMode.Gradient)
                        {
                            Color[,] c = m_ColorModifierAnimatedGradient.Colors;
                            GLDraw.DrawLine(_bfrPos, _bflPos, c[0, 0], c[0, 1], 2f);
                            GLDraw.DrawLine(_bbrPos, _bblPos, c[0, 0], c[0, 1], 2f);

                            GLDraw.DrawLine(_bfrPos, _bbrPos, c[1, 0], c[1, 1], 2f);
                            GLDraw.DrawLine(_bflPos, _bblPos, c[1, 0], c[1, 1], 2f);

                            GLDraw.DrawLine(_tfrPos, _tflPos, c[0, 0], c[0, 1], 2f);
                            GLDraw.DrawLine(_tbrPos, _tblPos, c[0, 0], c[0, 1], 2f);

                            GLDraw.DrawLine(_tfrPos, _tbrPos, c[1, 0], c[1, 1], 2f);
                            GLDraw.DrawLine(_tflPos, _tblPos, c[1, 0], c[1, 1], 2f);

                            GLDraw.DrawLine(_bfrPos, _tfrPos, c[2, 0], c[2, 1], 2f);
                            GLDraw.DrawLine(_bbrPos, _tbrPos, c[2, 0], c[2, 1], 2f);

                            GLDraw.DrawLine(_bflPos, _tflPos, c[3, 0], c[3, 1], 2f);
                            GLDraw.DrawLine(_bblPos, _tblPos, c[3, 0], c[3, 1], 2f);
                            continue;
                        }

                        GLDraw.DrawLine(_bfrPos, _bflPos, color, 2f);
                        GLDraw.DrawLine(_bbrPos, _bblPos, color, 2f);

                        GLDraw.DrawLine(_bfrPos, _bbrPos, color, 2f);
                        GLDraw.DrawLine(_bflPos, _bblPos, color, 2f);

                        GLDraw.DrawLine(_tfrPos, _tflPos, color, 2f);
                        GLDraw.DrawLine(_tbrPos, _tblPos, color, 2f);

                        GLDraw.DrawLine(_tfrPos, _tbrPos, color, 2f);
                        GLDraw.DrawLine(_tflPos, _tblPos, color, 2f);

                        GLDraw.DrawLine(_bfrPos, _tfrPos, color, 2f);
                        GLDraw.DrawLine(_bbrPos, _tbrPos, color, 2f);

                        GLDraw.DrawLine(_bflPos, _tflPos, color, 2f);
                        GLDraw.DrawLine(_bblPos, _tblPos, color, 2f);
                    }
                }
            }
        }
    }
}
