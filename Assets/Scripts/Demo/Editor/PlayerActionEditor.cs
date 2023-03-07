using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LlamAcademy.Guns.Demo.Editors
{
    [CustomEditor(typeof(PlayerAction))]
    public class PlayerActionEditor : Editor
    {
        private void OnSceneGUI()
        {
            PlayerAction action = (PlayerAction)target;

            if (Application.isPlaying) // can't show any visualization if we aren't in play mode
            {
                Camera camera = action.GunSelector.Camera;
                GunScriptableObject gun = action.GunSelector.ActiveGun;
                ParticleSystem shootSystem = gun.GetType().GetField("ShootSystem", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(gun) as ParticleSystem;

                Vector3 startPoint;
                Vector3 forward;

                if (gun.ShootConfig.ShootType == ShootType.FromCamera)
                {
                    startPoint = camera.transform.position + camera.transform.forward * Vector3.Distance(camera.transform.position, shootSystem.transform.position);
                    forward = camera.transform.forward;

                    Handles.color = Color.red;
                    Handles.DrawLine(camera.transform.position, startPoint);

                }
                else
                {
                    startPoint = shootSystem.transform.position;
                    forward = shootSystem.transform.forward;
                }

                Handles.color = Color.green;
                Handles.SphereHandleCap(GUIUtility.GetControlID(FocusType.Passive), startPoint, Quaternion.identity, 0.25f, EventType.Repaint);
                Handles.ArrowHandleCap(GUIUtility.GetControlID(FocusType.Passive), startPoint, Quaternion.LookRotation(forward), 1, EventType.Repaint);

                if (Physics.Raycast(
                        startPoint,
                        forward,
                        out RaycastHit hit,
                        float.MaxValue,
                        gun.ShootConfig.HitMask))
                {
                    Handles.DrawSolidDisc(hit.point, hit.normal, 0.05f);
                    Handles.DrawLine(startPoint, hit.point);
                    Handles.Label(hit.point, $"{hit.point}");

                    if (gun.ShootConfig.ShootType == ShootType.FromCamera)
                    {
                        Handles.color = Color.yellow;
                        Handles.DrawLine(shootSystem.transform.position, hit.point);
                    }
                }
                else
                {
                    Handles.DrawLine(startPoint, startPoint + forward * 50f);
                }

                Handles.color = Color.green;
                Handles.Label(startPoint - Vector3.up * 0.25f, $"{forward}");
                Handles.Label(startPoint, $"{startPoint}");
            }
        }
    }
}
