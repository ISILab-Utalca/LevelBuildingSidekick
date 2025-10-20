using System;
using System.Threading;
using System.Threading.Tasks;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.Editor.Windows;

using LBS;
using LBS.Components;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("HillClimbingAssistant", typeof(HillClimbingAssistant))]
    public class HillClimbingAssistantEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private readonly UnityEngine.Color AssistantColor = LBSSettings.Instance.view.assistantColor;

        private HillClimbingAssistant hillClimbing;

        private Foldout foldout;
        private Button revert;
        private Button execute;
        private Button executeOne;
        
        private LBSCustomToggleField toggle;
        private LBSCustomToggleField benchmarkToggle;
        private Toggle toggleTimer;

        private Button recalculate;

        private LBSLayer tempLayer;

        // Manipulators
        private SetZoneConnection setZoneConnection;
        private RemoveZoneConnection removeZoneConnection;

        private CancellationTokenSource _currentTaskCts;
        #endregion
        
        public HillClimbingAssistantEditor(object target) : base(target)
        {
            hillClimbing = target as HillClimbingAssistant;

            CreateVisualElement();

            var window = EditorWindow.GetWindow<LBSMainWindow>();

            // NO longer redrawing the whole window we aint psychopaths!
          //  hillClimbing.OnTermination += window.Repaint;
        }

        public override void Repaint()
        {
            var moduleConstr = hillClimbing.OwnerLayer.GetModule<ConstrainsZonesModule>();
            foldout.Clear();
            foreach (var constraint in moduleConstr.Constraints)
            {
                var view = new ConstraintView();
                view.SetData(constraint);
                foldout.Add(view);
            }
        }

        public override void SetInfo(object paramTarget)
        {
            hillClimbing = paramTarget as HillClimbingAssistant;
        }

        public void SetTools(ToolKit toolKit)
        {
            setZoneConnection = new SetZoneConnection();
            var t1 = new LBSTool(setZoneConnection);
            t1.OnSelect += LBSInspectorPanel.ActivateAssistantTab;
            t1.Init(hillClimbing.OwnerLayer, hillClimbing);
            toolKit.ActivateTool(t1,hillClimbing.OwnerLayer, hillClimbing);
            
            removeZoneConnection = new RemoveZoneConnection();
            var t2 = new LBSTool(removeZoneConnection);
            t2.OnSelect += LBSInspectorPanel.ActivateAssistantTab;
            toolKit.ActivateTool(t2,hillClimbing.OwnerLayer, hillClimbing);
            
            setZoneConnection.SetRemover(removeZoneConnection);
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("HillClimbingEditor");
            visualTree.CloneTree(this);

            var moduleConstr = hillClimbing.OwnerLayer.GetModule<ConstrainsZonesModule>();

            // Foldout
            foldout = this.Q<LBSCustomFoldout>();
            foreach (var constraint in moduleConstr.Constraints)
            {
                var view = new ConstraintView();
                view.SetData(constraint);
                foldout.Add(view);
            }

            // Print Timers
            toggleTimer = this.Q<Toggle>("ShowTimerToggle");
            toggleTimer.RegisterCallback<ChangeEvent<bool>>(x =>
            {
                hillClimbing.printClocks = x.newValue;
            });

            // Execute
            execute = this.Q<Button>("Execute");
            execute.clicked += Execute;

            // Execute 1 step
            executeOne = this.Q<Button>("ExecuteOneStep");
            executeOne.clicked += ExecuteOneStep;

            // Show Constraint
            toggle = this.Q<LBSCustomToggleField>("ShowConstraintToggle");
            toggle.value = hillClimbing.visibleConstraints;
            toggle.RegisterCallback<ChangeEvent<bool>>(x =>
            {
                hillClimbing.visibleConstraints = x.newValue;
                DrawManager.ReDraw();
            });
            
            benchmarkToggle = this.Q<LBSCustomToggleField>("UseBenchmark");

            recalculate = new Button
            {
                text = "Recalculate Constraints"
            };
            recalculate.clicked += ClickedRecalculate;

            Add(recalculate);

            return this;
        }

        void CancelCurrentTask()
        {
            if(_currentTaskCts == null) return;
            if(_currentTaskCts.IsCancellationRequested) return;
            _currentTaskCts.Cancel();
        }
        
        private void ClickedRecalculate()
        {
            // Save history version to revert if necessary
            var x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Recalculate Constraints");
            EditorGUI.BeginChangeCheck();

            // Recalculate constraints
            RunRecalculateTask(x);
        }

        private void RunRecalculateTask(LoadedLevel level)
        {
            _currentTaskCts?.Cancel();

            _currentTaskCts = new CancellationTokenSource();
            var token = _currentTaskCts.Token;

            var taskbar = LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>();
            
            taskbar.OnProgressCancelled -= CancelCurrentTask;
            taskbar.OnProgressCancelled += CancelCurrentTask;

            void ReportProgress(float normalized)
            {
                // Use update so progress applies immediately
                EditorApplication.update += UpdateOnce;

                void UpdateOnce()
                {
                    taskbar.SetProgressPercent(normalized);
                    EditorApplication.update -= UpdateOnce;
                }
            }
            
            taskbar.EnableProcess(true, hillClimbing.Name);
            Task.Run(() =>
            {
                try
                {
                    hillClimbing.RecalculateConstraint(ReportProgress, token);

                    EditorApplication.delayCall += () =>
                    {
                        LBSMainWindow.MessageNotify("Zones constraints recalculated.");

                        // Mark as dirty
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(level);
                        }

                        DrawManager.Instance.RedrawLayer(hillClimbing.OwnerLayer);
                        Paint();
                        
                        taskbar.EnableProcess(false);
                    };

                }
                catch (Exception ex)
                {
                    Debug.LogError($"[HillClimbingAssistant] Task failed: {ex}");
                    EditorApplication.delayCall += () => taskbar.EnableProcess(false);
                }
            }, token);
        }

        private void Paint()
        {
            Clear();
            CreateVisualElement();
        }

        private void ExecuteOneStep()
        {
            // Save history version to revert if necessary
            var x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Execute One Step");
            EditorGUI.BeginChangeCheck();

            _currentTaskCts?.Cancel();

            _currentTaskCts = new CancellationTokenSource();
            var token = _currentTaskCts.Token;

            var taskbar = LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>();
            ;

            taskbar.OnProgressCancelled -= CancelCurrentTask;
            taskbar.OnProgressCancelled += CancelCurrentTask;

            void ReportProgress(float normalized)
            {
                // Use update so progress applies immediately
                EditorApplication.update += UpdateOnce;

                void UpdateOnce()
                {
                    taskbar.SetProgressPercent(normalized);
                    EditorApplication.update -= UpdateOnce;
                }
            }

            taskbar.EnableProcess(true, hillClimbing.Name);
            Task.Run(() =>
            {
                try
                {
                    // Execute hill climbing one step
                    hillClimbing.ExecuteOneStep(ReportProgress, token);
                    
                    EditorApplication.delayCall += () =>
                    {
                        hillClimbing.ExecutionEnded();
                        LBSMainWindow.MessageNotify("Hill Climbing One Step executed.");
                        
                        // Mark as dirty
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(x);
                        }
                        DrawManager.Instance.RedrawLayer(hillClimbing.OwnerLayer);
                        Paint();
                        taskbar.EnableProcess(false);
                    };
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[HillClimbingAssistant] Task failed: {ex}");
                    EditorApplication.delayCall += () => taskbar.EnableProcess(false);
                }
            }, token);
        }

        private void Execute()
        {
            // Save history version to revert if necessary
            LoadedLevel x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Execute HillClimbing");
            EditorGUI.BeginChangeCheck();

            _currentTaskCts?.Cancel();

            _currentTaskCts = new CancellationTokenSource();
            var token = _currentTaskCts.Token;

            var taskbar =LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>();;
            
            taskbar.OnProgressCancelled -= CancelCurrentTask;
            taskbar.OnProgressCancelled += CancelCurrentTask;
            
            void ReportProgress(float normalized)
            {
                // Use update so progress applies immediately
                EditorApplication.update += UpdateOnce;
                void UpdateOnce()
                {
                    taskbar.SetProgressPercent(normalized);
                    EditorApplication.update -= UpdateOnce;
                }
            }
            taskbar.EnableProcess(true, hillClimbing.Name);
            Task.Run(() =>
            {
                try
                {
                    bool valid = hillClimbing.TryExecute(out string failedLog, ReportProgress, token);
                    EditorApplication.delayCall += () =>
                    {
                        if (valid)
                        {
                            hillClimbing.ExecutionEnded();
                            LBSMainWindow.MessageNotify("Hill Climbing executed.");
                        }
                        else
                        {
                            LBSMainWindow.MessageNotify(failedLog, LogType.Warning, 5);
                        }
                        
                        // Mark as dirty
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(x);
                        }
                        
                        DrawManager.Instance.RedrawLayer(hillClimbing.OwnerLayer);
                        Paint();
                        taskbar.EnableProcess(false);
                    };

                }
                catch (Exception ex)
                {
                    Debug.LogError($"[HillClimbingAssistant] Task failed: {ex}");
                    EditorApplication.delayCall += () => taskbar.EnableProcess(false);
                }
            }, token);
        }
    }
}