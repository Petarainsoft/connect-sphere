using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class PeerScannerTest : PeerScanner
    {
        public int min = 1;
        public int max = 8;

        private IEnumerator Start()
        {
            while (true)
            {
                for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
                {
                    var firstUserId = UnityEngine.Random.Range(min, 3);
                    var secondUserId = UnityEngine.Random.Range(min, max);
                    while (secondUserId == firstUserId)
                    {
                        secondUserId = UnityEngine.Random.Range(min, max);
                        yield return null;
                    }

                    yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
                    AddPeers(new OrderedPeersInfo(firstUserId, secondUserId));
                }


                for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
                {
                    var firstUserId = UnityEngine.Random.Range(min, 4);
                    var secondUserId = UnityEngine.Random.Range(min, max);
                    while (secondUserId == firstUserId)
                    {
                        secondUserId = UnityEngine.Random.Range(min, max);
                        yield return null;
                    }

                    yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
                    RemovePeers(new OrderedPeersInfo(firstUserId, secondUserId));
                }


                for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
                {
                    var firstUserId = UnityEngine.Random.Range(min, 6);
                    var secondUserId = UnityEngine.Random.Range(min, max);
                    while (secondUserId == firstUserId)
                    {
                        secondUserId = UnityEngine.Random.Range(min, max);
                        yield return null;
                    }

                    yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
                    AddPeers(new OrderedPeersInfo(firstUserId, secondUserId));
                }


                for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
                {
                    var firstUserId = UnityEngine.Random.Range(min, 4);
                    var secondUserId = UnityEngine.Random.Range(min, max);
                    while (secondUserId == firstUserId)
                    {
                        secondUserId = UnityEngine.Random.Range(min, max);
                        yield return null;
                    }

                    yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
                    RemovePeers(new OrderedPeersInfo(firstUserId, secondUserId));
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 2f));
            }
        }
    }
}