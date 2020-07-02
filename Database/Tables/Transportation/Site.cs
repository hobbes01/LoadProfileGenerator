﻿#region header

// // ProfileGenerator DatabaseIO changed: 2017 03 22 14:10

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Automation;
using Automation.ResultFiles;
using Common;
using Database.Database;
using Database.Tables.BasicElements;
using Database.Tables.BasicHouseholds;
using JetBrains.Annotations;

namespace Database.Tables.Transportation {
    public class Site : DBBaseElement {
        public const string TableName = "tblSites";
        [NotNull] [ItemNotNull] private readonly ObservableCollection<SiteLocation> _siteLocations = new ObservableCollection<SiteLocation>();
        [CanBeNull] private string _description;

        public Site([NotNull] string name, [CanBeNull] int? pID, [NotNull] string connectionString,
            [CanBeNull] string description, StrGuid guid) : base(name, TableName,
            connectionString, guid)
        {
            _description = description;
            ID = pID;
            AreNumbersOkInNameForIntegrityCheck = true;
            TypeDescription = "Site";
        }

        [CanBeNull]
        [UsedImplicitly]
        public string Description {
            get => _description;
            set => SetValueWithNotify(value, ref _description, nameof(Description));
        }

        [ItemNotNull]
        [NotNull]
        [UsedImplicitly]
        public ObservableCollection<SiteLocation> Locations => _siteLocations;
        public void AddLocation([NotNull] Location location, bool savetoDB = true)
        {
            if (location == null) {
                throw new LPGException("Can't add a null location.");
            }

            if (location.ConnectionString != ConnectionString) {
                throw new LPGException("A location from another DB was just added!");
            }

            if (_siteLocations.Any(x => x.Location == location)) {
                Logger.Error("This location was already added.");
                return;
            }

            var siteloc = new SiteLocation(null, location, IntID, ConnectionString, location.Name, System.Guid.NewGuid().ToStrGuid());
            _siteLocations.Add(siteloc);
            if (savetoDB) {
                siteloc.SaveToDB();
            }

            _siteLocations.Sort();
        }

        [NotNull]
        [UsedImplicitly]
        public static DBBase CreateNewItem([NotNull] Func<string, bool> isNameTaken, [NotNull] string connectionString) => new Site(
            FindNewName(isNameTaken, "New Site "), null, connectionString, "",System.Guid.NewGuid().ToStrGuid());

        public void DeleteLocation([NotNull] SiteLocation ld)
        {
            ld.DeleteFromDB();
            _siteLocations.Remove(ld);
        }

        public override void DeleteFromDB()
        {
            foreach (SiteLocation location in _siteLocations)
            {
                location.DeleteFromDB();
            }
            base.DeleteFromDB();
        }

        [NotNull]
        [UsedImplicitly]
        public static Site ImportFromItem([NotNull] Site toImport, [NotNull] Simulator dstSim)
        {
            var site = new Site(toImport.Name, null, dstSim.ConnectionString, toImport.Description, toImport.Guid);
            site.SaveToDB();
            foreach (var siteloc in toImport._siteLocations) {
                var loc = GetItemFromListByName(dstSim.Locations.Items, siteloc.Location.Name);
                if (loc == null) {
                    Logger.Error("While importing, could not find a location. Skipping.");
                    continue;
                }
                site.AddLocation(loc);
            }
            return site;
        }

        public static void LoadFromDatabase([ItemNotNull] [NotNull] ObservableCollection<Site> result, [NotNull] string connectionString,
            bool ignoreMissingTables, [ItemNotNull] [NotNull] ObservableCollection<Location> locations)
        {
            var aic = new AllItemCollections();
            LoadAllFromDatabase(result, connectionString, TableName, AssignFields, aic, ignoreMissingTables, true);
            var ld = new ObservableCollection<SiteLocation>();
            SiteLocation.LoadFromDatabase(ld, connectionString, locations, ignoreMissingTables);
            SetSubitems(new List<DBBase>(result), new List<DBBase>(ld), IsCorrectSiteLocationParent,
                ignoreMissingTables);
        }

        public override void SaveToDB()
        {
            base.SaveToDB();
            foreach (var siteLocation in _siteLocations) {
                siteLocation.SaveToDB();
            }
        }

        protected override bool IsItemLoadedCorrectly(out string message)
        {
            message = "";
            return true;
        }

        protected override void SetSqlParameters(Command cmd)
        {
            cmd.AddParameter("Name", Name);
        }

        [NotNull]
        private static Site AssignFields([NotNull] DataReader dr, [NotNull] string connectionString, bool ignoreMissingFields,
            [NotNull] AllItemCollections aic)
        {
            var name = dr.GetString("Name", false, "(no name)", ignoreMissingFields);
            var description = dr.GetString("Description", false, "(no description)", ignoreMissingFields);
            var id = dr.GetIntFromLong("ID", false, ignoreMissingFields, -1);
            var guid = GetGuid(dr, ignoreMissingFields);
            return new Site(name, id, connectionString, description, guid);
        }

        private static bool IsCorrectSiteLocationParent([NotNull] DBBase parent, [NotNull] DBBase child)
        {
            var hd = (SiteLocation) child;

            if (parent.ID == hd.SiteID) {
                var site = (Site) parent;
                site._siteLocations.Add(hd);
                return true;
            }

            return false;
        }

        public override DBBase ImportFromGenericItem(DBBase toImport, Simulator dstSim)
            => ImportFromItem((Site)toImport, dstSim);

        public override List<UsedIn> CalculateUsedIns(Simulator sim)
        {
            List<UsedIn> uis = new List<UsedIn>();
            var routes = sim.TravelRoutes.Items.Where(x => x.SiteA == this || x.SiteB == this).ToList();
            foreach (var route in routes) {
                uis.Add(new UsedIn(route,"Route"));
            }

            var routesets = sim.TravelRouteSets.Items.Where(x => x.TravelRoutes.Any(y => routes.Contains(y.TravelRoute)));
            foreach (var routeset in routesets)
            {
                uis.Add(new UsedIn(routeset, "Route Set"));
            }

            foreach (var chargingStationSet in sim.ChargingStationSets.Items) {
                if (chargingStationSet.ChargingStations.Any(x => x.Site == this)) {
                    uis.Add(new UsedIn(chargingStationSet,"Charging Station Set"));
                }
            }

            return uis;
        }

        [NotNull]
        public Site MakeCopy([NotNull] Simulator sim)
        {
            var newSite = sim.Sites.CreateNewItem(sim.ConnectionString);
            newSite.Name = Name + "(copy)";
            newSite.Description = Description;
            newSite.SaveToDB();
            foreach (SiteLocation location in Locations) {
                newSite.AddLocation(location.Location);
            }
            return newSite;
        }
    }
}