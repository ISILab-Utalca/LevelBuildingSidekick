using System;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace ISILab.Macros
{
    public class LBSAssetMacro
    {
        
        private const string PlaceholderTextureGuid = "edcbfe04a88995d49aabd5bf8ee28e79";
        
        /// <summary>
        /// Loads an asset of type T from its GUID.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="guid">The GUID of the asset.</param>
        /// <returns>The loaded asset of type T, or null if not found.</returns>
        public static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return !string.IsNullOrEmpty(path) ? AssetDatabase.LoadAssetAtPath<T>(path) : null;
        }
        
        
        /// <summary>
        /// Retrieves the GUID of the given asset.
        /// </summary>
        /// <param name="asset">The asset to retrieve the GUID from.</param>
        /// <returns>The GUID as a string, or null if not found.</returns>
        public static string GetGuidFromAsset(Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.AssetPathToGUID(path);
        }

        /// <summary>
        /// Tries to return a LBSTag
        /// </summary>
        /// <param name="tag">The tag name that you are looking for</param>
        /// <returns></returns>
        public static LBSTag GetLBSTag(string tag)
        {
            var lbsTags = LBSAssetsStorage.Instance.Get<LBSTag>();
            return lbsTags.FirstOrDefault(lbsTag => lbsTag.Label == tag);
        }
        
        public static Texture2D LoadPlaceholderTexture()
        {
            return LoadAssetByGuid<Texture2D>(PlaceholderTextureGuid);
        }
        
    }

    public class LBSLayerHelper
    {
        
        /// <summary>
        /// Retrieves an object of type T from the Behaviours, Assistants, or Modules list within the given layerChild's OwnerLayer.
        /// </summary>
        /// <typeparam name="T">The type of object to find.</typeparam>
        /// <param name="layerChild">An object with an OwnerLayer property that contains Behaviours, Assistants, and Modules</param>
        /// <returns>The first matching object of type T found, or null if not found.</returns>
        public static T GetObjectFromLayerChild<T>(object layerChild) where T : class
        {
            // Use reflection to get the OwnerLayer property
            var ownerLayerProp = layerChild.GetType().GetProperty("OwnerLayer");
            if (ownerLayerProp == null) return null;

            var ownerLayer = ownerLayerProp.GetValue(layerChild);
            if (ownerLayer == null) return null;

            // Look for the T in Behaviours, Assistants, and Modules
            foreach (var listName in new[] { "Behaviours", "Assistants", "Modules" })
            {
                var listProp = ownerLayer.GetType().GetProperty(listName);
                if (listProp == null) continue;

                var list = listProp.GetValue(ownerLayer) as IEnumerable<object>;

                var match = list?.FirstOrDefault(b => b is T);
                if (match != null) return match as T;
            }

            return null;
        }
        
        /// <summary>
        /// Retrieves an object of type T from the Behaviours, Assistants, or Modules list within the provided layer.
        /// </summary>
        /// <typeparam name="T">The type of object to find.</typeparam>
        /// <param name="layer">An object that contains Behaviours, Assistants, and Modules properties.</param>
        /// <returns>The first matching object of type T found, or null if not found.</returns>
        public static T GetObjectFromLayer<T>(object layer) where T : class
        {
            if (layer == null) return null;

            foreach (var listName in new[] { "Behaviours", "Assistants", "Modules" })
            {
                var listProp = layer.GetType().GetProperty(listName);
                if (listProp == null) continue;

                var list = listProp.GetValue(layer) as IEnumerable<object>;
                var match = list?.FirstOrDefault(b => b is T);
                if (match != null) return match as T;
            }

            return null;
        }

        public static Tuple<LBSLayer, TileBundleGroup> GetBundleTileByPosition(Vector2Int TilePosition, List<LBSLayer> Layers)
        {
            foreach (var layer in Layers)
            {
                // Only check layers with a PopulationBehaviour
                var population = layer.GetBehaviour<PopulationBehaviour>();
                if (population == null)
                    continue;

                var tileGroup = population.GetTileGroup(TilePosition);
                if (tileGroup != null)
                {
                    return Tuple.Create(layer, tileGroup);
                }
            }

            return null; // nothing found
        }

        public static Tuple<LBSLayer, TileBundleGroup> GetBundleTileByMouse(Vector2Int mousePosition, List<LBSLayer> Layers)
        {
            foreach (var layer in Layers)
            {
                // Only check layers with a PopulationBehaviour
                var population = layer.GetBehaviour<PopulationBehaviour>();
                if (population == null)
                    continue;

                var tileGroup = population.GetTileGroup(population.OwnerLayer.ToFixedPosition(mousePosition));
                if (tileGroup != null)
                {
                    return Tuple.Create(layer, tileGroup);
                }
            }

            return null; // nothing found
        }

    }
    
    public class LBSVisualElementHelper
    {
                
        /// <summary>
        /// Searches on the parents of a visual element until a parent of a given VisualElement class is found
        /// </summary>
        /// <param name="element">the element from which we will search the parent</param>
        /// <typeparam name="T">VisualElement class we are searching for</typeparam>
        /// <returns></returns>
        public static T FindParentOfType<T>(VisualElement element) where T : VisualElement
        {
            var current = element;
            while (current != null)
            {
                if (current is T target)
                    return target;
                current = current.parent;
            }
            return null;
        }
    }
    
}
