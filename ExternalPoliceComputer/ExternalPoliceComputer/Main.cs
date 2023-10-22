﻿using Rage;
using LSPD_First_Response.Mod.API;
using System.IO;
using System;
using System.Threading;

namespace ExternalPoliceComputer {
    private Timer timer;
    private int intervalInMilliseconds = 15000; // Change this to your desired interval
    public class Main : Plugin {

        public override void Initialize() {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            Game.LogTrivial("Plugin ExternalPoliceComputer has been initialized.");
        }

        public override void Finally() {
            Game.LogTrivial("ExternalPoliceComputer has been unloaded.");
        }

        private static void Functions_OnOnDutyStateChanged(bool OnDuty) {
            if (OnDuty) {
                try {
                    addEventsWithSTP();

                    updateWorldPeds();
                    updateWorldCars();

                    Game.DisplayNotification("ExternalPoliceComputer has been loaded.");
                } catch {
                    Events.OnCalloutDisplayed += Events_OnCalloutDisplayed;
                    Events.OnPulloverStarted += Events_OnPulloverStarted;
                    Events.OnPedPresentedId += Events_OnPedPresentedId;

                    updateWorldPeds();
                    updateWorldCars();

                    Game.DisplayNotification("ExternalPoliceComputer has been loaded (without StopThePed).");
                }
            }
        }

        public InfiniteLoopTimerExample()
        {
            // Create a Timer with a callback function
            timer = new Timer(TimerCallback, null, 0, intervalInMilliseconds);

            // You can stop the timer at any time by calling timer.Change
            // with dueTime and period set to Timeout.Infinite
            // For example, to stop the timer after 10 seconds:
            // timer.Change(10000, Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            // This method will be called repeatedly at the specified interval
            updateWorldPeds();
            updateWorldCars();
        }

        public static void Main(string[] args)
        {
            InfiniteLoopTimerExample timerExample = new InfiniteLoopTimerExample();

            // Keep the application running
            Console.WriteLine("Ped's and Vehicles Will Update every 15 Seconds");
            Console.ReadLine();
        }
    }

        private static void Events_OnPedPresentedId(Ped ped, LHandle pullover, LHandle pedInteraction) {
            updateWorldPeds();
            updateWorldCars();
        }

        private static void addEventsWithSTP() {
            Events.OnCalloutDisplayed += Events_OnCalloutDisplayed;
            Events.OnPulloverStarted += Events_OnPulloverStarted;
            StopThePed.API.Events.askIdEvent += Events_askIdEvent;
            StopThePed.API.Events.pedArrestedEvent += Events_pedArrestedEvent;
        }

        private static void Events_pedArrestedEvent(Ped ped) {
            updateWorldPeds();
            updateWorldCars();
        }

        private static void Events_askIdEvent(Ped ped) {
            updateWorldPeds();
            updateWorldCars();
        }

        private static void Events_OnPulloverStarted(LHandle handle) {
            updateWorldPeds();
            updateWorldCars();
        }

        private static void Events_OnCalloutDisplayed(LHandle handle) {
            updateWorldPeds();
            updateWorldCars();
        }

        private static void updateWorldPeds() {
            Game.LogTrivial("ExternalPoliceComputer: Update EPC/worldPeds.data");
            Ped[] allPeds = World.GetAllPeds();
            string[] persList = new string[allPeds.Length];

            foreach (Ped ped in allPeds) {
                if (ped.Exists()) {
                    persList[Array.IndexOf(allPeds, ped)] = $"name={Functions.GetPersonaForPed(ped).FullName}&birthday={Functions.GetPersonaForPed(ped).Birthday.Month}/{Functions.GetPersonaForPed(ped).Birthday.Day}/{Functions.GetPersonaForPed(ped).Birthday.Year}&gender={Functions.GetPersonaForPed(ped).Gender}&isWanted={Functions.GetPersonaForPed(ped).Wanted}&licenseStatus={Functions.GetPersonaForPed(ped).ELicenseState}";
                }

            }

            File.WriteAllText("EPC/worldPeds.data", string.Join(",", persList));

            Game.LogTrivial("ExternalPoliceComputer: Updated EPC/worldPeds.data");
        }

        
        public static string getRegistration(Vehicle car) {
            switch (StopThePed.API.Functions.getVehicleRegistrationStatus(car)) {
                case StopThePed.API.STPVehicleStatus.Expired:
                    return "Expired";
                case StopThePed.API.STPVehicleStatus.None:
                    return "None";
                case StopThePed.API.STPVehicleStatus.Valid:
                    return "Valid";
            }
            return "";
        }

        public static string getInsurance(Vehicle car) {
            switch (StopThePed.API.Functions.getVehicleInsuranceStatus(car)) {
                case StopThePed.API.STPVehicleStatus.Expired:
                    return "Expired";
                case StopThePed.API.STPVehicleStatus.None:
                    return "None";
                case StopThePed.API.STPVehicleStatus.Valid:
                    return "Valid";
            }
            return "";
        }

        private static void updateWorldCarsWithSTP() {
            Vehicle[] allCars = World.GetAllVehicles();
            string[] carsList = new string[allCars.Length];

            foreach (Vehicle car in allCars) {
                if (car.Exists()) {
                    string driver = car.Driver.Exists() ? Functions.GetPersonaForPed(car.Driver).FullName : "";
                    carsList[Array.IndexOf(allCars, car)] = $"licensePlate={car.LicensePlate}&model={car.Model.Name}&isStolen={car.IsStolen}&isPolice={car.IsPoliceVehicle}&owner={Functions.GetVehicleOwnerName(car)}&driver={driver}&registration={getRegistration(car)}&insurance={getInsurance(car)}";
                }
            }

            File.WriteAllText("EPC/worldCars.data", string.Join(",", carsList));
        }

        private static void updateWorldCars() {
            Game.LogTrivial("ExternalPoliceComputer: Update EPC/worldCars.data");

            try {
                updateWorldCarsWithSTP();
            } catch {
                Vehicle[] allCars = World.GetAllVehicles();
                string[] carsList = new string[allCars.Length];

                foreach (Vehicle car in allCars) {
                    if (car.Exists()) {
                        string driver = car.Driver.Exists() ? Functions.GetPersonaForPed(car.Driver).FullName : "";
                        carsList[Array.IndexOf(allCars, car)] = $"licensePlate={car.LicensePlate}&model={car.Model.Name}&isStolen={car.IsStolen}&isPolice={car.IsPoliceVehicle}&owner={Functions.GetVehicleOwnerName(car)}&driver={driver}";
                    }
                }

                File.WriteAllText("EPC/worldCars.data", string.Join(",", carsList));
            }
            
            Game.LogTrivial("ExternalPoliceComputer: Updated EPC/worldCars.data");
        }
    }
}