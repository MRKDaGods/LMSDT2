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

using UnityEngine;

namespace LMSDT2
{
    public enum ColorMode
    {
        Fixed = 0,
        Rainbow = 1,
        Gradient = 2,
        Visibility = 3
    }

    public class ColorModifierRainbow
    {
        Color m_Current;
        Color m_Target;
        float m_Delta;

        public Color Current => m_Current;

        public void Update()
        {
            m_Delta += Time.deltaTime * 3f; //3 secs
            if (m_Delta >= 1f)
            {
                m_Delta = 0f;
                m_Target = new Color(Random.value, Random.value, Random.value);
            }
            m_Current = Color.Lerp(m_Current, m_Target, m_Delta);
        }
    }

    public class ColorModifierAnimatedGradient
    {
        Color[,] m_Colors; //4->2
        Color[,] m_Targets; //4->2
        float[,] m_Deltas; //4->2
        uint m_Alloc;

        public Color[,] Colors => m_Colors;

        public ColorModifierAnimatedGradient(uint alloc = 4)
        {
            m_Alloc = alloc;
            m_Colors = new Color[alloc, alloc / 2];
            m_Targets = new Color[alloc, alloc / 2];
            m_Deltas = new float[alloc, alloc / 2];
        }

        public unsafe void Update()
        {
            for (int i = 0; i < m_Alloc; i++)
            {
                for (int j = 0; j < m_Alloc / 2; j++)
                {
                    fixed (float *currentDelta = &m_Deltas[i, j])
                    {
                        fixed (Color *target = &m_Targets[i, j])
                        {
                            fixed (Color *current = &m_Colors[i, j])
                            {
                                *currentDelta += Time.deltaTime * 3f;
                                if (*currentDelta >= 1f)
                                {
                                    *currentDelta = 0f;
                                    *target = new Color(Random.value, Random.value, Random.value);
                                }
                                *current = Color.Lerp(*current, *target, *currentDelta);
                            }
                        }
                    }
                }
            }
        }
    }

    public abstract class Hack
    {
        public abstract int C { get; }
        public abstract int H { get; }
        public bool Enabled { get; private set; }

        public virtual void OnUpdate(Context context)
        {
        }

        public virtual void OnGUI(Context context)
        {
        }

        public virtual void ReceiveMessageF(int o, float v)
        {
        }

        public virtual void ReceiveMessageI(int o, int v)
        {
        }

        public virtual void ReceiveMessageB(int o, bool b)
        {
            if (o == 0)
                Enabled = b;
        }

        public unsafe virtual void ReceiveMessagePFPF(int o, float *pF1, float *pF2)
        {
        }
    }
}
