using System;

namespace FalconNet.Campaign
{
// A-Search is an A* Search algorythm which can be used to find the best-cost
// path between two nodes in a directed graph or a grid map (specialized directed
// graph, essentially.
// A-Search is an A* Search algorythm which can be used to find the best-cost
// path between two nodes in a directed graph or a grid map (specialized directed
// graph, essentially.

// User's extend function should take a location and find the following
// information on that location's neighbors (and call ASFillNode)
// * cost to move from location to neighbor;
// * estimated cost to go (lower bound);
// * the direction taken to get to the neighbor;
// * a pointer to either the coordinate or objective we're dealing with
//   or zero if no such neighbor exists.

// ==========================================
// A* classes
// ==========================================
// TODO typedef float costtype;

// ==========
// Path Class
// ==========
	public class BasePathClass
	{
	
		protected float	cost;					// Cost of full path
		protected byte		length;					// Length of the path
		protected byte		max_length;				// Longest this path can get
		protected byte		current_location;
		protected byte[]		path;					// The actual path bits

	
		public BasePathClass ()
		{
			max_length = ASData.MAX_DISTANCE;
			length = current_location = 0;
			cost = 0.0F;
		}

		public BasePathClass (byte[] stream)
		{
			throw new NotImplementedException ();
		}
 
		// TODO public ~BasePathClass ();
		public virtual int Save (byte[] stream)
		{
			throw new NotImplementedException ();
		}

		public virtual int SaveSize ()
		{
			throw new NotImplementedException ();
		}

		public int GetNextDirection ()
		{
			if (current_location >= length)
				return ASData.MAX_NEIGHBORS;
			int i = current_location / ASData.PATH_DIV;
			int o = current_location % ASData.PATH_DIV;
			return (path [i] >> (o * ASData.PATH_BITS)) & ASData.PATH_MASK;
		}
		
		// Get the next direction in the path
		public int GetPreviousDirection (int num)
		{
			int l = current_location - num;
			if (l < 0)
				return ASData.MAX_NEIGHBORS;
			int i = l / ASData.PATH_DIV;
			int o = l % ASData.PATH_DIV;
			return (path [i] >> (o * ASData.PATH_BITS)) & ASData.PATH_MASK;
		}
		
		// Get the numth direction before current direction
		public int GetDirection (int num)
		{
			if (num >= length)
				return ASData.MAX_NEIGHBORS;
			int i = num / ASData.PATH_DIV;
			int o = num % ASData.PATH_DIV;
			return (path [i] >> (o * ASData.PATH_BITS)) & ASData.PATH_MASK;
		}
		
		// Get the numth direction in the path
		public int GetCurrentPosition ()
		{
			return current_location;
		}

		public int GetLength ()
		{
			return length;
		}

		public int GetMaxLength ()
		{
			return max_length;
		}

		public float GetCost ()
		{
			return cost;
		}

		public void SetPathPosition (byte num)
		{
			current_location = num;
		}

		public void SetDirection (int num, int d)
		{
			if (d < 0)
				d = ASData.MAX_NEIGHBORS;
			int i = num / ASData.PATH_DIV;
			int o = num % ASData.PATH_DIV;
			byte temp = (byte)(ASData.PATH_MASK << (o * ASData.PATH_BITS));
			path [i] &= (byte)~temp;
			path [i] |= (byte)(d << (o * ASData.PATH_BITS));
		}

		public void SetNextDirection (int d)
		{
			SetDirection (current_location, d);
		}

		public void SetCost (float c)
		{
			cost = c;
		}

		public void StepPath ()
		{
			current_location++;
		}
		
		// Move to the next direction
		public int AddToPath (int d, float c)
		{
			if (length == max_length)
				return 0;
		
			SetDirection (length, d);
			cost = c;
			length++;
			return length;
		}

		public void ClearPath ()
		{
			length = 0;
		}

		public int CopyPath (BasePathClass from_path)
		{
			cost = from_path.cost;
			current_location = from_path.current_location;
			if (from_path.length > max_length) {
				length = max_length;
				if (from_path.current_location > max_length)
					current_location = max_length;
				//TODO memcpy(pathfrom_path.path,((max_length*ASData.PATH_BITS)+7)/8);
				Array.Copy (from_path.path, path, ((max_length * ASData.PATH_BITS) + 7) / 8);
				return 0;
			} else {
				length = from_path.length;
				// TODO  memcpy(path,from_path.path,((from_path.length*ASData.PATH_BITS)+7)/8);
				Array.Copy (from_path.path, path, ((from_path.length * ASData.PATH_BITS) + 7) / 8);
				return 1;
			}
		}
		// Copy from from_path
	}

	public class PathClass : BasePathClass
	{
	
		private byte[]		path_pool = new byte[((ASData.MAX_DISTANCE * ASData.PATH_BITS) + 7) / 8];		// The actual path bits
	
		public PathClass ()
		{
			path = path_pool;
		}
 
		public override int SaveSize ()
		{
			return sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(byte) * (length + 1) / ASData.PATH_DIV;
		}
 
	}

