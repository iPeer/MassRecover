using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MassRecover
{
    [KSPAddon(KSPAddon.Startup.TrackingStation, true)]
    public class Main : MonoBehaviour
    {

        ApplicationLauncherButton button;
        bool buttonAdded = false;

        void Start()
        {
            DontDestroyOnLoad(this);
        }

        void Awake()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(() => addToolbar());           
        }

        void addToolbar()
        {
            if (!buttonAdded)
            {
                buttonAdded = true;
                button = ApplicationLauncher.Instance.AddModApplication(this.recoverVessels, null, null, null, null, null, ApplicationLauncher.AppScenes.TRACKSTATION, getButtonTexture());

            }
        }

        /*void OnDestroy()
        {
            ApplicationLauncher.Instance.RemoveModApplication(button);
            Destroy(button);
        }*/

        public Texture2D getButtonTexture()
        {
            return GameDatabase.Instance.GetTexture("iPeer/MassRecover/Textures/Toolbar", false);
        }

        void recoverVessels()
        {
            //List<Vessel> vessels = FlightGlobals.fetch.vessels;
            //List<Vessel> suitable = vessels.FindAll(a => ((a.situation == Vessel.Situations.LANDED || a.situation == Vessel.Situations.SPLASHED) && a.orbitDriver.celestialBody.bodyName.ToLower().Equals("kerbin")));

            List<ProtoVessel> vessels = HighLogic.fetch.currentGame.flightState.protoVessels;
            List<ProtoVessel> suitable = vessels.FindAll(a => ((a.situation == Vessel.Situations.LANDED || a.situation == Vessel.Situations.SPLASHED) && a.orbitSnapShot.ReferenceBodyIndex == 1 && a.vesselType != VesselType.Flag));

            Debug.Log(String.Format("[MassRecover]: {0} vessels suitable for recovery:", suitable.Count));

            if (suitable.Count == 0)
            {
                PopupDialog.SpawnPopupDialog(new MultiOptionDialog("No vessels to recover!", "Nothing to recover", HighLogic.UISkin, new DialogGUIButton("OK", () => resetButton())), false, HighLogic.UISkin);
            }
            else
            {
                DialogGUIButton[] options = new DialogGUIButton[] {
                new DialogGUIButton("Yes", () => recoverVessels(suitable)),
                new DialogGUIButton("No", () => resetButton())
            };
                MultiOptionDialog mdo = new MultiOptionDialog(String.Format("This will recover {0} vessel(s), are you sure you want to continue?", suitable.Count), "Confirm recovery", HighLogic.UISkin, options);
                PopupDialog.SpawnPopupDialog(mdo, false, HighLogic.UISkin);
            }
        }

        void resetButton()
        {
            this.button.SetFalse(false);
        }

        void recoverVessels(List<ProtoVessel> vessels)
        {
            foreach (ProtoVessel v in vessels)
            {
                Debug.Log(String.Format("[MassRecover]: {0}", v.vesselName));
                if (v.vesselRef.IsRecoverable)
                    ShipConstruction.RecoverVesselFromFlight(v, HighLogic.fetch.currentGame.flightState);
            }
            resetButton();
        }

    }
}
