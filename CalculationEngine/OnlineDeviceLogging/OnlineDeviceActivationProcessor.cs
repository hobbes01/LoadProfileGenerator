﻿//-----------------------------------------------------------------------

// <copyright>
//
// Copyright (c) TU Chemnitz, Prof. Technische Thermodynamik
// Written by Noah Pflugradt.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the distribution.
//  All advertising materials mentioning features or use of this software must display the following acknowledgement:
//  “This product includes software developed by the TU Chemnitz, Prof. Technische Thermodynamik and its contributors.”
//  Neither the name of the University nor the names of its contributors may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE UNIVERSITY 'AS IS' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE UNIVERSITY OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, S
// PECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; L
// OSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// </copyright>

//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automation;
using Automation.ResultFiles;
using CalculationEngine.OnlineLogging;
using Common;
using Common.CalcDto;
using Common.JSON;
using JetBrains.Annotations;

namespace CalculationEngine.OnlineDeviceLogging {
    using Common.SQLResultLogging.Loggers;

    public class SetToZeroEntry {
        public SetToZeroEntry([NotNull] TimeStep startTime, [NotNull] TimeStep endTime, OefcKey key) {
            StartTime = startTime;
            EndTime = endTime;
            Key = key;
        }

        [NotNull]
        public TimeStep EndTime { get; }

        public OefcKey Key { get; }
        [NotNull]
        public TimeStep StartTime { get; }
    }

    public interface IOnlineDeviceActivationProcessor
    {
        OefcKey RegisterDevice([NotNull] CalcLoadTypeDto loadType, [NotNull] CalcDeviceDto device);

        void AddNewStateMachine( [NotNull] TimeStep startTimeStep,
                                [NotNull] CalcLoadTypeDto loadType, [NotNull] string affordanceName, [NotNull] string activatorName,
             OefcKey oefckey, CalcDeviceDto calcDeviceDto, StepValues sv);

        void AddZeroEntryForAutoDev(OefcKey zeKey,[NotNull] TimeStep starttime, int totalDuration);

        [NotNull]
        OnlineEnergyFileColumns Oefc { get; }

        [NotNull]
        [ItemNotNull]
        List<OnlineEnergyFileRow> ProcessOneTimestep([NotNull] TimeStep timeStep);

        [NotNull]
        Dictionary<CalcLoadTypeDto, BinaryWriter> BinaryOutStreams { get; }
        [NotNull]
        Dictionary<CalcLoadTypeDto, BinaryWriter> SumBinaryOutStreams { get; }
        [NotNull]
        Dictionary<ProfileActivationEntry.ProfileActivationEntryKey, ProfileActivationEntry> ProfileEntries { get; }
    }
    public class OnlineDeviceActivationProcessor : IOnlineDeviceActivationProcessor {
        private readonly IOnlineLoggingData _old;
        [NotNull] private readonly CalcParameters _calcParameters;

        [NotNull]
        private readonly Dictionary<CalcLoadTypeDto, BinaryWriter> _binaryOutStreams;
        [NotNull]
        private readonly FileFactoryAndTracker _fft;
        [NotNull]
        private readonly Dictionary<CalcLoadTypeDto, int> _loadTypeDict;
        [NotNull]
        private readonly Dictionary<ProfileActivationEntry.ProfileActivationEntryKey, ProfileActivationEntry>
            _profileEntries =
                new Dictionary<ProfileActivationEntry.ProfileActivationEntryKey, ProfileActivationEntry>();
        [NotNull]
        private readonly Dictionary<CalcLoadTypeDto, BinaryWriter> _sumBinaryOutStreams;

        [ItemNotNull]
        [NotNull]
        private readonly List<SetToZeroEntry> _zeroEntries = new List<SetToZeroEntry>();

        private readonly HashSet<StrGuid> _savedDevices = new HashSet<StrGuid>();
        //[ItemNotNull]
        //[JetBrains.Annotations.NotNull]
        //private List<OnlineDeviceStateMachine> _statemachines = new List<OnlineDeviceStateMachine>();

        public OnlineDeviceActivationProcessor(IOnlineLoggingData old, [NotNull] CalcParameters calcParameters, [NotNull] FileFactoryAndTracker fft)
        {
            _old = old;
            _calcParameters = calcParameters;
            Oefc = new OnlineEnergyFileColumns(old);
            _loadTypeDict = new Dictionary<CalcLoadTypeDto, int>();
            _fft = fft;
            _binaryOutStreams = new Dictionary<CalcLoadTypeDto, BinaryWriter>();
            _sumBinaryOutStreams = new Dictionary<CalcLoadTypeDto, BinaryWriter>();
            Logger.Info("Initializing the online device activation processor...");
        }

