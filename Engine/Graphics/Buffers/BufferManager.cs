using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics.Buffers
{
    class BufferManager: IDisposable
    {
        private VertexBuffer buffer;
        private Allocator allocator;

        public BufferManager(int size, int chunkSize)
        {
            allocator = new Allocator(size, chunkSize);
            buffer = new VertexBuffer(size, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
        }

        protected bool disposed;
        public void Dispose()
        {
            if (!disposed) return;
            buffer.Dispose();
            disposed = true;
        }
    }

    public class Allocator
    {
        private Block head; // First element of the linked list of memory blocks
        private Dictionary<int, Block> chunks; // To find buddies by offset
        public int Size { get; set; }
        public int Used { get; set; }
        protected int ChunkSize { get; set; }

        public Allocator(int size, int chunkSize)
        {
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Non-power-of-two size isn't supported.", "size");
            if ((chunkSize & (chunkSize - 1)) != 0)
                throw new ArgumentException("Non-power-of-two chunk size isn't supported.", "chunkSize");
            ChunkSize = chunkSize;
            Size = size;
            chunks = new Dictionary<int, Block>(Size / ChunkSize);
            head = new Block(this, (int) Math.Round(Math.Log(size, 2)), 0);
        }

        public BlockHandle Allocate(int size)
        {
            if (size < ChunkSize) size = ChunkSize;
            size = (int) Math.Pow(2, Math.Ceiling(Math.Log(size, 2)));
            var current = head;
            Block bestmatch = null;

            while (current != null)
            {
                if (!current.IsUsed)
                    if (current.Size > size)
                    {
                        bestmatch = current;
                        break;
                    }
                    else if (current.Size == size)
                    {
                        // Got perfectly fitted block
                        current.IsUsed = true;
                        Used += current.Size;
                        return new BlockHandle(current);
                    }
                current = current.Next;
            }
            
            if (bestmatch == null) return null; // No free blocks at all. throw an exception?

            // Free block is too large. splitting it.
            while (bestmatch.Size > size)
                bestmatch = bestmatch.Split(); // Chop off half a block each time
            bestmatch.IsUsed = true;
            Used += bestmatch.Size;
            return new BlockHandle(bestmatch);
        }

        internal class Block
        {
            private Allocator parent;
            public Block Previous { get; private set; }
            public Block Next { get; private set; }

            private int k;
            public bool IsUsed { get; set; }
            public int Offset { get; private set; }
            public int Size
            {
                get { return (int) Math.Pow(2, k); }
            }

            public Block(Allocator parent, int k, int offset)
            {
                this.parent = parent;
                this.k = k;
                Offset = offset;
                IsUsed = false;
                parent.chunks.Add(offset, this);
            }

            public Block Split()
            {
                if (IsUsed) return null;
                if (Size == parent.ChunkSize) return null;
                k--;
                new Block(parent, k, Offset + Size).AddAfter(this);
                return this;
            }

            public void Free()
            {
                parent.Used -= Size;
                FreeBlock();
            }

            private void FreeBlock()
            {
                IsUsed = false;
                while ((Size < parent.Size) && !Buddy.IsUsed)
                {
                    if (Offset > Buddy.Offset)
                    {
                        Buddy.FreeBlock();
                        return;
                    }
                    Buddy.Remove();
                    k++;
                }
            }

            private void Remove()
            {
                parent.chunks.Remove(Offset);
                if (Previous != null) Previous.Next = Next;
                if (Next != null) Next.Previous = Previous;
            }

            private void AddAfter(Block block)
            {
                if (block.Next != null)
                {
                    Next = block.Next;
                    Next.Previous = this;
                }
                block.Next = this;
                Previous = block;
            }

            private Block Buddy 
            {
                get
                {
                    var buddyOffset =  Offset ^ (1 << k);
                    return parent.chunks[buddyOffset];
                }
            }
        }

        public class BlockHandle: IDisposable
        {
            private Block block;

            internal BlockHandle(Block b)
            {
                block = b;
            }

            public bool IsUsed { get { return block.IsUsed; } }
            public int Offset { get { return block.Offset; } }
            public int Size { get { return block.Size; } }
            
            public void Free()
            {
                Dispose();
            }

            private bool disposed;
            public void Dispose()
            {
                if (disposed) return;
                disposed = true; 
                block.Free();
            }

            ~BlockHandle()
            {
                Dispose();
            }

            public override string ToString()
            {
                return string.Format("Block: {0} bytes @ {1}", Size, Offset);
            }
        }
    }
}
