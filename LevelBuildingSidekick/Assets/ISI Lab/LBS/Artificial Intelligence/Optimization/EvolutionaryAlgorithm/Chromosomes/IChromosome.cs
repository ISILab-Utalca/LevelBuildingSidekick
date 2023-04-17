using System;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// Defines an interface for a chromosome.
    /// <remarks>
    /// In genetic algorithms, a chromosome (also sometimes called a genome) is a set of parameters 
    /// which define a proposed solution to the problem that the genetic algorithm is trying to solve. 
    /// The chromosome is often represented as a simple string, although a wide variety of other data 
    /// structures are also used.
    /// <see href="http://en.wikipedia.org/wiki/Chromosome_(genetic_algorithm)">http://en.wikipedia.org/wiki/Chromosome_(genetic_algorithm)</see> 
    /// </remarks>
    /// </summary>
    public interface IChromosome : IComparable<IChromosome>, IOptimizable
    {
        #region Properties
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        int Length { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        object GenerateGene();

        /// <summary>
        /// Replaces the gene in the specified index.
        /// </summary>
        /// <param name="index">The gene index to replace.</param>
        /// <param name="gene">The new gene.</param>
        void ReplaceGene<T>(int index, T gene);

        /// <summary>
        /// Replaces the genes starting in the specified index.
        /// </summary>
        /// <remarks>
        /// The genes to be replaced can't be greater than the available space between the start index and the end of the chromosome.
        /// </remarks>
        /// <param name="startIndex">Start index.</param>
        /// <param name="genes">The genes.</param>
        void ReplaceGenes<T>(int startIndex, T[] genes);

        /// <summary>
        /// Resizes the chromosome to the new length.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        void Resize(int newLength);

        /// <summary>
        /// Gets the gene in the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="index">The gene index.</param>
        T GetGene<T>(int index);
        object GetGene(int index);

        /// <summary>
        /// Gets the genes.
        /// </summary>
        /// <returns>The genes.</returns>
        T[] GetGenes<T>();
        object[] GetGenes();

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        IChromosome CreateNewChromosome();

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        IChromosome CloneChromosome();

        public bool IsValid();
        public T[] GetDataSquence<T>();

        public T GetData<T>();
        public void SetDataSequence<T>(T[] data);
        public void SetData<T>(T data);
        public void SetDeafult(int index);
        #endregion
    }
}