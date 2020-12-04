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
using LMSReceiverServiceProvider;

namespace LMSDT2
{
    public class HackManager : MonoBehaviour
    {
        static GameObject ms_ManagedHackManager;
        public static HackManager Instance { get; private set; }

        Hack[] m_Hacks;
        Context m_Context;

        void Awake()
        {
            NativePipe.RegisterCallback(NativePipeMessageType.F, (c, h, o, v) =>
            {
                Utils.NativeLMSLog(string.Format("ReceivedMessage, type F({0},{1},{2},{3})", c, h, o, v));
                GetHack(c, h).ReceiveMessageF(o, (float)v[0]);
            });
            NativePipe.RegisterCallback(NativePipeMessageType.I, (c, h, o, v) =>
            {
                Utils.NativeLMSLog(string.Format("ReceivedMessage, type I({0},{1},{2},{3})", c, h, o, v));
                GetHack(c, h).ReceiveMessageI(o, (int)v[0]);
            });
            NativePipe.RegisterCallback(NativePipeMessageType.B, (c, h, o, v) =>
            {
                Utils.NativeLMSLog(string.Format("ReceivedMessage, type B({0},{1},{2},{3})", c, h, o, v));
                GetHack(c, h).ReceiveMessageB(o, (bool)v[0]);
            });
            unsafe
            {
                NativePipe.RegisterCallback(NativePipeMessageType.PFPF, (c, h, o, v) =>
                {
                    PFPF pfpf = (PFPF)v[0];
                    GetHack(c, h).ReceiveMessagePFPF(o, pfpf.PF1, pfpf.PF2);
                });
                NativePipe.RegisterCallback(NativePipeMessageType.PB, (c, h, o, v) =>
                {

                });
            }
        }

        void Start()
        {
            m_Context = new Context();
            m_Hacks = new Hack[]
            {
                new Aimbot(),
                new SilentAimbot(),
                new Triggerbot(),
                new Boxes(),
                new Tracers(),
                new Distance(),
                new Health(),
                new Fly()
            };
            Utils.NativeLMSLog("Hack Manager init");
        }

        void Update()
        {
            m_Context.Update(Player.Instance);
            foreach (Hack hack in m_Hacks)
            {
                if (hack.Enabled)
                {
                    try
                    {
                        hack.OnUpdate(m_Context);
                    }
                    catch (Exception ex)
                    {
                        Utils.NativeLMSLog("Exception: " + ex.ToString());
                    }
                }
            }
        }

        public Hack GetHack(int c, int h)
        {
            return m_Hacks.Where(x => x.C == c && x.H == h).ToArray()[0];
        }

        void OnGUI()
        {
            foreach(Hack hack in m_Hacks)
            {
                if (hack.Enabled)
                {
                    try
                    {
                        hack.OnGUI(m_Context);
                    }
                    catch (Exception ex)
                    {
                        Utils.NativeLMSLog("Exception: " + ex.ToString());
                    }
                }
            }
        }

        //--------IL CODE EMULATION---------

        class lovemehasy
        {
            protected virtual void OnInitialize()
            {
            }

            public bool vis;

            protected lovemehasy GetChild<T>(string h, bool k) where T : lovemehasy
            {
                return this;
            }
        }

        class lovemehasweel : lovemehasy
        {
            protected override void OnInitialize()
            {
                Context.SampleL();
                NativePipe.SampleL();
                base.OnInitialize();
                RegisterPane(new lovemehasy());
                RegisterPane(new lovemehasy());
                RegisterPane(new lovemehasy());
                if (x.isHer())
                {
                    GetChild<lovemehasy>("rhfloel", true).vis = true;
                }
                else
                {
                    RegisterPane(new lovemehasy());
                }
            }

            class x
            {
                public static bool isHer()
                {
                    return true;
                }
            }

            void RegisterPane(object o)
            {
            }
        }

        internal void CompilerCopy()
        {
            Context.SampleL();
            NativePipe.SampleL();
            
        }

        static void NativeInit()
        {
            ms_ManagedHackManager = new GameObject("ManagedHackManager");
            Instance = ms_ManagedHackManager.AddComponent<HackManager>();
            DontDestroyOnLoad(ms_ManagedHackManager);
        }
    }
}