        public Dictionary<CalcLoadTypeDto, BinaryWriter> BinaryOutStreams => _binaryOutStreams;

        public OnlineEnergyFileColumns Oefc { get; }

        public Dictionary<ProfileActivationEntry.ProfileActivationEntryKey, ProfileActivationEntry> ProfileEntries
            => _profileEntries;

        [NotNull]
        public Dictionary<CalcLoadTypeDto, List<OnlineDeviceStateMachine>> StateMachinesByLoadtype => _stateMachinesByLoadtype;

        [NotNull] private readonly Dictionary<CalcLoadTypeDto, List<OnlineDeviceStateMachine>> _stateMachinesByLoadtype = new Dictionary<CalcLoadTypeDto, List<OnlineDeviceStateMachine>>();
        public Dictionary<CalcLoadTypeDto, BinaryWriter> SumBinaryOutStreams => _sumBinaryOutStreams;

        public void AddNewStateMachine( TimeStep startTimeStep,
                                       CalcLoadTypeDto loadType, string affordanceName, string activatorName,
                                             OefcKey oefckey, [NotNull] CalcDeviceDto calcDeviceDto, [NotNull] StepValues sv)
        {
            Oefc.IsDeviceRegistered(loadType, oefckey);
            //OefcKey oefckey = new OefcKey(householdKey, deviceType, deviceID, locationID, loadType.ID);
            // this is for logging the used time profiles which gets dumped to the time profile log
            ProfileActivationEntry.ProfileActivationEntryKey key =
                new ProfileActivationEntry.ProfileActivationEntryKey(calcDeviceDto.Name, sv.Name, sv.DataSource,
                    loadType.Name);
            if (!_profileEntries.ContainsKey(key))
            {
                ProfileActivationEntry entry = new ProfileActivationEntry(calcDeviceDto.Name, sv.Name, sv.DataSource,
                    loadType.Name,_calcParameters);
                _profileEntries.Add(entry.GenerateKey(), entry);
            }
            _profileEntries[key].ActivationCount++;
            // do the device activiation
            var columnNumber = Oefc.GetColumnNumber(loadType, oefckey);
            var dsm = new OnlineDeviceStateMachine( startTimeStep,
                 loadType, calcDeviceDto.Name, oefckey, affordanceName, _calcParameters, sv, columnNumber);
            //_statemachines.Add(dsm);
            _stateMachinesByLoadtype[loadType].Add(dsm);
            // log the affordance energy use.
            if (_calcParameters.IsSet(CalcOption.DeviceActivations)) {
                double totalPowerSum = dsm.CalculateOfficialEnergyUse();
                double totalEnergysum = loadType.ConversionFactor * totalPowerSum;
                var entry = new DeviceActivationEntry( dsm.AffordanceName,
                    dsm.LoadType,totalEnergysum , activatorName,sv.Values.Count,
                    startTimeStep, calcDeviceDto); // dsm.StepValues.ToArray(),
                if (!_savedDevices.Contains(calcDeviceDto.DeviceInstanceGuid)) {
                    _old.RegisterDeviceArchiveDto(new CalcDeviceArchiveDto( calcDeviceDto));
                    _savedDevices.Add(calcDeviceDto.DeviceInstanceGuid);
                }
                _old.RegisterDeviceActivation(entry);
            }
        }

        public void AddZeroEntryForAutoDev(OefcKey zeKey, TimeStep starttime, int totalDuration) {
            var stze = new SetToZeroEntry(starttime, starttime.AddSteps( totalDuration), zeKey);
            _zeroEntries.Add(stze);
        }

        private static void CleanExpiredStateMachines([NotNull] TimeStep timestep,[NotNull] Dictionary<CalcLoadTypeDto, List<OnlineDeviceStateMachine>> statemachines) {
            // alte state machines entsorgen
            foreach (KeyValuePair<CalcLoadTypeDto, List<OnlineDeviceStateMachine>> pair in statemachines) {
                var todelete = pair.Value.Where(x => x.IsExpired(timestep)).ToList();
                foreach (var machine in todelete) {
                    pair.Value.Remove(machine);
                }
            }
        }

        private void CleanZeroValueEntries([NotNull] TimeStep currentTime)
        {
            TimeStep nextStep = currentTime.AddSteps(1);
            var items2Delete = _zeroEntries.Where(x => x.EndTime < nextStep).ToList();
            foreach (var entry in items2Delete) {
                _zeroEntries.Remove(entry);
            }
        }

