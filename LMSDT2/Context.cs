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
    public delegate AgentHumanDT EntitySearchDelegate(Transform transform);

    public class Context
    {
        EntitySearchDelegate[] m_SearchDelegates;

        public List<AgentHumanDT> VisibleEntities { get; private set; }
        public List<AgentHumanDT> LivingEnitites { get; private set; }
        public AgentHumanDT LocalPlayer { get; private set; }
        public AgentHumanDT ClosestEntity { get; private set; }
        public AgentHumanDT ClosestVEntity { get; private set; }

        public Context()
        {
            VisibleEntities = new List<AgentHumanDT>();
            LivingEnitites = new List<AgentHumanDT>();
            m_SearchDelegates = new EntitySearchDelegate[]
            {
                (trans) =>
                {
                   return trans.GetComponent<AgentHumanDT>();
                },
                (trans) =>
                {
                    return trans.GetComponentInChildren<AgentHumanDT>();
                },
                (trans) =>
                {
                    return trans.GetComponentInParent<AgentHumanDT>();
                }
            };
        }

        public AgentHumanDT SearchForEntity(Transform transform)
        {
            AgentHumanDT target;
            int offset = 0;
            do
            {
                target = m_SearchDelegates[offset](transform);
                offset++;
            }
            while (target == null && offset < m_SearchDelegates.Length);
            return target;
        }

        public void Update(ComponentPlayer localPlayer)
        {
            VisibleEntities.Clear();
            LivingEnitites.Clear();
            //refresh context
            if (localPlayer != null)
            {
                LocalPlayer = localPlayer.Owner;
                float closestDist = Mathf.Infinity;
                float closestvDist = closestDist;
                foreach (AgentHumanDT agentHumanDT in Mission.Instance.CurrentGameZone.Agents)
                {
                    if (agentHumanDT.IsAlive && !agentHumanDT.IsFriend(LocalPlayer))
                    {
                        LivingEnitites.Add(agentHumanDT);
                        HitUtils.HitData data;
                        bool targetFound;
                        WeaponBase weapon = LocalPlayer.WeaponComponent.GetCurrentWeapon();
                        Vector3 startPos = LocalPlayer.Position;
                        startPos.y += 1.7f;
                        weapon.ComputeAimAssistDir(startPos, agentHumanDT.EyePosition - startPos, out targetFound, out data);
                        if (targetFound)
                        {
                            float vdist = Vector3.Distance(agentHumanDT.transform.position, localPlayer.transform.position);
                            if (closestvDist > vdist)
                            {
                                closestvDist = vdist;
                                ClosestVEntity = agentHumanDT;
                            }
                            VisibleEntities.Add(agentHumanDT);
                        }
                        /*RaycastHit hit;
                        if (Physics.SphereCast(localPlayer.Owner.EyePosition, 5f, agentHumanDT.ChestPosition - localPlayer.Owner.EyePosition, out hit, 100f, -5, QueryTriggerInteraction.UseGlobal))
                        {
                            AgentHumanDT target;
                            int offset = 0;
                            do
                            {
                                target = m_SearchDelegates[offset](hit.transform);
                                offset++;
                            }
                            while (target == null && offset < m_SearchDelegates.Length);
                            if (target != null)
                                VisibleEntities.Add(agentHumanDT);
                        }*/
                        float dist = Vector3.Distance(agentHumanDT.transform.position, localPlayer.transform.position);
                        if (closestDist > dist)
                        {
                            closestDist = dist;
                            ClosestEntity = agentHumanDT;
                        }
                    }
                }
            }
            Utils.NativeLMSLog(string.Format("Context update: v {0} l {1} p {2}", VisibleEntities.Count, LivingEnitites.Count, localPlayer != null));
        }

        public static void SampleL()
        {
        }
    }
}
