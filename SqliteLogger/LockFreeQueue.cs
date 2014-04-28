using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace inercya.EntityLite.SqliteProfiler
{
    internal static class SyncMethods
    {

        public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class
        {
            return
                (object)comparand ==
                (object)Interlocked.CompareExchange<T>(ref location, newValue, comparand);
        }
    }

    internal class SingleLinkNode<T>
    {
        public SingleLinkNode<T> Next;
        public T Item;
    }

    public class LockFreeQueue<T>
    {

        SingleLinkNode<T> head;
        SingleLinkNode<T> tail;

        public LockFreeQueue()
        {
            head = new SingleLinkNode<T>();
            tail = head;
        }

        public void Enqueue(T item)
        {
            SingleLinkNode<T> oldTail = null;
            SingleLinkNode<T> oldNext = null;

            // create and initialize the new node
            SingleLinkNode<T> node = new SingleLinkNode<T>();
            node.Item = item;

            // loop until we have managed to update the tail's Next link 
            // to point to our new node
            bool UpdatedNewLink = false;
            while (!UpdatedNewLink)
            {

                // make local copies of the tail and its Next link, but in 
                // getting the latter use the local copy of the tail since
                // another thread may have changed the value of tail
                oldTail = tail;
                oldNext = oldTail.Next;

                // providing that the tail field has not changed...
                if (tail == oldTail)
                {

                    // ...and its Next field is null
                    if (oldNext == null)
                    {

                        // ...try to update the tail's Next field
                        UpdatedNewLink = SyncMethods.CAS<SingleLinkNode<T>>(ref tail.Next, null, node);
                    }

                    // if the tail's Next field was non-null, another thread
                    // is in the middle of enqueuing a new node, so try and 
                    // advance the tail to point to its Next node
                    else
                    {
                        SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, oldNext);
                    }
                }
            }

            // try and update the tail field to point to our node; don't
            // worry if we can't, another thread will update it for us on
            // the next call to Enqueue()
            SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, node);
        }



        public bool Dequeue(out T result)
        {
            result = default(T);

            // loop until we manage to advance the head, removing 
            // a node (if there are no nodes to dequeue, we'll exit
            // the method instead)
            bool HaveAdvancedHead = false;
            while (!HaveAdvancedHead)
            {
                // make local copies of the head, the tail, and the head's Next 
                // reference
                SingleLinkNode<T> oldHead = head;
                SingleLinkNode<T> oldTail = tail;
                SingleLinkNode<T> oldHeadNext = oldHead.Next;


                // providing that the head field has not changed...
                if (oldHead == head)
                {

                    // ...and it is equal to the tail field
                    if (oldHead == oldTail)
                    {

                        // ...and the head's Next field is null
                        if (oldHeadNext == null)
                        {

                            // ...then there is nothing to dequeue
                            return false;
                        }

                        // if the head's Next field is non-null and head was equal to the tail
                        // then we have a lagging tail: try and update it
                        SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, oldHeadNext);
                    }

                    // otherwise the head and tail fields are different
                    else
                    {

                        // grab the item to dequeue, and then try to advance the head reference
                        result = oldHeadNext.Item;
                        HaveAdvancedHead =
                          SyncMethods.CAS<SingleLinkNode<T>>(ref head, oldHead, oldHeadNext);
                    }
                }
            }
            return true;
        }
    }

}
