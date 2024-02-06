using LlamAcademy.Guns.ImpactEffects;
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
            
            if (EditorApplication.isPlaying) // can't show any visualization if we aren't in play mode
            {
                Camera camera = action.GunSelector.Camera;
                GunScriptableObject gun = action.GunSelector.ActiveGun;
                ParticleSystem shootSystem =
                    gun.GetType().GetField("ShootSystem", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(gun) as ParticleSystem;

                Vector3 startPoint;
                Vector3 forward;

                if (gun.ShootConfig.ShootType == ShootType.FromCamera)
                {
                    startPoint = camera.transform.position + camera.transform.forward *
                        Vector3.Distance(camera.transform.position, shootSystem.transform.position);
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
                Handles.SphereHandleCap(GUIUtility.GetControlID(FocusType.Passive), startPoint, Quaternion.identity,
                    0.25f, EventType.Repaint);
                Handles.ArrowHandleCap(GUIUtility.GetControlID(FocusType.Passive), startPoint,
                    Quaternion.LookRotation(forward), 1, EventType.Repaint);

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

                    if (gun.BulletPenConfig != null && gun.BulletPenConfig.MaxObjectsToPenetrate > 0)
                    {
                        RecursiveDrawPenetration(gun, hit.point, forward, 0);
                    }

                    foreach (ICollisionHandler handler in gun.BulletImpactEffects)
                    {
                        if (handler is AbstractAreaOfEffect)
                        {
                            AbstractAreaOfEffect explosionEffect = (AbstractAreaOfEffect)handler;

                            Collider[] hits = new Collider[explosionEffect.MaxEnemiesAffected];
                            int numberOfEnemiesExploded = Physics.OverlapSphereNonAlloc(hit.point,
                                explosionEffect.Radius, hits, gun.ShootConfig.HitMask);

                            if (numberOfEnemiesExploded > 1)
                            {
                                Handles.color = Color.green;
                            }
                            else
                            {
                                Handles.color = Color.red;
                            }

                            // Handles.SphereHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), hit.point, Quaternion.identity, explosionEffect.Radius, EventType.Repaint);

                            for (int i = 0; i < numberOfEnemiesExploded; i++)
                            {
                                if (hits[i].TryGetComponent(out IDamageable damageable))
                                {
                                    float distance = Vector3.Distance(hit.point, hits[i].ClosestPoint(hit.point));

                                    Handles.Label(hits[i].transform.position - Vector3.up,
                                        $"{Mathf.CeilToInt(explosionEffect.BaseDamage * explosionEffect.DamageFalloff.Evaluate(distance / explosionEffect.Radius))}",
                                        new GUIStyle()
                                        {
                                            normal = new GUIStyleState()
                                            {
                                                textColor = Color.red
                                            }
                                        });
                                }
                            }
                        }
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

        private void RecursiveDrawPenetration(GunScriptableObject Gun, Vector3 Origin, Vector3 Forward, int Iteration)
        {
            if (Iteration >= Gun.BulletPenConfig.MaxObjectsToPenetrate)
            {
                return;
            }
            float maxWallThickness = Gun.BulletPenConfig.MaxPenetrationDepth;
            Vector3 maxImpactRandomness = Gun.BulletPenConfig.AccuracyLoss;
            
            Vector3 inverseOrigin = Origin + Forward * maxWallThickness;
            Handles.color = new Color(1, 0, 1, 0.33f);
            Handles.DrawSolidDisc(inverseOrigin, -Forward, 0.1f);
            if (Physics.Raycast(inverseOrigin, -Forward, out RaycastHit inverseHit, maxWallThickness,
                    Gun.ShootConfig.HitMask))
            {
                Handles.DrawLine(inverseOrigin, inverseHit.point);

                Vector3 startArcPoint = Forward - maxImpactRandomness;

                if (Physics.Raycast(
                        inverseHit.point,
                        Forward, 
                        out RaycastHit secondHit,
                        float.MaxValue,
                        Gun.ShootConfig.HitMask))
                {
                    Handles.color = new Color(1, 0.92f, 0.016f, 0.125f);
                    Handles.DrawSolidArc(inverseHit.point, Forward, startArcPoint,
                        360, Vector3.Distance(inverseHit.point, secondHit.point));
                    
                    Handles.color = Color.yellow;
                    Handles.DrawLine(inverseHit.point, secondHit.point);
                    
                    RecursiveDrawPenetration(Gun, secondHit.point, (secondHit.point - inverseHit.point).normalized, Iteration + 1);
                }
                else
                {
                    Handles.color = new Color(1, 0.92f, 0.016f, 0.125f);
                    Handles.DrawSolidArc(inverseHit.point, Forward, startArcPoint,
                        360, 2f);
                    
                    Handles.color = Color.yellow;
                    Handles.DrawLine(inverseHit.point, inverseHit.point + (Forward) * 50);
                }
            }
        }
    }
}