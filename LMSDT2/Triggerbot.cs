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
    public class Triggerbot : Hack
    {
        float m_CrosshairRadius;

        public override int C => 0;
        public override int H => 2;

        public Triggerbot()
        {
            m_CrosshairRadius = 20f;
        }

        public override void OnUpdate(Context context)
        {
            if (context.LivingEnitites.Count == 0 || context.LocalPlayer == null)
                return;
            WeaponBase weapon = context.LocalPlayer.WeaponComponent.GetCurrentWeapon();
            bool targetFound;
            HitUtils.HitData data;
            weapon.ComputeAimAssistDir(weapon.AttackPos, weapon.AttackDir, out targetFound, out data);
            if (targetFound)
                context.LocalPlayer.BlackBoard.ActionAdd(AgentActionAttack.CreateAction(weapon.AttackDir));
            /*
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;
            if (Physics.SphereCast(camTransform.position, 5f, camTransform.forward, out hit, 100f, -5))
            {
                if (context.SearchForEntity(hit.transform) != null)
                    context.LocalPlayer.BlackBoard.ActionAdd(AgentActionAttack.CreateAction(camTransform.forward));
            }
            */
        }
    }
}
