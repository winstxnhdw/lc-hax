#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public static class Mask {
    public static readonly LayerMask Default = LayerMask.GetMask("Default");
    public static readonly LayerMask TransparentFX = LayerMask.GetMask("TransparentFX");
    public static readonly LayerMask IgnoreRaycast = LayerMask.GetMask("Ignore Raycast");
    public static readonly LayerMask Player = LayerMask.GetMask("Player");
    public static readonly LayerMask Water = LayerMask.GetMask("Water");
    public static readonly LayerMask UI = LayerMask.GetMask("UI");
    public static readonly LayerMask Props = LayerMask.GetMask("Props");
    public static readonly LayerMask HelmetVisor = LayerMask.GetMask("HelmetVisor");
    public static readonly LayerMask Room = LayerMask.GetMask("Room");
    public static readonly LayerMask InteractableObject = LayerMask.GetMask("InteractableObject");
    public static readonly LayerMask Foliage = LayerMask.GetMask("Foliage");
    public static readonly LayerMask Colliders = LayerMask.GetMask("Colliders");
    public static readonly LayerMask PhysicsObject = LayerMask.GetMask("PhysicsObject");
    public static readonly LayerMask Triggers = LayerMask.GetMask("Triggers");
    public static readonly LayerMask MapRadar = LayerMask.GetMask("MapRadar");
    public static readonly LayerMask NavigationSurface = LayerMask.GetMask("NavigationSurface");
    public static readonly LayerMask RoomLight = LayerMask.GetMask("RoomLight");
    public static readonly LayerMask Anomaly = LayerMask.GetMask("Anomaly");
    public static readonly LayerMask LineOfSight = LayerMask.GetMask("LineOfSight");
    public static readonly LayerMask Enemies = LayerMask.GetMask("Enemies");
    public static readonly LayerMask PlayerRagdoll = LayerMask.GetMask("PlayerRagdoll");
    public static readonly LayerMask MapHazards = LayerMask.GetMask("MapHazards");
    public static readonly LayerMask ScanNode = LayerMask.GetMask("ScanNode");
    public static readonly LayerMask EnemiesNotRendered = LayerMask.GetMask("EnemiesNotRendered");
    public static readonly LayerMask MiscLevelGeometry = LayerMask.GetMask("MiscLevelGeometry");
    public static readonly LayerMask Terrain = LayerMask.GetMask("Terrain");
    public static readonly LayerMask PlaceableShipObjects = LayerMask.GetMask("PlaceableShipObjects");
    public static readonly LayerMask PlacementBlocker = LayerMask.GetMask("PlacementBlocker");
    public static readonly LayerMask Railing = LayerMask.GetMask("Railing");
    public static readonly LayerMask DecalStickableSurface = LayerMask.GetMask("DecalStickableSurface");
    public static readonly LayerMask Ship = LayerMask.GetMask("Ship");
    public static readonly LayerMask ShipInterior = LayerMask.GetMask("ShipInterior");
    public static readonly LayerMask Unused1 = 1 << 30;
    public static readonly LayerMask Unused2 = 1 << 31;

    public static readonly LayerMask All = ~0;


    public static LayerMask Combine(params LayerMask[] layerMasks) {
        LayerMask combinedLayerMask = 0;
        foreach (LayerMask mask in layerMasks) combinedLayerMask |= mask;
        return combinedLayerMask;
    }

    public static LayerMask ToLayerMask(this HashSet<LayerMask> layerMasks) {
        LayerMask combinedMask = 0;
        foreach (LayerMask mask in layerMasks) combinedMask |= mask;
        return combinedMask;
    }

    public static LayerMask ToLayerMask(this List<LayerMask> layerMasks) {
        LayerMask combinedMask = 0;
        foreach (LayerMask mask in layerMasks) combinedMask |= mask;
        return combinedMask;
    }
}
