using FalconNet.VU;

namespace Acmi
{
    public class ACMI_HASHNODE
    {
        public VU_ID ID;
        public int Index;
        public string label;//[16];
        public int color;
        public ACMI_HASHNODE Next;
    }

    public class ACMI_HASHROOT
    {
        public ACMI_HASHNODE Root_;
    }

    public class ACMI_Hash
    {
        private int ID_;
        private int TableSize_;
        private ACMI_HASHROOT[] Table_;



        public ACMI_Hash()
        {
            TableSize_ = 0;
            ID_ = 1;
            Table_ = null;
        }

        //public ~ACMI_Hash();

        public void Setup(int Size)
        {
            TableSize_ = Size;

            Table_ = new ACMI_HASHROOT[TableSize_];

            for (int i = 0; i < TableSize_; i++)
                Table_[i].Root_ = null;
        }

        public void Cleanup()
        {
            long i;
            ACMI_HASHNODE cur, prev;

            if (TableSize_ != 0 || Table_ != null)
            {
                for (i = 0; i < TableSize_; i++)
                {
                    cur = Table_[i].Root_;

                    while (cur != null)
                    {
                        prev = cur;
                        cur = cur.Next;
                        //delete prev;
                    }
                }

                // delete Table_;
                Table_ = null;
                TableSize_ = 0;
            }
        }

        public ACMI_HASHNODE Get(VU_ID ID)
        {
            long idx;
            ACMI_HASHNODE cur;

            if (TableSize_ == 0 || Table_ == null) return null;

            idx = ((uint)ID.creator_ | ID.num_) % TableSize_;
            cur = Table_[idx].Root_;

            while (cur != null)
            {
                if (cur.ID == ID)
                {
                    return (cur);
                }

                cur = cur.Next;
            }

            return null;
        }

        public long Find(VU_ID ID)
        {
            long idx;
            ACMI_HASHNODE cur;

            if (TableSize_ == 0 || Table_ == null) return 0;

            idx = ((uint)ID.creator_ | ID.num_) % TableSize_;
            cur = Table_[idx].Root_;

            while (cur != null)
            {
                if (cur.ID == ID)
                {
                    return (cur.Index);
                }

                cur = cur.Next;
            }

            return (0);
        }

        public long Add(VU_ID ID, string lbl, int color)
        {
            long idx;
            ACMI_HASHNODE cur, newhash;

            if (TableSize_ == 0 || Table_ == null) return 0;

            cur = Get(ID);

            if (cur != null)
            {
                if (lbl != null)
                {
                    cur.label = lbl;
                    cur.color = color;
                }

                return (cur.Index);
            }

            newhash = new ACMI_HASHNODE();
            newhash.ID = ID;

            if (lbl != null)
            {
                newhash.label = lbl;
            }
            else
                newhash.label = "";

            newhash.color = color;
            newhash.Index = ID_++;
            newhash.Next = null;

            idx = ((uint)ID.creator_ | ID.num_) % TableSize_;

            if (Table_[idx].Root_ == null)
            {
                Table_[idx].Root_ = newhash;
            }
            else
            {
                cur = Table_[idx].Root_;

                while (cur.Next != null)
                    cur = cur.Next;

                cur.Next = newhash;
            }

            return (newhash.Index);
        }

        public void Remove(VU_ID ID)
        {
            long idx;
            ACMI_HASHNODE cur, prev;

            if (TableSize_ == 0 || Table_ == null) return;

            idx = ((uint)ID.creator_ | ID.num_) % TableSize_;

            if (Table_[idx].Root_ == null) return;

            //?? Table_[idx].Root_;

            if (Table_[idx].Root_.ID == ID)
            {
                prev = Table_[idx].Root_;
                Table_[idx].Root_ = Table_[idx].Root_.Next;
                //delete prev;
            }
            else
            {
                cur = Table_[idx].Root_;

                while (cur.Next != null)
                {
                    if (cur.Next.ID == ID)
                    {
                        prev = cur.Next;
                        cur.Next = cur.Next.Next;
                        //delete prev;
                        return;
                    }

                    cur = cur.Next;
                }
            }
        }

        public long GetFirst(ref ACMI_HASHNODE current, ref long curidx)
        {
            ACMI_HASHNODE cur;

            curidx = 0;
            current = null;

            cur = Table_[curidx].Root_;

            while (cur == null && curidx < (TableSize_ - 1))
            {
                curidx++;
                cur = Table_[curidx].Root_;
            }

            if (curidx < TableSize_)
            {
                current = cur;

                if (cur != null)
                    return (cur.Index);
            }

            current = null;
            return (-1);
        }

        public long GetNext(ref ACMI_HASHNODE current, ref long curidx)
        {
            ACMI_HASHNODE cur;

            if (current == null)
                return (-1);

            cur = current.Next;

            while (cur == null && curidx < (TableSize_ - 1))
            {
                curidx++;
                cur = Table_[curidx].Root_;
            }

            current = cur;

            if (cur != null)
                return (cur.Index);

            return (-1);
        }

        public long GetLastID()
        {
            return (ID_);
        }
    }
}
