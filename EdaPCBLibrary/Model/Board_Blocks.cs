namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Collections;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        // Blocs
        private ParentChildKeyCollection<Board, Block, String> blocks;

        /// <summary>
        /// Afeigeix un bloc.
        /// </summary>
        /// <param name="block">El bloc a afeigir.</param>
        /// 
        public void AddBlock(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (block.Board != null)
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', ya pertenece a una placa.", block.Name));

            if (blocks == null)
                blocks = new ParentChildKeyCollection<Board, Block, String>(this);
            blocks.Add(block);
        }

        public void AddBlocks(IEnumerable<Block> blocks) {

            if (blocks == null)
                throw new ArgumentNullException("blocks");

            foreach (var block in blocks)
                AddBlock(block);
        }

        /// <summary>
        /// Elimina un block.
        /// </summary>
        /// <param name="block">El bloc a eliminar.</param>
        /// 
        public void RemoveBlock(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (block.Board != this)
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}' no esta asignado a esta placa.", block.Name));

            blocks.Remove(block);
            if (blocks.IsEmpty)
                blocks = null;
        }

        /// <summary>
        /// Obte un bloc pel seu nom.
        /// </summary>
        /// <param name="name">El nom del bloc.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>El bloc, o null si no el troba.</returns>
        /// 
        public Block GetBlock(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Block block = blocks?.Get(name);
            if (block != null)
                return block;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', no esta asignado a esta placa.", name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte blocs
        /// </summary>
        /// 
        public bool HasBlocks {
            get {
                return blocks != null;
            }
        }

        /// <summary>
        /// Enumera els noms dels blocks
        /// </summary>
        /// 
        public IEnumerable<string> BlockNames {
            get {
                return blocks?.Keys;
            }
        }

        /// <summary>
        /// Obte un enumerador pels blocs.
        /// </summary>
        /// 
        public IEnumerable<Block> Blocks {
            get {
                return blocks;
            }
        }
    }
}
