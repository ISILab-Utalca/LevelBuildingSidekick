using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using static ISILab.LBS.Characteristics.LBSDirectionedChance;
using static UnityEngine.GraphicsBuffer;
using System.Data;










#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule), typeof(ConnectedTileMapModule))]
    public class AssistantWFC : LBSAssistant
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private bool overrideValues;

        [JsonProperty, SerializeReference, SerializeField, JsonRequired]
        private Bundle targetBundleRef;
        
        /***
         * Use asset's GUID; current bundle:
         * - "Exterior_Plains" 
         */
        private string defaultBundleGuid = "9d3dac0f9a486fd47866f815b4fefc29";

        private ConnectedTileMapModule.ConnectedTileType? gridType;

        private bool safeMode;

        const int MAX_MEMORY = 3, MAX_RETRIES = 5;
        const int SAVE_STATE_INTERVAL = 10;
        const int MAX_SIZE_X = 10;
        const int MAX_SIZE_Y = 10;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public bool OverrideValues
        {
            get => overrideValues;
            set => overrideValues = value;
        }

        [JsonIgnore]
        public List<Vector2Int> Positions { get; set; }

        [JsonIgnore]
        public Bundle Bundle
        {
            get => GetBundleRef();
            set => targetBundleRef = value;
        }

        [JsonIgnore]
        private List<Vector2Int> Dirs
        {
            get
            {
                switch(GridType)
                {
                    case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                        return Directions.Bidimencional.Edges;
                    case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                        return Directions.Bidimencional.All;
                }
                return new List<Vector2Int>();
            }
        }

        [JsonIgnore]
        private ConnectedTileMapModule.ConnectedTileType GridType
        {
            get
            {
                if(!gridType.HasValue)
                {
                    gridType = OwnerLayer.GetModule<ConnectedTileMapModule>().GridType;
                }

                return gridType.Value;
            }
        }

        [JsonIgnore]
        public bool SafeMode
        {
            get => safeMode;
            set => safeMode = value;
        }

        #endregion

        #region CONSTRUCTORS

        public AssistantWFC(VectorImage icon, string name, Color colorTint, Bundle targetBundleRef = null) : base(icon, name, colorTint)
        {
            if(targetBundleRef != null)
                this.targetBundleRef = targetBundleRef;
            SafeMode = true;
            OnGUI(); 
        }
        
        #endregion

        #region METHODS

        public sealed override void OnGUI()
        {
            GetBundleRef();
        }

        public override object Clone()
        {
            return new AssistantWFC(Icon, Name, ColorTint, targetBundleRef);
        }

        public bool ExecuteTest(bool overrideValues)
        {
            Positions = new List<Vector2Int>();
            this.overrideValues = overrideValues;
            Rect bounds = OwnerLayer.GetModule<TileMapModule>().GetBounds();
            for(int i = (int)bounds.x; i < (int)(bounds.x + bounds.width); i++)
            {
                for(int j = (int)bounds.y; j < (int)(bounds.y + bounds.height); j++)
                {
                    Positions.Add(new Vector2Int(i, j));
                }
            }

            return TryExecute(out _, out _);
        }

        public bool TryExecute(out string log, out LogType logType, int limit = 5)
        {
            log = "";
            logType = LogType.Log;

            // Get Bundle
            OnGUI();

            if (targetBundleRef == null)
            {
                log = "No bundle selected.";
                logType = LogType.Warning;
                return false;
            }

            /*
            if(targetBundleRef.GetCharacteristics<LBSDirectionedGroup>().Count == 0)
            {
                log = "Cannot generate. Invalid bundle.";
                logType = LogType.Warning;
                return false;
            }
            */

            var sw = System.Diagnostics.Stopwatch.StartNew();
            Func<double> getSeconds = () =>
            {
                sw.Stop();
                long ticks = sw.ElapsedTicks;
                return (double)ticks / System.Diagnostics.Stopwatch.Frequency;
            };

            var originalPositions = new List<Vector2Int>(Positions);

            if (safeMode)
            {
                int xStart = Positions.OrderBy(p => p.x).First().x;
                int yStart = Positions.OrderBy(p => p.y).First().y;
                int xEnd = Positions.OrderBy(p => -p.x).First().x;
                int yEnd = Positions.OrderBy(p => -p.y).First().y;
                int width = xEnd - xStart + 1;
                int height = yEnd - yStart + 1;
                RectInt rect = new RectInt(new Vector2Int(xStart, yStart), new Vector2Int(width, height));
                int xSectors = Mathf.CeilToInt((float)rect.width / (float)MAX_SIZE_X);
                int ySectors = Mathf.CeilToInt((float)rect.height / (float)MAX_SIZE_Y);
                int sectorSizeX = Mathf.CeilToInt((float)width / (float)xSectors);
                int sectorSizeY = Mathf.CeilToInt((float)height / (float)ySectors);
                Vector2Int sectorSize = new Vector2Int(sectorSizeX, sectorSizeY);
                List<RectInt> sectors = new List<RectInt>();
                for(int i = 0; i < xSectors; i++)
                {
                    for(int j = 0; j < ySectors; j++)
                    {
                        Vector2Int offset = new Vector2Int(sectorSizeX * i, sectorSizeY * j);
                        RectInt sector = new RectInt(rect.position + offset, new Vector2Int(sectorSizeX, sectorSizeY));
                        sectors.Add(sector);
                    }
                }
                for (int i = 0; i < limit; i++)
                {
                    int sectorSuccessCount = 0;
                    foreach(RectInt sector in sectors)
                    {
                        List<Vector2Int> positions = new List<Vector2Int>();
                        for(int j = sector.position.x; j < sector.position.x + sector.width; j++)
                        {
                            for(int k = sector.position.y; k < sector.position.y + sector.height; k++)
                            {
                                positions.Add(new Vector2Int(j, k));
                            }
                        }
                        Positions = positions;
                        bool sectorSuccess = Execute();//ExecuteChance();
                        if (sectorSuccess)
                        {
                            sectorSuccessCount++;
                        }
                        else break;
                        
                    }
                    if (sectorSuccessCount >= sectors.Count)
                    {
                        log = $"Safely generated after {i + 1} attempts. ({getSeconds()} s)";

                        RequestRepaint();
                        return true;
                    }
                }

                log = $"Could not safely generate after {limit} attempts. ({getSeconds()} s)";
                logType = LogType.Warning;

                //RequestRepaint();
                return false;
            }
            else
            {
                Execute();//ExecuteChance();
                log = $"Generated. ({getSeconds()} s)";
                RequestRepaint();
                return true;
            }

            void RequestRepaint()
            {
                var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();
                ExteriorBehaviour exterior = OwnerLayer.GetBehaviour<ExteriorBehaviour>();
                var ogPairs = originalPositions.Select(pos => connected.GetPair(pos)).ToList().RemoveEmpties();
                exterior.RequestTilesRepaint(ogPairs.Select(p => p.Tile));
                var others = new List<Vector2Int>();
                int minX = originalPositions.Min(pos => pos.x) - 1;
                int maxX = originalPositions.Max(pos => pos.x) + 1;
                int minY = originalPositions.Min(pos => pos.y) - 1;
                int maxY = originalPositions.Max(pos => pos.y) + 1;
                for(int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        if(x == minX || x == maxX || y == minY || y == maxY)
                        {
                            others.Add(new Vector2Int(x, y));
                        }
                    }
                }
                ;
                var pairs = others.Select(pos => connected.GetPair(pos)).ToList().RemoveEmpties();
                exterior.RequestTilesRepaint(pairs.Select(p => p.Tile));
                //originalPositions.ForEach(pos => RequestTilePaint(connected.GetPair(pos).Tile));
            }
        }

        /// <summary>
        /// This new version, is similar but it constraints where the wave function collapse is applied, to the selected tiles only
        /// </summary>
        public bool Execute()
        {
            bool success = false;

            int initialRetryBonus = 10;
            (int, int) retryCount = (MAX_MEMORY, MAX_RETRIES + initialRetryBonus);
            int step = 0, maxStep = 0;

            Bundle bundle = targetBundleRef;

            var group = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];
            var map = OwnerLayer.GetModule<TileMapModule>();
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();
            var og = new List<LBSModule>() { OwnerLayer.GetModule<ConnectedTileMapModule>() };
            var originalTM = og.Clone()[0] as ConnectedTileMapModule;

            // Paso 1
            // Get tiles to change
            List<LBSTile> toCalc = GetTileToCalc(map, connected);

            // Build whitelist (positions + direct neighbors)
            // and selection area neighbourhood
            var whitelist = new HashSet<Vector2Int>();
            var areaNeighbours = new List<(LBSTile, int)>();
            bool implemented = true;
            foreach (LBSTile tile in toCalc)
            {
                whitelist.Add(tile.Position);
                List<LBSTile> neighbours = map.GetTileNeighbors(tile, Dirs);

                for(int i = 0; i < neighbours.Count; i++)
                {
                    if (neighbours[i] == null) continue;

                    bool isAreaNeighbour = !toCalc.Contains(neighbours[i]);
                    bool haveEmpties = connected.GetConnections(neighbours[i]).Contains("");

                    if (isAreaNeighbour && haveEmpties)
                        continue;

                    whitelist.Add(neighbours[i].Position);

                    if(isAreaNeighbour)
                    {
                        switch(GridType)
                        {
                            case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                                areaNeighbours.Add((neighbours[i], (i+2)%4));
                                break;
                            case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                                implemented = false;
                                break;
                        }
                    }
                }
            }
            if(implemented)
            {
                foreach ((LBSTile, int) areaNeighbour in areaNeighbours)
                {
                    connected.SetConnection(areaNeighbour.Item1, areaNeighbour.Item2, "", false);
                    toCalc.Add(areaNeighbour.Item1);
                }
            }
            else Debug.LogError("Unhandled case for Vertex-based grid. Could not build area neighbourhood.");

            //Lista que guarda los tiles ya generados
            var closed = new List<LBSTile>();

            //Tiles que tienen que recalcular los candidatos después de generar un tile
            //generalmente van los tiles vecinos del que se generó
            var reCalc = new List<LBSTile>();

            //Diccionario que guarda los candidatos posibles para cada tile
            var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();

            //Paso 2
            foreach (LBSTile tile in toCalc)
            {
                List<Candidate> candidates = CalcCandidates(tile, group);
                currentCalcs.Add(tile, candidates);
            }

            List<WFCState> states = new List<WFCState>();
            if(safeMode)
            {
                states.Add(new WFCState(0, connected, toCalc, closed, currentCalcs));
            }
            bool stepSuccess = true;
            int tryCount = 0;

            /// MAIN LOOP
            while (toCalc.Count > 0)
            {
                tryCount++;

                //Paso 3

                List<KeyValuePair<LBSTile, List<Candidate>>> xx = safeMode ? 
                    currentCalcs.Where(e => !closed.Contains(e.Key)).ToList() :
                    currentCalcs.Where(e => e.Value.Count > 1).ToList();

                if (xx.Count <= 0)
                    break;

                // Paso 3.5

                KeyValuePair<LBSTile, List<Candidate>> current = xx.OrderBy(e => e.Value.Count).First();

                // If cannot generate next tile
                if(safeMode && (!stepSuccess || current.Value.Count <= 0))
                {
                    if (Backtrack(states, ref retryCount, connected, originalTM, ref step, maxStep, ref toCalc, ref closed, ref currentCalcs))
                    {
                        stepSuccess = true;
                        Debug.Log($"TRY: {tryCount}\tSTEP {step}\tMAX STEP {maxStep}\tRETRY COUNT {retryCount}");
                        continue;
                    }
                    else return false;
                }

                stepSuccess = true;

                //Paso 4

                //a
                Candidate selected = current.Value.RandomRullete(c => c.weigth);
                
                //b
                List<string> connections = selected.bundle.GetConnection(selected.rotation).ToList();
                
                //c
                connected.SetConnections(current.Key, connections, new List<bool>() { false, false, false, false });
                
                //d
                currentCalcs[current.Key] = new List<Candidate>() { selected };
                closed.Add(current.Key);

                var _closed = new List<LBSTile>(closed);

                //Paso 5
                List<LBSTile> neigth = map.GetTileNeighbors(current.Key, Dirs);
                SetConnectionNei(current.Key, neigth.ToArray(), closed, whitelist);

                List<LBSTile> neigthCalcs = neigth.RemoveEmpties()
                                         .Where(n => currentCalcs.ContainsKey(n) && whitelist.Contains(n.Position))
                                         .ToList();
                reCalc.AddRange(neigthCalcs);

                //bool noCandidatesFlag = false;

                //Paso 6
                while (reCalc.Count > 0)
                {
                    LBSTile tile = reCalc.First();

                    if (!whitelist.Contains(tile.Position))
                    {
                        reCalc.Remove(tile);
                        continue;
                    }

                    currentCalcs.TryGetValue(tile, out List<Candidate> lastCandidates);
                    List<Candidate> newCandidates = CalcCandidates(tile, group);

                    if (safeMode && newCandidates.Count == 0)
                    {
                        // No possible candidates: must revert step in next iteration
                        stepSuccess = false;
                        reCalc.Clear();
                        break;
                    }

                    if (lastCandidates == null || newCandidates.Count < lastCandidates.Count)
                    {
                        currentCalcs[tile] = newCandidates;

                        List<LBSTile> neighs = map.GetTileNeighbors(tile, Dirs).RemoveEmpties();
                        //foreach (LBSTile nei in neighs)
                        for(int i = 0; i < neighs.Count; i++)
                        {
                            if (_closed.Contains(neighs[i]) || reCalc.Contains(neighs[i]))
                                continue;

                            if (whitelist.Contains(neighs[i].Position))
                                reCalc.Add(neighs[i]);
                        }
                    }

                    reCalc.Remove(tile);
                    _closed.Add(tile);
                }

                toCalc.Remove(current.Key);

                step++;
                // Restore retry limit if further progress
                if(step > maxStep)
                {
                    maxStep = step;
                    if(maxStep > SAVE_STATE_INTERVAL)
                        initialRetryBonus = 0;
                    retryCount = (MAX_MEMORY, MAX_RETRIES + initialRetryBonus);
                }

                if(safeMode)
                {
                    //Debug.Log($"TRY: {tryCount}\tSTEP {step}\tMAX STEP {maxStep}\tRETRY COUNT {retryCount}");
                    if(step % SAVE_STATE_INTERVAL == 0)
                    {
                        // Save state
                        states.Add(new WFCState(step, connected, toCalc, closed, currentCalcs));
                        if (states.Count > MAX_MEMORY + 1)
                        {
                            states.RemoveAt(0);
                        }
                    }
                }
            }

            success = toCalc.Count == 0;
            if (safeMode && !success) connected.Rewrite(originalTM);
            return success;
        }

        public bool ExecuteChance()
        {
            bool success = false;

            int initialRetryBonus = 10;
            (int, int) retryCount = (MAX_MEMORY, MAX_RETRIES + initialRetryBonus);
            int step = 0, maxStep = 0;

            Bundle bundle = targetBundleRef;

            var group = bundle.GetCharacteristics<LBSDirectionedChance>()[0];
            var map = OwnerLayer.GetModule<TileMapModule>();
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

            //guarda el mapa original para restaurarlo en caso de fallo
            var og = new List<LBSModule>() { OwnerLayer.GetModule<ConnectedTileMapModule>() };
            var originalTM = og.Clone()[0] as ConnectedTileMapModule;

            // Get tiles to change
            List<LBSTile> toCalc = GetTileToCalc(map, connected);

            // Build whitelist (positions + direct neighbors)
            // and selection area neighbourhood
            var whitelist = new HashSet<Vector2Int>();
            var areaNeighbours = new List<(LBSTile, int)>();
            bool implemented = true;
            foreach (LBSTile tile in toCalc)
            {
                whitelist.Add(tile.Position);
                List<LBSTile> neighbours = map.GetTileNeighbors(tile, Dirs);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (neighbours[i] == null) continue;

                    bool isAreaNeighbour = !toCalc.Contains(neighbours[i]);
                    bool haveEmpties = connected.GetConnections(neighbours[i]).Contains("");

                    if (isAreaNeighbour && haveEmpties)
                        continue;

                    whitelist.Add(neighbours[i].Position);

                    if (isAreaNeighbour)
                    {
                        switch (GridType)
                        {
                            case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                                areaNeighbours.Add((neighbours[i], (i + 2) % 4));
                                break;
                            case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                                implemented = false;
                                break;
                        }
                    }
                }
            }
            if (implemented)
            {
                foreach ((LBSTile, int) areaNeighbour in areaNeighbours)
                {
                    connected.SetConnection(areaNeighbour.Item1, areaNeighbour.Item2, "", false);
                    toCalc.Add(areaNeighbour.Item1);
                }
            }
            else Debug.LogError("Unhandled case for Vertex-based grid. Could not build area neighbourhood.");

            var closed = new List<LBSTile>();
            var reCalc = new List<LBSTile>();
            var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();

            foreach (LBSTile tile in toCalc)
            {
                List<Candidate> candidates = CalcCandidates(tile, group, closed, map);
                currentCalcs.Add(tile, candidates);
            }

            List<WFCState> states = new List<WFCState>();
            if (safeMode)
            {
                states.Add(new WFCState(0, connected, toCalc, closed, currentCalcs));
            }
            bool stepSuccess = true;
            int tryCount = 0;

            /// MAIN LOOP
            while (toCalc.Count > 0)
            {
                tryCount++;

                List<KeyValuePair<LBSTile, List<Candidate>>> xx = safeMode ?
                    currentCalcs.Where(e => !closed.Contains(e.Key)).ToList() :
                    currentCalcs.Where(e => e.Value.Count > 1).ToList();

                if (xx.Count <= 0)
                    break;

                KeyValuePair<LBSTile, List<Candidate>> current = xx.OrderBy(e => e.Value.Count).First();

                // If cannot generate next tile
                if (safeMode && (!stepSuccess || current.Value.Count <= 0))
                {
                    if (Backtrack(states, ref retryCount, connected, originalTM, ref step, maxStep, ref toCalc, ref closed, ref currentCalcs))
                    {
                        stepSuccess = true;
                        Debug.Log($"TRY: {tryCount}\tSTEP {step}\tMAX STEP {maxStep}\tRETRY COUNT {retryCount}");
                        continue;
                    }
                    else return false;
                }

                stepSuccess = true;

                Candidate selected = current.Value.RandomRullete(c => c.weigth);
                List<string> connections = selected.bundle.GetConnection(selected.rotation).ToList();
                connected.SetConnections(current.Key, connections, new List<bool>() { false, false, false, false });
                currentCalcs[current.Key] = new List<Candidate>() { selected };
                closed.Add(current.Key);

                var _closed = new List<LBSTile>(closed);

                List<LBSTile> neigth = map.GetTileNeighbors(current.Key, Dirs);
                SetConnectionNei(current.Key, neigth.ToArray(), closed, whitelist);

                List<LBSTile> neigthCalcs = neigth.RemoveEmpties()
                                         .Where(n => currentCalcs.ContainsKey(n) && whitelist.Contains(n.Position))
                                         .ToList();
                reCalc.AddRange(neigthCalcs);

                //bool noCandidatesFlag = false;

                while (reCalc.Count > 0)
                {
                    LBSTile tile = reCalc.First();

                    if (!whitelist.Contains(tile.Position))
                    {
                        reCalc.Remove(tile);
                        continue;
                    }

                    currentCalcs.TryGetValue(tile, out List<Candidate> lastCandidates);
                    List<Candidate> newCandidates = CalcCandidates(tile, group, closed, map);

                    if (safeMode && newCandidates.Count == 0)
                    {
                        // No possible candidates: must revert step in next iteration
                        stepSuccess = false;
                        reCalc.Clear();
                        break;
                    }

                    if (lastCandidates == null || newCandidates.Count < lastCandidates.Count)
                    {
                        currentCalcs[tile] = newCandidates;

                        List<LBSTile> neighs = map.GetTileNeighbors(tile, Dirs).RemoveEmpties();
                        //foreach (LBSTile nei in neighs)
                        for (int i = 0; i < neighs.Count; i++)
                        {
                            if (_closed.Contains(neighs[i]) || reCalc.Contains(neighs[i]))
                                continue;

                            if (whitelist.Contains(neighs[i].Position))
                                reCalc.Add(neighs[i]);
                        }
                    }

                    reCalc.Remove(tile);
                    _closed.Add(tile);
                }

                toCalc.Remove(current.Key);

                step++;
                // Restore retry limit if further progress
                if (step > maxStep)
                {
                    maxStep = step;
                    if (maxStep > SAVE_STATE_INTERVAL)
                        initialRetryBonus = 0;
                    retryCount = (MAX_MEMORY, MAX_RETRIES + initialRetryBonus);
                }

                if (safeMode)
                {
                    Debug.Log($"TRY: {tryCount}\tSTEP {step}\tMAX STEP {maxStep}\tRETRY COUNT {retryCount}");
                    if (step % SAVE_STATE_INTERVAL == 0)
                    {
                        // Save state
                        states.Add(new WFCState(step, connected, toCalc, closed, currentCalcs));
                        if (states.Count > MAX_MEMORY + 1)
                        {
                            states.RemoveAt(0);
                        }
                    }
                }
            }

            success = toCalc.Count == 0;
            if (safeMode && !success) connected.Rewrite(originalTM);
            return success;
        }

        private bool Backtrack(
            List<WFCState> states, ref (int, int) retryCount, 
            ConnectedTileMapModule currentTM, ConnectedTileMapModule originalTM, 
            ref int currentStep, int maxStep,
            ref List<LBSTile> toCalc, ref List<LBSTile> closed, ref Dictionary<LBSTile, List<Candidate>> currentCalcs)
        {
            // Decrease step retries
            retryCount.Item2--;
            // If step retries run out, it rollbacks to previous state
            if (retryCount.Item2 <= 0)
            {
                retryCount.Item2 = MAX_RETRIES;
                retryCount.Item1--;
                // If it reaches maximum number of reverts allowed, it cancels generation
                if (retryCount.Item1 <= 0)
                {
                    currentTM.Rewrite(originalTM);
                    return false;
                }
            }
            // Determines target step and number of steps to revert
            int offset = (MAX_MEMORY - retryCount.Item1) * SAVE_STATE_INTERVAL + (maxStep % SAVE_STATE_INTERVAL);
            int targetStep = maxStep - offset;
            int stepsToRevert = currentStep - targetStep;
            currentStep = targetStep;
            if (currentStep < 0)
            {
                currentTM.Rewrite(originalTM);
                return false;
            }

            int statesToRevert = stepsToRevert / SAVE_STATE_INTERVAL;

            states.Reverse();
            for (int i = 0; i < statesToRevert; i++)
                states.RemoveAt(0);
            WFCState prevState = states[0];
            currentTM.Rewrite(prevState.tileMap);
            toCalc = prevState.toCalc.Clone();
            closed = prevState.closed.Clone();
            //currentCalcs = prevState.currentCalcs.Clone(); //revisar clonacion
            currentCalcs = new Dictionary<LBSTile, List<Candidate>>(prevState.currentCalcs);
            states.Reverse();

            return true;
        }

        public void SetConnectionNei(LBSTile origin, LBSTile[] neis, List<LBSTile> closed, HashSet<Vector2Int> whitelist)
        {
            var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();
            List<string> originConnections = connected.GetConnections(origin);

            for (int i = 0; i < neis.Length; i++)
            {
                LBSTile nei = neis[i];
                if (nei == null || closed.Contains(nei))
                    continue;

                if (!whitelist.Contains(nei.Position))
                    continue;

                List<int> indices = new List<int>();
                switch(GridType)
                {
                    case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                        indices.Add(Dirs[i].GetEdge(Dirs));
                        connected.SetConnection(nei, indices[0], originConnections[i], false);
                        break;
                    case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                        indices.AddRange(Dirs[i].GetVertices(out List<int> originIndices));
                        bool invert = !(originIndices.SequenceEqual(new[] { 0, 3 }) || originIndices.SequenceEqual(new[] { 1, 2 }));
                        for (int j = 0; j < indices.Count; j++)
                        {
                            int dirIndex = indices[j];
                            int k = invert ? indices.Count - 1 - j : j;
                            int originInd = originIndices[k];
                            connected.SetConnection(nei, dirIndex, originConnections[originInd], false);
                        }
                        break;
                }
            }
        }

        private List<LBSTile> GetTileToCalc(TileMapModule map, ConnectedTileMapModule connected)
        {
            var toR = new List<LBSTile>();
            foreach (var position in Positions)
            {
                // Get tile information
                var tile = map.GetTile(position);

                // Check if tile is null
                if (tile == null)
                    continue;

                // Get connections
                //var connection = connected.GetConnections(tile);

                if (overrideValues)
                {
                    //Clear prev connection
                    connected.SetConnections(tile,
                        new List<string>() { "", "", "", "" },
                        new List<bool>() { false, false, false, false });
                }

                toR.Add(tile);
            }
            return toR;
        }

        private List<Candidate> CalcCandidates(LBSTile tile, LBSDirectionedGroup group)
        {
            // Get modules
            var connectedMod = OwnerLayer.GetModule<ConnectedTileMapModule>();

            var candidates = new List<Candidate>();
            for (int i = 0; i < group.Weights.Count; i++)
            {
                // Get characteristics and weigh
                float weigth = group.Weights[i].weight;
                LBSDirection sBundle = group.Weights[i].target.GetCharacteristics<LBSDirection>()[0];

                for (int j = 0; j < 4; j++)
                {
                    // Get connection rotated
                    string[] array = sBundle.GetConnection(j); //(!)

                    // Check if is valid rotated connection
                    List<string> connections = connectedMod.GetConnections(tile);
                    if (Compare(connections.ToArray(), array))
                    {
                        var candidate = new Candidate()
                        {
                            bundle = sBundle,
                            weigth = weigth,
                            rotation = j,
                        };

                        candidates.Add(candidate);
                    }
                }
            }

            return candidates;
        }

        private List<Candidate> CalcCandidates(LBSTile tile, LBSDirectionedChance chanceGroup,
                List<LBSTile> closedList, TileMapModule map)
        {
            //TO DO
            //Check que chucha está pasando

            var candidates = new List<Candidate>();
            var connectedMod = OwnerLayer.GetModule<ConnectedTileMapModule>();
            var neighbors = map.GetTileNeighbors(tile, Dirs);

            foreach (var tileDir in chanceGroup.tileDirections)
            {
                var candidateBundle = tileDir.mainTarget;
                float totalChance = 0f;
                int neighborCount = 0;
                bool allNeighborsAccept = true;

                for (int dirIndex = 0; dirIndex < neighbors.Count; dirIndex++)
                {
                    var neighbor = neighbors[dirIndex];
                    if (neighbor == null || closedList.Contains(neighbor))
                        continue;

                    // Busca el TileDirection del vecino en la dirección opuesta
                    var neighborDir = chanceGroup.tileDirections.FirstOrDefault(td =>
                        td != null &&
                        td.direction != null &&
                        td.direction.Contains((dirIndex + 2) % 4) // dirección opuesta
                    );

                    if (neighborDir == null || neighborDir.chances == null)
                    {
                        allNeighborsAccept = false;
                        break;
                    }

                    // Busca si el candidato existe en los chances del vecino
                    var chanceObj = neighborDir.chances.FirstOrDefault(c => c.target == candidateBundle);
                    if (chanceObj == null)
                    {
                        allNeighborsAccept = false;
                        break;
                    }

                    totalChance += chanceObj.chance;
                    neighborCount++;
                }

                if (allNeighborsAccept && neighborCount > 0)
                {
                    float avgChance = totalChance / neighborCount;
                    var sBundle = candidateBundle.GetCharacteristics<LBSDirection>()[0];

                    for (int rot = 0; rot < 4; rot++)
                    {
                        string[] array = sBundle.GetConnection(rot);
                        List<string> connections = connectedMod.GetConnections(tile);

                        if (Compare(connections.ToArray(), array))
                        {
                            candidates.Add(new Candidate
                            {
                                bundle = sBundle,
                                weigth = avgChance,
                                rotation = rot
                            });
                        }
                    }
                }
            }

            return candidates;
        }



        private List<Candidate> CalcCandidates()
        {
            return new List<Candidate>();
        }

        public bool CaptureWeights(out string errMsg)
        {
            errMsg = null;

            List<TileConnectionsPair> pairs = OwnerLayer.GetModule<ConnectedTileMapModule>().Pairs;
            if(pairs.Count == 0)
            {
                errMsg = "Empty map! Could not capture its weights.";
                return false;
            }

            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];

            var currentBundles = new List<Bundle>();
            group.Weights.ForEach(ws => currentBundles.Add(ws.target));

            var bundleFrequency = new Dictionary<Bundle, int>();
            int maxFreq = 0;
            currentBundles.ForEach(b => bundleFrequency.Add(b, 0));

            for(int i = 0; i < pairs.Count; i++)
            {
                bool matchFound = false;
                List<string> tileConns = pairs[i].Connections;
                for(int j = 0; j < currentBundles.Count; j++)
                {
                    Bundle bundle = currentBundles[j];
                    LBSDirection directionChar = bundle.GetCharacteristics<LBSDirection>()[0];
                    //List<string> bundleConns = directionChar.Connections;
                    for (int k = 0; k < 4; k++)
                    {
                        List<string> rotatedBundleConns = directionChar.GetConnection(k).ToList();//bundleConns.Rotate(k);

                        if(Compare(tileConns.ToArray(), rotatedBundleConns.ToArray(), false))
                        {
                            bundleFrequency[bundle]++;
                            if(bundleFrequency[bundle] > maxFreq)
                                maxFreq = bundleFrequency[bundle];
                            matchFound = true;
                            j = currentBundles.Count;
                            break;
                        }
                    }
                }

                if (!matchFound)
                    Debug.LogWarning($"Tile {pairs[i].Tile.Position} has no matching bundle");
            }

            if(maxFreq == 0)
            {
                errMsg = "Empty map! Could not capture its weights.";
                return false;
            }
            
            for (int i = 0; i < currentBundles.Count; i++) 
            {
                //Debug.Log($"{currentBundles[i]} Frequency: {bundleFrequency[currentBundles[i]]}");
                group.Weights[i].weight = maxFreq != 0 ? (float)bundleFrequency[currentBundles[i]] / (float)maxFreq : 1;
            }

            //Selection.activeObject = targetBundleRef;
            RefreshInspector(targetBundleRef);

            return true;
        }

        //out string errMsg

        public bool CaptureRules()
        {
            Selection.activeObject = targetBundleRef;

            //errMsg = null;

            Dictionary<TileConnectionsPair, List<TileChance>> tileChances = new();

            var group = targetBundleRef.GetCharacteristics<LBSDirectionedChance>()[0];

            List<TileConnectionsPair> pairs = OwnerLayer.GetModule<ConnectedTileMapModule>().Pairs;

            if (pairs.Count == 0)
            {
                //errMsg = "Empty map! Could not capture its weights.";
                return false;
            }

            pairs = pairs.OrderBy(t => -t.Tile.Position.y).ThenBy(t => t.Tile.Position.x).ToList();

            var currentBundles = new List<Bundle>();
            group.tileDirections.ForEach(ws => currentBundles.Add(ws.mainTarget));

            foreach (var p in pairs)
            {
                bool newfound = false;
                var adjacent = GetAdjacentFromCurrent(pairs, p);
                List<TileChance> aux = new();

                foreach (TileConnectionsPair key in tileChances.Keys)
                {
                    // Check if each connection from key and p are equal
                    if (key.Connections.SequenceEqual(p.Connections))
                    {
                        foreach (TileChance tca in adjacent)
                        {
                            // Busca si ya existe un TileChance igual en tileChances[key]
                            var existing = tileChances[key].FirstOrDefault(tc => tc.Equals(tca));
                            if (existing != null)
                            {
                                existing.count += tca.count;
                            }
                            else
                            {
                                tileChances[key].Add(new TileChance(tca.tile, tca.count, tca.direction));
                            }
                        }
                        newfound = true;
                    }
                }

                if (!newfound)
                {
                    tileChances.Add(p, adjacent);
                }
            }

            TileDirection td = null;

            foreach (var rule in tileChances)
            {
                td = new()
                {
                    mainTarget = FindEqualConnection(currentBundles, rule.Key.Connections),
                    direction = new List<int>()
                };
                //Debug.Log(rule.Key);

                int total = rule.Value.Sum(t => t.count);
                //Debug.Log(total);

                foreach (var pair in rule.Value)
                {
                    td.direction.Add(pair.direction);

                    TileDirectionChance tileDirectionChance = new()
                    {
                        target = FindEqualConnection(currentBundles, pair.tile.Connections),
                        chance = (float)pair.count / total
                    };
                    td.chances.Add(tileDirectionChance);
                    

                    //float chance = (float)pair.count / total;
                    //Debug.Log($" - {string.Join(", ", pair.tile + " " + pair.direction + " Count: " + pair.count + " Chance: " + chance.ToString("F2"))}");
                }

                group.tileDirections.Add(td);
            }

            group.tileDirections.RemoveAll(td => !td.chances.Any());

            foreach (var item in group.tileDirections)
            {
                Debug.Log("- " + item.mainTarget.BundleName);

                for (int i = 0; i < item.direction.Count; i++)
                {
                    Debug.Log("-> " + item.direction[i]);
                    foreach (var item2 in item.chances)
                    {
                        Debug.Log(item2.target + " " + item2.chance.ToString());
                    }
                }
            }

            RefreshInspector(targetBundleRef);

            return true;
        }

        private Bundle FindEqualConnection(List<Bundle> bundle, List<string> tileConnection)
        {
            int count = 0;

            for (int i = 0; i < bundle.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (bundle[i].GetCharacteristics<LBSDirection>()[0].Connections[j] == tileConnection[j])
                    {
                        count++;
                    }

                    if (count == 4)
                    {
                        return bundle[i];
                    }
                }

            }

            return null;
        }


        private void ArrangeListByPosition(List<TileConnectionsPair> tiles)
        {
            tiles = tiles.OrderBy(t => t.Tile.Position.x).ThenBy(t => t.Tile.Position.y).ToList();
        }

        private List<TileChance> GetAdjacentFromCurrent(List<TileConnectionsPair> tiles, TileConnectionsPair current)
        {
            List<TileChance> adjacent = new();

            foreach (var tile in tiles)
            {
                if (tile.Tile == current.Tile)
                    continue;

                Vector2Int currentPos = current.Tile.Position;
                Vector2Int tilePos = tile.Tile.Position;

                for (int i = 0; i < 4; i++)
                {
                    if (tilePos == currentPos + Directions.Bidimencional.Edges[i])
                    {
                        //0 => "Right",
                        //1 => "Up",
                        //2 => "Left",
                        //3 => "Down",

                        //Busca en la lista de adjacent si hay uno con el mismo tile y direccion

                        TileChance t = new(tile, i);

                        adjacent.Add(t);
                        continue;
                    }
                    
                }
            }

            return adjacent;
        }


        public bool SaveWeights(string presetName, string folder, out string endName, out WFCPreset newPreset, out string errMsg)
        {
            endName = null;
            newPreset = null;
            errMsg = null;

            if(string.IsNullOrEmpty(folder))
            {
                errMsg = "Cannot save preset. You need to specify a Save Folder.";
                return false;
            }

            endName = presetName;
            if (endName.Length == 0)
            {
                endName = "New WFC Preset";
            }
            if(endName == "New WFC Preset")
            {
                int count = AssetDatabase.FindAssets(endName).Length;
                if(count > 0)
                {
                    endName += $" ({count})";
                }
            }
            string path = folder + "/" + endName + ".asset";
            bool overwrite = AssetDatabase.FindAssets(endName, new[] { folder })
                .Count(guid => AssetDatabase.GUIDToAssetPath(guid).Equals(path)) > 0;
            if (overwrite)
            {
                bool confirmOverwrite = EditorUtility.DisplayDialog("Overwrite?", $"You are about to overwrite the WFC preset at {path}. Continue?", "Yes", "No");
                if (!confirmOverwrite) return false;
            }

            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];
            newPreset = ScriptableObject.CreateInstance<WFCPreset>();
            newPreset.Name = endName;
            newPreset.SetWeights(group.Weights);

            AssetDatabase.CreateAsset(newPreset, folder + "/" + endName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = newPreset;

            return true;
        }

        public void LoadWeights(WFCPreset preset)
        {
            var group = targetBundleRef.GetCharacteristics<LBSDirectionedGroup>()[0];
            for (int i = 0; i < group.Weights.Count; i++)
            {
                bool found = false;
                foreach (var presetWS in preset.GetWeights()) 
                {
                    if (group.Weights[i].target.Equals(presetWS.target))
                    {
                        group.Weights[i].weight = presetWS.weight;
                        found = true;
                        break;
                    }
                }
                // Testear cambiando los bundles hijos
                if(!found)
                    Debug.LogWarning($"Bundle '{group.Weights[i].target}' was not in preset '{preset.Name}'");
            }

            // Refresh bundle on inspector. Works inconsistently.
            RefreshInspector(targetBundleRef);
            //Selection.activeObject = null;
            //EditorApplication.delayCall += () => Selection.activeObject = targetBundleRef;
        }

        private void RefreshInspector(UnityEngine.Object target)
        {
            Action makeNull = () => Selection.activeObject = null;
            Action set = () => Selection.activeObject = target;

            EditorApplication.delayCall += () => 
            { 
                makeNull();
                EditorApplication.delayCall += () => set();
            };
        }

        public bool Compare(string[] a, string[] b, bool ignoreEmpties = true)
        {
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    if (!a[i].Equals(b[i]))
                    {
                        bool empties = string.IsNullOrEmpty(a[i]) || string.IsNullOrEmpty(b[i]);
                        if (ignoreEmpties && empties)
                            continue;
                        else return false;
                    }
                }
            }
            return true;
        }

        public Bundle GetBundle(string bundleID)
        {
            // Get Target bundle
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            foreach (var bundle in bundles)
            {
                if (bundle.name == bundleID)
                {
                    return bundle;
                }
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AssistantWFC;

            if (other == null) return false;

            if (!other.Name.Equals(Name)) return false;

            if (!Equals(other.targetBundleRef, targetBundleRef))
                return false;


            if (!other.overrideValues.Equals(overrideValues)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
       
        public Bundle GetBundleRef()
        {
            if (!targetBundleRef) // if it's null load default
            {
                targetBundleRef = LBSAssetMacro.LoadAssetByGuid<Bundle>(defaultBundleGuid);
            }
            
            return targetBundleRef;
        }
        #endregion

        #region DEPRECATED

        //public void OLDExecute()
        //{
        //    // Get Bundle
        //    OnGUI();
        //    var bundle = targetBundleRef;// GetBundle(targetBundle);

        //    // Cheack if can execute
        //    if (bundle == null)
        //    {
        //        Debug.LogWarning("No bundle selected.");
        //        return;
        //    }

        //    // Get bundles posible tiles
        //    var cGroup = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];

        //    // Get modules
        //    var map = OwnerLayer.GetModule<TileMapModule>();
        //    var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

        //    // Get tiles to change
        //    var toCalc = GetTileToCalc(Positions, map, connected);

        //    // Create auxiliar collections
        //    var closed = new List<LBSTile>();
        //    var reCalc = new List<LBSTile>();

        //    //Init
        //    var currentCalcs = new Dictionary<LBSTile, List<Candidate>>();
        //    foreach (var tile in toCalc)
        //    {
        //        Debug.Log("tile:" + tile.Position);
        //        // Get candidates related to current tile
        //        var candidates = CalcCandidates(tile, cGroup);
        //        currentCalcs.Add(tile, candidates);
        //    }

        //    // Run as long as you have tiles 
        //    while (toCalc.Count > 0)
        //    {
        //        var _closed = new List<LBSTile>(closed);

        //        // end condition
        //        var xx = currentCalcs.Where(e => e.Value.Count > 1).ToList();
        //        if (xx.Count <= 0)
        //            break;

        //        // Get tile with lees possibilities
        //        var current = xx.OrderBy(e => e.Value.Count).First();

        //        // cheack if curren tile have tile possibilities
        //        if (current.Value.Count <= 0)
        //        {
        //            // Remove from the list of tiles to calculate 
        //            Debug.Log(current.Key.Position + " no tiene posibles tile.");
        //            toCalc.Remove(current.Key);
        //            continue;
        //        }

        //        // Collapse possibilities
        //        var selected = current.Value.RandomRullete(c => c.weigth);
        //        var connections = selected.bundle.GetConnection(selected.rotation);
        //        connected.SetConnections(current.Key, connections.ToList(), new List<bool>() { false, false, false, false });
        //        currentCalcs[current.Key] = new List<Candidate>() { selected };

        //        // Ignore This tiles
        //        closed.Add(current.Key);

        //        // Collapse neighbours connection 
        //        var neigth = map.GetTileNeighbors(current.Key, Dirs);
        //        OLDSetConnectionNei(current.Key, neigth.ToArray(), closed);

        //        // Add to reCalc list
        //        var neigthCalcs = neigth.RemoveEmpties().Where(n => currentCalcs.Any(c => c.Key == n)).ToList();
        //        reCalc.AddRange(neigthCalcs);

        //        while (reCalc.Count > 0)
        //        {
        //            var tile = reCalc.First();

        //            // Get candidates related to current tile
        //            List<Candidate> lastCandidates;
        //            currentCalcs.TryGetValue(tile, out lastCandidates);
        //            var newCandidates = CalcCandidates(tile, cGroup);

        //            if (lastCandidates == null || newCandidates.Count < lastCandidates.Count)
        //            {
        //                currentCalcs[tile] = newCandidates;

        //                // Get neighbours
        //                var neigs = map.GetTileNeighbors(tile, Dirs).RemoveEmpties();

        //                // Add to reCalc list
        //                foreach (var nei in neigs)
        //                {
        //                    // Check if tile is closed
        //                    if (_closed.Contains(nei))
        //                        continue;

        //                    if (reCalc.Contains(nei))
        //                        continue;

        //                    reCalc.Add(nei);
        //                }
        //            }
        //            reCalc.Remove(tile);
        //            _closed.Add(tile);
        //        }

        //        // Remove from the list of tiles to calculate 
        //        toCalc.Remove(current.Key);
        //    }
        //}

        //public void OLDSetConnectionNei(LBSTile origin, LBSTile[] neis, List<LBSTile> closed)
        //{
        //    var connected = OwnerLayer.GetModule<ConnectedTileMapModule>();

        //    var dirs = Directions.Bidimencional.Edges;

        //    var oring = connected.GetConnections(origin);

        //    for (int i = 0; i < neis.Length; i++)
        //    {
        //        if (neis[i] == null)
        //            continue;

        //        if (closed.Contains(neis[i]))
        //            continue;

        //        var idir = dirs.FindIndex(d => d.Equals(-dirs[i]));

        //        connected.SetConnection(neis[i], idir, oring[i], false);
        //    }
        //}

        #endregion
    }

    public class TileChance
    {
        public TileConnectionsPair tile;
        public int count;

        public int direction;
        //0 => "Right",
        //1 => "Up",
        //2 => "Left",
        //3 => "Down",

        public TileChance(TileConnectionsPair tile, int count, int direction)
        {
            this.tile = tile;
            this.count = count;
            this.direction = direction;
        }

        public TileChance(TileConnectionsPair tile, int direction)
        {
            this.tile = tile;
            this.direction = direction;
            count = 1;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileChance;

            if (other == null) return false;

            if (!other.tile.Connections.SequenceEqual(tile.Connections)) return false;
            if (other.direction != direction) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class TileConnections
    {
        public List<string> connections;

        public TileConnections(List<string> strings)
        {
            connections = strings;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (obj is not TileConnections other)
            {
                return false;
            }

            if (connections.SequenceEqual(other.connections))
            {
                return true;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return connections.GetEnumerator();
        }
    }

    public class Candidate : ICloneable
    {
        public float weigth;
        public LBSDirection bundle;
        public int rotation;

        public Candidate() { }

        public Candidate(float weigth, LBSDirection bundle, int rotation)
        {
            this.weigth = weigth;
            this.bundle = bundle;
            this.rotation = rotation;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Candidate;

            if (other == null) return false;

            return
                weigth == other.weigth &&
                bundle.Equals(other.bundle) &&
                rotation == other.rotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(weigth, bundle.GetHashCode(), rotation);
        }

        public override string ToString()
        {
            return bundle.Owner.Name;
        }

        public object Clone()
        {
            return new Candidate(weigth, bundle, rotation);
        }
    }

    public class ChanceCandidate : ICloneable
    {
        public float weigth;
        public LBSDirection bundle;
        public int rotation;

        public ChanceCandidate() { }

        public ChanceCandidate(float weigth, LBSDirection bundle, int rotation)
        {
            this.weigth = weigth;
            this.bundle = bundle;
            this.rotation = rotation;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ChanceCandidate;

            if (other == null) return false;

            return
                weigth == other.weigth &&
                bundle.Equals(other.bundle) &&
                rotation == other.rotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(weigth, bundle.GetHashCode(), rotation);
        }

        public override string ToString()
        {
            return bundle.Owner.Name;
        }

        public object Clone()
        {
            return new ChanceCandidate(weigth, bundle, rotation);
        }
    }

    class WFCState
    {
        public int step;
        public ConnectedTileMapModule tileMap;
        public List<LBSTile> toCalc;
        public List<LBSTile> closed = new List<LBSTile>();
        public Dictionary<LBSTile, List<Candidate>> currentCalcs = new Dictionary<LBSTile, List<Candidate>>();

        public WFCState(int step, ConnectedTileMapModule tileMap, List<LBSTile> toCalc, List<LBSTile> closed, Dictionary<LBSTile, List<Candidate>> currentCalcs)
        {
            this.step = step;
            var tm = new List<LBSModule>() { tileMap };
            this.tileMap = tm.Clone()[0] as ConnectedTileMapModule;
            this.toCalc = toCalc.Clone();
            this.closed = closed.Clone();
            //this.currentCalcs = currentCalcs.Clone();
            this.currentCalcs = new Dictionary<LBSTile, List<Candidate>>(currentCalcs);
        }
    }
}