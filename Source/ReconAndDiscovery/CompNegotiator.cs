using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
    [StaticConstructorOnStartup]
    public class CompNegotiator : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            List<FloatMenuOption> list = base.CompFloatMenuOptions(selPawn).ToList<FloatMenuOption>();
            FloatMenuOption item = new FloatMenuOption("RD_Negotiate".Translate(), delegate ()
            {
                Job job = new Job(JobDefOfReconAndDiscovery.RD_Negotiate);
                job.targetA = this.parent;
                job.playerForced = true;
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.Default, null, null, 0f, null, null);
            list.Add(item);
            return list;
        }

        private static void RenderExclamationPointOverlay(Thing t)
        {
            if (!t.Spawned)
            {
                return;
            }
            Vector3 drawPos = t.DrawPos;
            drawPos.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
            if (t is Pawn)
            {
                drawPos.x += (float)t.def.size.x - 1f;
                drawPos.z += (float)t.def.size.z + 0.2f;
            }
            RenderPulsingOverlayQuest(t, QuestionMarkMat, drawPos, MeshPool.plane05);
        }

        private static void RenderPulsingOverlayQuest(Thing thing, Material mat, Vector3 drawPos, Mesh mesh)
        {
            float num = ((float)Math.Sin((double)((Time.realtimeSinceStartup + 397f * (float)(thing.thingIDNumber % 571)) * 4f)) + 1f) * 0.5f;
            num = 0.3f + num * 0.7f;
            Material material = FadedMaterialPool.FadedVersionOf(mat, num);
            Graphics.DrawMesh(mesh, drawPos, Quaternion.identity, material, 0);
        }

        private static readonly Material QuestionMarkMat = MaterialPool.MatFrom("UI/Overlays/QuestionMark", ShaderDatabase.MetaOverlay);

        public override void CompTick()
        {
            RenderExclamationPointOverlay(this.parent);
        }
    }
}