        public List<OnlineEnergyFileRow> ProcessOneTimestep(TimeStep timeStep) {
            CleanExpiredStateMachines(timeStep, _stateMachinesByLoadtype);
            var fileRows = new List<OnlineEnergyFileRow>();
            //var procesedMachines = new Dictionary<CalcLoadTypeDto, List< OnlineDeviceStateMachine>>();
            //foreach (OnlineDeviceStateMachine stateMachine in _statemachines) {
            //    if (!_loadTypeDict.ContainsKey(stateMachine.LoadType)) {
            //        throw new LPGException("Found a state machine for a load type that does not exist: " + stateMachine.OefcKey + ": " + stateMachine.LoadType);
            //    }
            //}
            //foreach (var loadType in _loadTypeDict.Keys) {
            //    var columnEntriesDeviceKey = Oefc.ColumnEntriesByLoadTypeByDeviceKey[loadType];
            //    foreach (var machine in _statemachines) {
            //        if (columnEntriesDeviceKey.ContainsKey(machine.OefcKey)) {
            //            procesedMachines[loadType].Add( machine);
            //        }
            //    }
            //}

            foreach (var pair in _stateMachinesByLoadtype) {
                //if (procesedMachines[pair.Key].Count != pair.Value) {

                //}
                var energyvalues = new List<double>(new double[Oefc.ColumnCountByLoadType[pair.Key]]);
                foreach (var machine in pair.Value) {
                    energyvalues[machine.ColumnNumber] +=
                        machine.GetEnergyValueForTimeStep(timeStep,  _zeroEntries);
                }
                var fileRow = new OnlineEnergyFileRow(timeStep, energyvalues, pair.Key);
                fileRows.Add(fileRow);
            }
            /*if (Config.ExtraUnitTestChecking) {
                if (procesedMachines.Count != _statemachines.Count) {
                    var nonprocessed =
                        _statemachines.Where(x => !procesedMachines.Contains(x)).ToList();
                    throw new LPGException("Not all machines were processed! Processed:" + procesedMachines.Count +
                                           " Total:" + _statemachines.Count + Environment.NewLine + nonprocessed);
                }
            }*/
            CleanZeroValueEntries(timeStep);
            return fileRows;
        }

        public OefcKey RegisterDevice(CalcLoadTypeDto loadType, CalcDeviceDto devicedto)
        {
            var key= new OefcKey(devicedto, loadType.Guid);
            if(key.LoadtypeGuid != loadType.Guid && key.LoadtypeGuid != "-1".ToStrGuid()) {
                throw new LPGException("bug: loadtype id was wrong while registering a device");
            }

            Oefc.AddColumnEntry(devicedto.Name, key, devicedto.LocationName,
                loadType, key.DeviceInstanceGuid, key.HouseholdKey, key.DeviceCategory,devicedto);

            if (!_loadTypeDict.ContainsKey(loadType)) {
                _loadTypeDict.Add(loadType, 1);
                _stateMachinesByLoadtype.Add(loadType,new List<OnlineDeviceStateMachine>());
                if (_calcParameters.IsSet(CalcOption.DetailedDatFiles)) {
                    var s = _fft.MakeFile<BinaryWriter>("OnlineDeviceEnergyUsage." + loadType.Name + ".dat",
                        "Binary Device energy usage per device for " + loadType.Name, false,
                        ResultFileID.OnlineDeviceActivationFiles, Constants.GeneralHouseholdKey, TargetDirectory.Temporary,
                        _calcParameters.InternalStepsize,CalcOption.DetailedDatFiles, loadType.ConvertToLoadTypeInformation());
                    _binaryOutStreams.Add(loadType, s);
                }
                if (_calcParameters.IsSet(CalcOption.OverallDats)) {
                    var binaryWriter =
                        _fft.MakeFile<BinaryWriter>("OnlineDeviceEnergyUsage.Sums." + loadType.Name + ".dat",
                            "Binary Device summed energy usage per device for " + loadType.Name, false,
                            ResultFileID.OnlineSumActivationFiles, Constants.GeneralHouseholdKey, TargetDirectory.Temporary, _calcParameters.InternalStepsize,
                            CalcOption.OverallDats,
                            loadType.ConvertToLoadTypeInformation());
                    _sumBinaryOutStreams.Add(loadType, binaryWriter);
                }
            }
            return key;
        }
    }
}