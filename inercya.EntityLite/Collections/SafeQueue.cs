using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/*
 * code taken from http://www.altdevblogaday.com/2011/03/09/lock-free-queue/ 
 *
 */

namespace inercya.EntityLite.Collections
{
    ///
    /// A lock-free queue.
    ///
    ///Usage Note: thread safe, fast performance
    ///
    /// by jason swearingen, copyright novaleaf software.  all rights reserved.
    ///
    ///Released under the MSPL: http://www.opensource.org/licenses/ms-pl
    ///
    ///idea/core algorithm taken from http://www.research.ibm.com/people/m/michael/podc-1996.pdf
    /// inline comments are direct transcriptions of this paper's psudocode (thus this queue is virtually a 1:1 implementation of this awesome paper)
    ///
    ///
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "The suffix is correct")]
    public class SafeQueue<T>
	{
		//structure pointer t fptr: pointer to node t, count: unsigned integerg
		//structure node t fvalue: data type, next: pointer tg
		private class Node
		{
			public Node next;
			public T value;
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}
 
 
		//structure queue t fHead: pointer t, Tail: pointer tg
		private Node _head, _tail;
 
		public SafeQueue()
		{
			//initialize(Q: pointer to queue t)
			//node = new node() # Allocate a free node
			//node–>next.ptr = NULL # Make it the only node in the linked list
			//Q–>Head = Q–>Tail = node # Both Head and Tail point to it
			_head = _tail = new Node();
		}
 
		private volatile int _count;
 
		public bool IsEmpty
		{
			get { return _count == 0; }
		}
 
		//enqueue(Q: pointer to queue t, value: data type)
 
		public virtual void Enqueue(T value)
		{
			Enqueue(ref value);
		}
 
		public virtual void Enqueue(ref T value)
		{
 
			//E1: node = new node() # Allocate a new node from the free list
			//E2: node–>value = value # Copy enqueued value into node
			//E3: node–>next.ptr = NULL # SafeSet next pointer of node to NULL
			Node toAdd = new Node();
			toAdd.value = value;
			Node tail = null;
			Node temp;
 
			//E4: loop # Keep trying until Enqueue is done
			while (true)
			{
				//E5: tail = Q–>Tail # Read Tail.ptr and Tail.count together
				//E6: next = tail.ptr–>next # Read next ptr and count fields together
				tail = this._tail;
				Node next = tail.next;
 
				//E7: if tail == Q–>Tail # Are tail and next consistent?
				if (Node.ReferenceEquals(tail, this._tail))
				{
					//E8: if next.ptr == NULL # Was Tail pointing to the last node?
					if (next == null)
					{
						//E9: if CAS(&tail.ptr–>next, next, ) # Try to link node at the end of the linked list
						temp = Interlocked.CompareExchange(ref tail.next, toAdd, next);
						if (Node.ReferenceEquals(temp, next))
						{
							//E10: break # Enqueue is done. Exit loop
							break;
							//E11: endif
						}
 
					}//E12: else # Tail was not pointing to the last node
					else
					{
						//E13: CAS(&Q–>Tail, tail, ) # Try to swing Tail to the next node for try again
						Interlocked.CompareExchange(ref this._tail, next, tail);
						//E14: endif
					}
					//E15: endif
				}
				//E16: endloop
			}
			//E17: CAS(&Q–>Tail, tail, ) # Enqueue is done. Try to swing Tail to the inserted node
			Interlocked.CompareExchange(ref this._tail, toAdd, tail);
			Interlocked.Increment(ref _count);
 
		}
 
		//dequeue(Q: pointer to queue t, pvalue: pointer to data type): boolean
		///
		///
		///
		///
 
		/// false if unable to dequeue
		public virtual bool Dequeue(out T value)
		{
 
			//D1: loop # Keep trying until Dequeue is done
			while (true)
			{
				//D2: head = Q–>Head # Read Head
				//D3: tail = Q–>Tail # Read Tail
				//D4: next = head–>next # Read Head.ptr–>next
				Node head = this._head;
				Node tail = this._tail;
				Node next = head.next;
				Node temp;
 
				//D5: if head == Q–>Head # Are head, tail, and next consistent?
				//if (Node.ReferenceEquals(head, this._head))
				if (head == this._head)
				{
					//D6: if head.ptr == tail.ptr # Is queue empty or Tail falling behind?
					//if (Node.ReferenceEquals(head, tail))
					if (head == tail)
					{
						//D7: if next.ptr == NULL # Is queue empty?
						if (next == null)
						{
							//D8: return FALSE # Queue is empty, couldn’t dequeue
							value = default(T);
							return false;
							//D9: endif
						}
						//D10: CAS(&Q–>Tail, tail, ) # Tail is falling behind. Try to advance it
						Interlocked.CompareExchange(ref this._tail, next, tail);
						//D11: else # No need to deal with Tail
					}
					else
					{
						//# Read value before CAS, otherwise another dequeue might free the next node
						//D12: *pvalue = next.ptr–>value
						value = next.value;
						//D13: if CAS(&Q–>Head, head, ) # Try to swing Head to the next node
						temp = Interlocked.CompareExchange(ref this._head, next, head);
						//if (Node.ReferenceEquals(temp, head))
						if (temp == head)
						{
							//D14: break # Dequeue is done. Exit loop
							break;
							//D15: endif
						}
 
						//D16: endif
					}
					//D17: endif
				}
				//D18: endloop
			}
 
			Interlocked.Decrement(ref _count);
 
			//D19: free(head.ptr) # It is safe now to free the old dummy node
			//D20: return TRUE # Queue was not empty, dequeue succeeded
			return true;
 
		}
 
		public void Clear()
        {
			while (Dequeue(out T value)) ;
        }
	
	}

}
