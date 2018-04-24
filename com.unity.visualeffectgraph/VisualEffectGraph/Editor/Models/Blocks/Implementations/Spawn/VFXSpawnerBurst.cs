using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.VFX;

namespace UnityEditor.VFX
{
    class VFXSpawnerBurstVariantCollection : IVariantProvider
    {
        public Dictionary<string, object[]> variants
        {
            get
            {
                return new Dictionary<string, object[]>
                {
                    { "repeat", new object[] { VFXSpawnerBurst.RepeatMode.Single, VFXSpawnerBurst.RepeatMode.Periodic } }
                };
            }
        }
    }

    [VFXInfo(category = "Spawn", variantProvider = typeof(VFXSpawnerBurstVariantCollection))]
    class VFXSpawnerBurst : VFXAbstractSpawner
    {
        public enum RepeatMode
        {
            Single,
            Periodic
        }

        public enum RandomMode
        {
            Constant,
            Random,
        }

        public RepeatMode Repeat { get { return repeat; } set { repeat = value; Invalidate(InvalidationCause.kSettingChanged); } }
        public RandomMode SpawnMode { get { return spawnMode; } set { spawnMode = value; Invalidate(InvalidationCause.kSettingChanged); } }
        public RandomMode DelayMode { get { return delayMode; } set { delayMode = value; Invalidate(InvalidationCause.kSettingChanged); } }


        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), SerializeField]
        private RepeatMode repeat = RepeatMode.Single;

        [VFXSetting, SerializeField]
        private RandomMode spawnMode =  RandomMode.Constant;

        [VFXSetting, SerializeField]
        private RandomMode delayMode = RandomMode.Constant;

        public override string name { get { return repeat.ToString() + " Burst"; } }
        public override VFXTaskType spawnerType { get { return repeat == RepeatMode.Periodic ? VFXTaskType.PeriodicBurstSpawner : VFXTaskType.BurstSpawner; } }

        public class AdvancedInputProperties
        {
            public Vector2 Count = new Vector2(0, 10);
            public Vector2 Delay = new Vector2(0, 1);
        }

        public class SimpleInputProperties
        {
            public float Count = 0.0f;
            public float Delay = 0.0f;
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get
            {
                var simple = PropertiesFromType("SimpleInputProperties");
                var advanced = PropertiesFromType("AdvancedInputProperties");

                if (spawnMode == RandomMode.Constant)
                    yield return simple.FirstOrDefault(o => o.property.name == "Count");
                else
                    yield return advanced.FirstOrDefault(o => o.property.name == "Count");

                if (delayMode == RandomMode.Constant)
                    yield return simple.FirstOrDefault(o => o.property.name == "Delay");
                else
                    yield return advanced.FirstOrDefault(o => o.property.name == "Delay");
            }
        }

        public override IEnumerable<VFXNamedExpression> parameters
        {
            get
            {
                // Get InputProperties
                var namedExpressions = GetExpressionsFromSlots(this);

                // Map Expressions based on Task Type (TODO: Fix names on C++ side)
                string countName = Repeat == RepeatMode.Periodic ? "nb" : "Count";
                string delayName = Repeat == RepeatMode.Periodic ? "period" : "Delay";

                // Process Counts
                var countExp = namedExpressions.First(e => e.name == "Count").exp;

                if (spawnMode == RandomMode.Random)
                    yield return new VFXNamedExpression(countExp, countName);
                else
                    yield return new VFXNamedExpression(new VFXExpressionCombine(countExp, countExp), countName);

                // Process Delay
                var delayExp = namedExpressions.First(e => e.name == "Delay").exp;

                if (delayMode == RandomMode.Random)
                    yield return new VFXNamedExpression(delayExp, delayName);
                else
                    yield return new VFXNamedExpression(new VFXExpressionCombine(delayExp, delayExp), delayName);
            }
        }
    }
}