	public class SmallPathClass : BasePathClass
	{
	
		private byte[]	path_pool = new byte[((ASData.SMALL_PATH_MAX_DISTANCE * ASData.PATH_BITS) + 7) / 8];		// The actual path bits
	
		public SmallPathClass ()
		{
			max_length = ASData.SMALL_PATH_MAX_DISTANCE;
			path = path_pool;
			length = current_location = 0;
			cost = 0.0F;
		}

		public override int SaveSize ()
		{
			return sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(byte) * (length + 1) / ASData.PATH_DIV;
		}
 
	}

	// TODO typedef BasePathClass* Path;

	// =============
	// AS_Node Class
	// =============
	
	public class ASNode // AS_NodeClass
	{
		public ASNode	next;						// used by queue
		public float		cost;						// How much it cost to get here
		public float		to_go;						// Minimum guess as to how much further
		public object		where_;						// Coordinate or Object we're at
		public PathClass		path;						// How we got there

		//		AS_NodeClass ();
//		~AS_NodeClass ();
	}
	
	// TODO typedef AS_NodeClass* ASNode;

	// =============
	// AS_Data Class
	// =============

	public class ASData //AS_DataClass* ASData
	{
		// ======================================
		// The one and only ASearch class
		// ======================================
		public static 	ASData ASD;
		// ==========================================
// ASearch flags
// ==========================================

		public const int RETURN_EMPTY_ON_FAIL = 0x00;	// Return an empty path if search failed
		public const int RETURN_PARTIAL_ON_FAIL = 0x01;	// Return a partial path if we failed to complete before timeout
		public const int RETURN_PARTIAL_ON_MAX = 0x02;	// Return a partial path if we exceed our path length

		// ==========================================
		// A* Data types and defines
		// ==========================================

		public const int MAX_SEARCH = 2000;				// How many locations were willing to scan
		public const int MAX_DISTANCE = 96	;				// Longest path we'll look for
		public const int MAX_NEIGHBORS = 8;					// How many neighbors we can have
		public const int PATH_BITS = 4;					// How many bits needed per direction (0-8 requires 4 bits)
		// 8 should be divisible by this (i.e: 2,4 or 8)
		public const int PATH_MASK = 0xF	;				// binary number, PATH_BITS wide, all 1s.

		public const int PATH_ARRAY = ((MAX_DISTANCE * PATH_BITS) + 7) / 8;		// # of array entries
		public const int PATH_DIV = (8 / PATH_BITS);
		public const int SMALL_PATH_MAX_DISTANCE = 8;		// Maximum length of our special smaller version of the path class

		private ASNode		queue;						// The queue of locations to search
		private ASNode		tried;						// The list of nodes visited
		private ASNode		waste;						// Unused nodes
		private ASNode		location;					// Where we are right now
		private ASNode[]	node_ptr;					// Array of all our nodes
		private ASNode[]	neighbors = new ASNode [MAX_NEIGHBORS];	// The list of current neighbors
#if AS_THREAD_SAFE
		F4CSECTIONHANDLE* cs;						// Critical section for this search class
#endif
		public ASData ()
		{
			int i;
	
			queue = null;
			tried = null;
	#if AS_THREAD_SAFE
			cs = null;
			cs = F4CreateCriticalSection ("AS_CS");
	#endif
			// Allocate a big chunk of nodes
			node_ptr = new ASNode[MAX_SEARCH];
			// We want our first node to be the current location
			location = node_ptr [0];
			location.cost = 0.0F;
			location.to_go = 0.0F;
			location.where_ = 0;
			location.next = null;
			// Build the waste list from the remaining chunks
			for (i=1; i<MAX_SEARCH-1; i++)
				node_ptr [i].next = node_ptr [i + 1];
			node_ptr [MAX_SEARCH - 1].next = null;
			waste = node_ptr [1];
		}
		//TODO public ~AS_DataClass ();
		
		public delegate void extend (ASData asd,object o,object t);
		
