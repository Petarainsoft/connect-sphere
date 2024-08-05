using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AhnLab.EventSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ConnectSphere
{
    public class DistanceScanner : PeerScanner
    {
        [SerializeField] private float _checkingInterval = 0.2f;
        [SerializeField] private float _minimalDistance = 1f;

        // [SerializeField] private Slider _method1Time;
        // [SerializeField] private Slider _method2Time;

        private Dictionary<int, PositionedPeer> currentPositions;

        protected override void Awake()
        {
            base.Awake();
            currentPositions = new Dictionary<int, PositionedPeer>();
            AEventHandler.RegisterEvent<int, Vector2>(GlobalEvents.PositionUpdated, HandlePositionUpdated);


            ProcessPeersInfo();
        }

        private void HandlePositionUpdated(int userId, Vector2 position)
        {
            if ( currentPositions.ContainsKey(userId) )
            {
                currentPositions[userId]._position = position;
            }
            else
            {
                currentPositions.Add(userId, new PositionedPeer()
                {
                    _userId = userId,
                    _position = position
                });
            }
        }

        private readonly Stopwatch stopwatch = new Stopwatch();

        // private KDTree kdTree = null;
        private async UniTaskVoid ProcessPeersInfo()
        {
            while (true)
            {
                // // algorithm 1
                // Debug.Log("<color=red>METHOD 1</color>");;
                //
                // var listPos = currentPositions.Values.Select(e=>e._position).ToList();
                // kdTree = new KDTree(listPos);
                // stopwatch.Reset();
                // stopwatch.Start();
                // List<(Vector2, Vector2)> pairsWithinDistance = kdTree.FindPairsWithinDistance(_minimalDistance);
                // stopwatch.Stop();
                //
                // foreach (var pair in pairsWithinDistance)
                // {
                //     Debug.Log($"Point1: {pair.Item1}, Point2: {pair.Item2}, Distance: {Vector2.Distance(pair.Item1, pair.Item2)}");
                // }
                //
                // Debug.Log($"<color=green>\tTime to compute the result {stopwatch.Elapsed.TotalMilliseconds}</color>");
                // _method1Time.value += (float)stopwatch.Elapsed.TotalMilliseconds;
                //
                // algorithm 2
                List<(PositionedPeer, PositionedPeer)> pairsWithinDistance2 = new List<(PositionedPeer, PositionedPeer)>();
                Debug.Log("<color=red>METHOD 2</color>");;
                stopwatch.Reset();
                stopwatch.Start();

                var listPos = currentPositions.Values.ToList();

                for (int i = 0; i < currentPositions.Count - 1; i++)
                {
                    for (int j = i + 1; j < currentPositions.Count; j++)
                    {
                        var distance = Vector2.Distance(listPos[i]._position, listPos[j]._position);
                        if ( distance <= _minimalDistance )
                        {
                            pairsWithinDistance2.Add((listPos[i], listPos[j]));
                        }
                    }
                }
                
                stopwatch.Stop();
                Debug.Log($"<color=green>\tTime to compute the result {stopwatch.Elapsed.TotalMilliseconds}</color>");
                // _method2Time.value += (float)stopwatch.Elapsed.TotalMilliseconds;
                
                _orderedPeers.Clear();
                foreach (var pair in pairsWithinDistance2)
                {
                    // Debug.Log($"Point1: {pair.Item1}, Point2: {pair.Item2}, Distance: {Vector2.Distance(pair.Item1, pair.Item2)}");
                    var orderedPeersInfo = new OrderedPeersInfo(pair.Item1._userId, pair.Item2._userId);
                    orderedPeersInfo.SetPeerGroup(PeerGroup.OutOfOffice);
                    _orderedPeers.Add(orderedPeersInfo);
                }
                
                InvokePeersChanged();
                
                // if (pairsWithinDistance2.Count != pairsWithinDistance.Count) Debug.LogError("WROOONG RESULT");
                
                await UniTask.WaitForSeconds(_checkingInterval);
            }
        }

        private class PositionedPeer
        {
            public int _userId;
            public Vector2 _position;
        }
    }

    // public class KDTreeNode
    // {
    //     public Vector2 Point { get; private set; }
    //     public KDTreeNode Left { get; set; }
    //     public KDTreeNode Right { get; set; }
    //
    //     public KDTreeNode(Vector2 point)
    //     {
    //         Point = point;
    //         Left = null;
    //         Right = null;
    //     }
    // }
    //
    // public class KDTree
    // {
    //     private KDTreeNode root;
    //     private int k = 2; // Number of dimensions (2 for 2D space)
    //
    //     public KDTree(List<Vector2> points)
    //     {
    //         root = BuildKDTree(points, 1);
    //     }
    //
    //     private KDTreeNode BuildKDTree(List<Vector2> points, int depth)
    //     {
    //         if ( points.Count == 0 )
    //             return null;
    //
    //         int axis = depth%k;
    //         points.Sort((a, b) => a[axis].CompareTo(b[axis]));
    //         int medianIndex = points.Count/2;
    //
    //         KDTreeNode node = new KDTreeNode(points[medianIndex])
    //         {
    //             Left = BuildKDTree(points.GetRange(0, medianIndex), depth + 1),
    //             Right = BuildKDTree(points.GetRange(medianIndex + 1, points.Count - medianIndex - 1), depth + 1)
    //         };
    //
    //         return node;
    //     }
    //
    //     public List<(Vector2, Vector2)> FindPairsWithinDistance(float distance)
    //     {
    //         List<(Vector2, Vector2)> result = new List<(Vector2, Vector2)>();
    //         FindPairsWithinDistance(root, distance, 0, result);
    //         return result;
    //     }
    //
    //     private void FindPairsWithinDistance(KDTreeNode node, float distance, int depth,
    //         List<(Vector2, Vector2)> result)
    //     {
    //         if ( node == null )
    //             return;
    //
    //         FindPairsWithinSubtree(node, node, distance, depth, result);
    //
    //         if ( node.Left != null )
    //             FindPairsWithinDistance(node.Left, distance, depth + 1, result);
    //
    //         if ( node.Right != null )
    //             FindPairsWithinDistance(node.Right, distance, depth + 1, result);
    //     }
    //
    //     private void FindPairsWithinSubtree(KDTreeNode node, KDTreeNode target, float distance, int depth,
    //         List<(Vector2, Vector2)> result)
    //     {
    //         if ( node == null || target == null )
    //             return;
    //
    //         if ( node != target && Vector2.Distance(node.Point, target.Point) < distance )
    //             result.Add((node.Point, target.Point));
    //
    //         int axis = depth%k;
    //         if ( node.Left != null && Mathf.Abs(node.Point[axis] - target.Point[axis]) < distance )
    //             FindPairsWithinSubtree(node.Left, target, distance, depth + 1, result);
    //
    //         if ( node.Right != null && Mathf.Abs(node.Point[axis] - target.Point[axis]) < distance )
    //             FindPairsWithinSubtree(node.Right, target, distance, depth + 1, result);
    //     }
    // }
}