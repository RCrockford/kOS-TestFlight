using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;
using kOS.Safe.Exceptions;
using kOS.Suffixed;
using kOS.Suffixed.Part;
using UnityEngine;
using TestFlight;
using TestFlightCore;
using TestFlightAPI;
using System;

namespace kOS.AddOns.TestflightAddon
{
    [kOSAddon("TF")]
    [KOSNomenclature("TestflightAddon")]
    public class Addon : Suffixed.Addon
    {
        public Addon(SharedObjects shared) : base(shared)
        {
            AddSuffix("MTBF", new OneArgsSuffix<ScalarValue, PartValue>(GetPartMTBF));
            AddSuffix("FailRate", new OneArgsSuffix<ScalarValue, PartValue>(GetPartFailureRate));
            AddSuffix("Reliability", new TwoArgsSuffix<ScalarValue, PartValue, ScalarValue>(GetPartReliability));
            AddSuffix("RunTime", new OneArgsSuffix<ScalarValue, PartValue>(GetPartRunTime));
            AddSuffix("RatedBurnTime", new OneArgsSuffix<ScalarValue, PartValue>(GetPartRatedBurnTime));
            AddSuffix("IgnitionChance", new OneArgsSuffix<ScalarValue, EngineValue>(GetIgnitionChance));
            AddSuffix("Failed", new OneArgsSuffix<BooleanValue, PartValue>(GetPartFailed));
        }

        public override BooleanValue Available()
        {
            return TestFlightManagerScenario.Instance != null;
        }

        private TestFlightCore.TestFlightCore GetCoreModule(PartValue part)
        {
            foreach (var module in part.Part.Modules)
            {
                if (module is TestFlightCore.TestFlightCore)
                {
                    return module as TestFlightCore.TestFlightCore;
                }
            }
            return null;
        }

        private ScalarValue GetPartMTBF(PartValue part)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(part);
            if (coreMod != null)
            {
                double currentFailRate = coreMod.GetBaseFailureRate();
                return TestFlightAPI.TestFlightUtil.FailureRateToMTBF(currentFailRate, TestFlightUtil.MTBFUnits.SECONDS);
            }

            return -1f;
        }

        private ScalarValue GetPartFailureRate(PartValue part)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(part);
            if (coreMod != null)
            {
                double currentFailRate = coreMod.GetBaseFailureRate();
                return currentFailRate;
            }

            return -1f;
        }

        private ScalarValue GetPartReliability(PartValue part, ScalarValue time)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(part);
            if (coreMod != null)
            {
                double currentFailRate = coreMod.GetBaseFailureRate();
                return TestFlightAPI.TestFlightUtil.FailureRateToReliability(currentFailRate, time);
            }

            return -1f;
        }

        private ScalarValue GetPartRatedBurnTime(PartValue part)
        {
            foreach (var module in part.Part.Modules)
            {
                var relMod = module as TestFlightReliabilityBase;

                if (relMod != null)
                {
                    double ratedBurnTime = relMod.GetRatedBurnTime();
                    if (ratedBurnTime > 0)
                        return ratedBurnTime;
                }
            }

            return -1f;
        }

        private ScalarValue GetPartRunTime(PartValue part)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(part);
            if (coreMod != null)
            {
                return coreMod.GetOperatingTime();
            }

            return -1f;
        }

        private ScalarValue GetIgnitionChance(EngineValue engine)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(engine);
            if (coreMod != null)
            {
                foreach (var module in engine.Part.Modules)
                {
                    var relMod = module as TestFlightFailure_IgnitionFail;

                    if (relMod != null)
                    {
                        double ignitionChance = relMod.baseIgnitionChance.Evaluate(coreMod.GetInitialFlightData());
                        if (ignitionChance > 0)
                            return ignitionChance;
                    }
                }
            }

            return -1f;
        }

        private BooleanValue GetPartFailed(PartValue part)
        {
            TestFlightCore.TestFlightCore coreMod = GetCoreModule(part);
            if (coreMod != null)
            {
                Int32 partFailed = coreMod.GetPartStatus();
                return partFailed > 0;
            }

            return false;
        }
    }
}
