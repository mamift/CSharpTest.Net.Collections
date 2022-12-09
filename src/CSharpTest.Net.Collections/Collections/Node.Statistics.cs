#region Copyright 2011-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion
using System;
using System.Collections.Generic;

namespace CSharpTest.Net.Collections
{
    partial class BPlusTree<TKey, TValue>
    {
        private bool Rank(NodePin nodePin, TKey key, out int rank)
        {
            rank = 0;
            try
            {
                if (nodePin != null)
                {
                    Node me = nodePin.Ptr;
                    bool isValueNode = me.IsLeaf;

                    int ordinal;
                    var keyInRange = me.BinarySearch(_itemComparer, new Element(key), out ordinal);

                    if (isValueNode)
                    {
                        rank += ordinal;
                    }
                    else
                    {
                        for (var i = 0; i <= ordinal; i++)
                        {
                            var nextPin = _storage.Lock(nodePin, me[i].ChildNode);
                            nodePin.Dispose();
                            int innerRank;
                            keyInRange = Rank(nextPin, key, out innerRank);
                            rank += innerRank;
                            if (keyInRange) break;
                        }
                    }

                    return keyInRange;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (nodePin != null) nodePin.Dispose();
            }
        }

        private bool Select(NodePin nodePin, int rank, out TKey key, out int offset)
        {
            key = default(TKey);
            offset = 0;
            try
            {
                if (nodePin != null)
                {
                    Node me = nodePin.Ptr;
                    bool isValueNode = me.IsLeaf;

                    bool found = false;

                    if (isValueNode)
                    {
                        while (offset < me.Count)
                        {
                            key = me[offset].Key;

							if (offset == rank)
                            {
                                found = true;
                                break;
                            }

                            offset++;
                        }
                    }
                    else
                    {
                        for (var i = 0; i <= me.Count; i++)
                        {
							if (me[i].IsEmpty)
                            {
                                continue;
                            }

                            var nextPin = _storage.Lock(nodePin, me[i].ChildNode);
                            nodePin.Dispose();
                            int innerOffset;
                            TKey innerFoundKey;
                            found = Select(nextPin, rank - offset, out innerFoundKey, out innerOffset);
                            offset += innerOffset;
							
                            if (found)
                            {
                                key = innerFoundKey;
                                break;
                            }
                        }
                    }

                    return found;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                if (nodePin != null) nodePin.Dispose();
            }
        }
    }
}
