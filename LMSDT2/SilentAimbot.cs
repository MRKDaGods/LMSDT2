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
using System.Reflection;

namespace LMSDT2
{
    public class SilentAimbot : Hack
    {
        bool m_ThroughWalls;
        int m_Bone;
        float m_MaxRange;
        FieldInfo m_ActiveProjectilesRef;

        public override int C => 0;
        public override int H => 1;

        public SilentAimbot()
        {
            m_MaxRange = 5f;
        }

        List<Projectile> GetProjectiles()
        {
            if (m_ActiveProjectilesRef == null)
                m_ActiveProjectilesRef = typeof(ProjectileManager).GetField("m_Active", BindingFlags.Instance | BindingFlags.NonPublic);
            return (List<Projectile>)m_ActiveProjectilesRef.GetValue(ProjectileManager.Instance);
        }

        public override void ReceiveMessageB(int o, bool b)
        {
            base.ReceiveMessageB(o, b);
            if (o == 3)
                m_ThroughWalls = b;
        }

        public override void ReceiveMessageI(int o, int v)
        {
            base.ReceiveMessageI(o, v);
            if (o == 2)
                m_Bone = v;
        }

        public override void ReceiveMessageF(int o, float v)
        {
            base.ReceiveMessageF(o, v);
            if (o == 1)
                m_MaxRange = v;
        }

        public override unsafe void ReceiveMessagePFPF(int o, float* pF1, float* pF2)
        {
            if (o == 1)
            {
                *pF1 = 5f;
                *pF2 = 100f;
            }
        }

        Vector3 GetTargetPosition(AgentHumanDT agentHumanDT)
        {
            switch (m_Bone)
            {
                case 0:
                    return agentHumanDT.ChestPosition;
                case 1:
                    return agentHumanDT.Head.position;
                case 2:
                    return agentHumanDT.ChestPosition + 2f * agentHumanDT.transform.right + -agentHumanDT.transform.up;
                case 3:
                    return agentHumanDT.ChestPosition + 2f * agentHumanDT.transform.right + 3f * -agentHumanDT.transform.up;
            }
            return Vector3.zero;
        }

        public override void OnUpdate(Context context)
        {
            if (context.LivingEnitites.Count == 0 || context.LocalPlayer == null || context.ClosestEntity == null)
                return;
            List<Projectile> projectiles = GetProjectiles();
            //Utils.NativeLMSLog("Projectile Count: " + projectiles.Count);
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Settings.Agent != context.LocalPlayer)
                    continue;
                //projectile.GetComponent<Collider>().enabled = false;
                AgentHumanDT target = m_ThroughWalls ? context.ClosestEntity : context.ClosestVEntity;
                if (Vector3.Distance(target.Position, context.LocalPlayer.Position) <= m_MaxRange)
                    projectile.transform.LookAt(GetTargetPosition(target));
            }
        }
    }
}
