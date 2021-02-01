# kOS-TestFlight
An interface to allow kOS to read Test Flight data

This mod requires a recent version of TestFlight and kOS to be installed.

Access from kOS via ADDONS:TF.

### Supported suffixes
- MTBF - Scalar - Gets the MTBF for the passed in engine.
- FAILRATE - Scalar - Gets the failure rate (chance the engine will fail in the next second) for the passed in engine.
- RELIABILITY - Scalar - Gets the reliability (chance the engine will not fail before the given time) for the passed in engine and time.
- RUNTIME - Scalar - Gets the current run time for the passed in engine.
- RATEDBURNTIME - Scalar - Gets the rated run time for the passed in engine.
- IGNITIONCHANCE - Scalar - Gets the ignition success chance for the passed in engine.
- FAILED - Boolean - Indicates whether the passed in engine has failed or not.

### Example usage
```print "MTBF: " + Addons:TF:MTBF(eng) + " seconds".
```
