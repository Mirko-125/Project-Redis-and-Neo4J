using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    public class LRUCache
    {
        class ListNode // lancana lista
        {
            public string key; 
            public string[] val;
            public ListNode prev, next;

            public ListNode(string k, string[] v)
            {
                key = k;
                val = v;
                prev = null;
                next = null;
            }
        }

        private int capacity, size;
        private ListNode dummyHead, dummyTail;

        private Dictionary<string, ListNode> map;

        public bool argumentOk = true;

        public LRUCache(int capacity) // konstruktor
        {
            if (capacity <= 0)
            {
                argumentOk = false;
                return;
            }

            this.capacity = capacity;
            size = 0;
            dummyHead = new ListNode(null, null);
            dummyTail = new ListNode(null, null);
            dummyTail.prev = dummyHead;
            dummyHead.next = dummyTail;
            map = new Dictionary<string, ListNode>();
        }
        public bool checkget(string key) 
        {
            return map.ContainsKey(key);
        }
        public string[] get(string key)
        {
            ListNode target = map[key];
            remove(target);
            addToLast(target);
            return target.val;
        }

        public void set(string key, string[] value)
        {
            if (map.ContainsKey(key))
            { // update old value of the key
                ListNode target = map[key];
                target.val = value;
                remove(target);
                addToLast(target);
            }
            else
            { // insert new key value pair, need to check current size
                if (size == capacity)
                {
                    map.Remove(dummyHead.next.key);
                    remove(dummyHead.next);
                    --size;
                }

                ListNode newNode = new ListNode(key, value);
                map.Add(key, newNode);
                addToLast(newNode);
                ++size;
            }
        }

        private void addToLast(ListNode target)
        {
            target.next = dummyTail;
            target.prev = dummyTail.prev;
            dummyTail.prev.next = target;
            dummyTail.prev = target;
        }

        private void remove(ListNode target)
        {
            target.next.prev = target.prev;
            target.prev.next = target.next;
            Console.WriteLine($"Removed {target.key} from cache because it was the least recently used.");
        }
    }
}