		// This is the main search routine. It must be passed an extension funtion
		// This is the key function
		// returns:		 1 = Successfull search
		//				 0 = Found partial path
		//				-1 = Unable to find a path
		public int ASSearch (BasePathClass p, object origin, object target, extend ex, int flags, int maxSearch, float maxCost)
		{
			int count = 0, retval = -1, max_length;
			ASNode T;
			float best;

			if (origin == target)
				return 1;
			if (target == null)
				return -1;

#if AS_THREAD_SAFE
	F4EnterCriticalSection(cs);
#endif
			AS_attach_queues ();
			location.where_ = origin;
			location.cost = 0.0F;
			location.next = null;
			location.path.ClearPath ();
			p.ClearPath ();
			max_length = p.GetMaxLength ();
			while (count < maxSearch && waste != null) {
				ex (this, location.where_, target);
				AS_merge (count);
				if (queue != null) {
					T = queue;
					queue = T.next;
					T.next = null;
					location.next = tried;
					tried = location;
					location = T;
				} else {
					// No possible moves
#if AS_THREAD_SAFE
			F4LeaveCriticalSection(cs);
#endif
					return retval;
				}

				// Check for solution
				if (location.where_ == target) {
					// We're done - Found full path
					p.CopyPath (location.path);
#if AS_THREAD_SAFE
			F4LeaveCriticalSection(cs);
#endif
					return 1;
				}

				// Check for exceeding the passed path's length
				if (location.path.GetLength () >= max_length) {
					if ((flags & RETURN_PARTIAL_ON_MAX) != 0) {
						// return a partial path
						p.CopyPath (location.path);
						retval = 0;
					}

#if AS_THREAD_SAFE
			F4LeaveCriticalSection(cs);
#endif
					return retval;
				}

				// Check for exceeding max cost and abort if so
				if (maxCost != 0 && location.cost > maxCost)
					count = maxSearch;
				
				count++;
			}

			// No solution found (timed out).
			if ((flags & RETURN_PARTIAL_ON_FAIL) != 0) {
				// Return a path to the best location so far
				T = queue;
				best = 99999.0F;
				while (T != null) {
					if (T.cost > 0 && T.to_go < best) {
						best = T.to_go;
						p.CopyPath (T.path);
					}
					T = T.next;
				}
				T = tried;
				while (T != null) {
					if (T.cost > 0 && T.to_go < best) {
						best = T.to_go;
						p.CopyPath (T.path);
					}
					T = T.next;
				}
				retval = 0;
			}
#if AS_THREAD_SAFE
	F4LeaveCriticalSection(cs);
#endif
			return retval;
		}


		// This needs to be called per potential direction by the extend function
		public void ASFillNode (int node, float cost, float to_go, string dir, object where_)
		{
			neighbors [node].where_ = where_;
			if (where_ != null) {
				neighbors [node].cost = cost + location.cost;
				neighbors [node].to_go = to_go;
			}
		}
	
		private void AS_dispose_queue (ASNode N)
		{
			ASNode T;
		
			while (N != null) {
				T = N.next;
				//delete N;
				N = T;
			}
		}

		private void AS_attach_queues ()
		{
			ASNode N;
		
			if (queue != null) {
				N = queue;
				while (N.next != null)
					N = N.next;
				N.next = waste;
				waste = queue;
				queue = null;
			}
			if (tried != null) {
				N = tried;
				while (N.next != null)
					N = N.next;
				N.next = waste;
				waste = tried;
				tried = null;
			}
		}

		private void AS_reattach (ASNode n)
		{
			n.next = waste;
			waste = n;
		}

		private void AS_merge (int count)
		{
			int n;
			ASNode new_node = null, T, insert_after;

			for (n=0; n<MAX_NEIGHBORS; n++) {
				if (neighbors [n].where_ == null)
					continue;
				if (queue == null || neighbors [n].cost + neighbors [n].to_go < queue.cost + queue.to_go) {
					new_node = AS_get_new_node (n);
					if (new_node == null)
						continue;
					new_node.next = queue;
					queue = new_node;
					continue;
				}
				insert_after = null;
				T = queue;
				while (T.next != null) {
					if (neighbors [n].where_ == T.next.where_) {
						if (insert_after != null) {
							// We've got a better version than this one, move this queue entry to waste
							ASNode temp = T.next;
							T.next = temp.next;
							AS_reattach (temp);
						}
						// Either way, quit searching
						break;
					} else if (insert_after == null && neighbors [n].cost + neighbors [n].to_go < T.next.cost + T.next.to_go)
						insert_after = T;
					T = T.next;
				}
				if (T != null && insert_after == null && T.next == null)
			// Insert at end of the queue
					insert_after = T;		
				if (insert_after != null) {
					new_node = AS_get_new_node (n);
					if (new_node == null)
						continue;
					new_node.next = insert_after.next;
					insert_after.next = new_node;
				}
			}
		}

		private ASNode AS_get_new_node (int n)
		{
			ASNode new_node, T;

			// Ignore this neighbor if already tried
			T = tried;
			while (T != null) {
				// KCK: The following line is only usefull if the user's huristic overestimates -
				// And even then, it only gets slightly better answers for a moderate cost.
//		if (T.where == neighbors[n].where && neighbors[n].cost+neighbors[n].to_go > T.cost + T.to_go)
				if (T.where_ == neighbors [n].where_)
					return null;
				T = T.next;
			}

			if (waste != null) {
				new_node = waste;
				waste = waste.next;
			} else
				return null;			// We're out of waste nodes. We'll quit looking.
			new_node.cost = neighbors [n].cost;
			new_node.to_go = neighbors [n].to_go;
			new_node.where_ = neighbors [n].where_;
			new_node.next = null;
			new_node.path.CopyPath (location.path);
			if (new_node.path.AddToPath (n, neighbors [n].cost) != 0)
				return new_node;
			else {
				AS_reattach (new_node);
				return null;
			}
		}

	}
	// TODO typedef AS_DataClass* ASData;


}

