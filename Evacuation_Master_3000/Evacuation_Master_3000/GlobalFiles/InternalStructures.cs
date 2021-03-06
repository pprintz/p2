﻿using System.Collections.Generic;
using System.Windows;

namespace Evacuation_Master_3000
{
    public delegate void PersonEvacuated(Person person);
    public delegate IEnumerable<BuildingBlock> ExtendedPathRequest(Person person);
    public delegate void PersonMoved(Person person);
    public delegate void FunctionDone(object sender, RoutedEventArgs e);
    public delegate void ResetClicked();
    public delegate Dictionary<int, Person> PrepareSimulation(IFloorPlan floorPlan);
    public delegate Dictionary<int, Person> UISimulationStart(bool heatmap, IPathfinding pathfindingAlgorithm, int tickLength);
    public delegate IFloorPlan ImportFloorPlan(string fileName);
    public delegate IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople);
    public delegate IFloorPlan NewFloorPlan(int width, int height, int floorAmount, string description);
    public delegate void Tick();
    public delegate void ChangeVisualFloor(int currentFloor);
    public delegate void ExportFloorPlanFeedBack(string message, Export.ExportOutcomes outcome);
    public delegate void ImportFloorPlanFeedBack();
    public delegate void BuildingPlanSuccessfullLoaded();
}
