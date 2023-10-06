using System;
using System.Diagnostics;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using System.Linq;
using System.Collections;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// A base class for chromosomes.
    /// </summary>
    [DebuggerDisplay("Fitness:{Fitness}, Genes:{Length}")]
    [Serializable]
    public abstract class ChromosomeBase : IOptimizable
    {
        #region Fields
        protected object[] genes;
        protected int[] immutableIndexes;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeBase"/> class.
        /// </summary>
        /// <param name="length">The length, in genes, of the chromosome.</param>
        protected ChromosomeBase(int length, int[] immutables = null)
        {
            ValidateLength(length);
            genes = new object[length];

            immutableIndexes = immutables;
            if (immutableIndexes == null)
                immutableIndexes = new int[0];

        }

        protected ChromosomeBase()
        {
            genes = new object[0];
            immutableIndexes = new int[0];
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the fitness of the chromosome in the current problem.
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Gets the length, in genes, of the chromosome.
        /// </summary>

        public int Length => genes.Length;
        public int ImmutablesCount => immutableIndexes.Length;
        #endregion

        #region Methods
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ChromosomeBase first, ChromosomeBase second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }

            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.CompareTo(second) == 0;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(ChromosomeBase first, ChromosomeBase second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return false;
            }

            if ((object)first == null)
            {
                return true;
            }

            if ((object)second == null)
            {
                return false;
            }

            return first.CompareTo(second) < 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(ChromosomeBase first, ChromosomeBase second)
        {
            return !(first == second) && !(first < second);
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public abstract ChromosomeBase CreateNewChromosome();

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public abstract ChromosomeBase CloneChromosome();

        /// <summary>
        /// Replaces the gene in the specified index.
        /// </summary>
        /// <param name="index">The gene index to replace.</param>
        /// <param name="gene">The new gene.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index;There is no Gene on index {0} to be replaced..With(index)</exception>
        public virtual void ReplaceGene<T>(int index, T gene)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "There is no Gene on index {0} to be replaced.".With(index));
            }
            try
            {
                genes[index] = gene; 
            }
            catch
            {
                throw new TypeAccessException("Incorrect Type T for " + GetType().Name);
            }
            Fitness = float.NaN;
        }

        /// <summary>
        /// Replaces the genes starting in the specified index.
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        /// <param name="genes">The genes.</param>
        /// <remarks>
        /// The genes to be replaced can't be greater than the available space between the start index and the end of the chromosome.
        /// </remarks>
        public virtual void ReplaceGenes<T>(int startIndex, T[] genes)
        {
            ExceptionHelper.ThrowIfNull("genes", genes);

            if (genes.Length > 0)
            {
                if (startIndex < 0 || startIndex >= Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "There is no Gene on index {0} to be replaced.".With(startIndex));
                }

                var genesToBeReplacedLength = genes.Length;

                var availableSpaceLength = Length - startIndex;

                if (genesToBeReplacedLength > availableSpaceLength)
                {
                    throw new ArgumentException(
                        "The number of genes to be replaced is greater than available space, there is {0} genes between the index {1} and the end of chromosome, but there is {2} genes to be replaced."
                        .With(availableSpaceLength, startIndex, genesToBeReplacedLength));
                }

                Array.Copy(genes, 0, this.genes, startIndex, genes.Length);

                Fitness = float.NaN;
            }
        }

        /// <summary>
        /// Resizes the chromosome to the new length.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        public void Resize(int newLength)
        {
            ValidateLength(newLength);

            Array.Resize(ref genes, newLength);
        }

        /// <summary>
        /// Gets the gene in the specified index.
        /// </summary>
        /// <param name="index">The gene index.</param>
        /// <returns>
        /// The gene.
        /// </returns>
        public virtual T GetGene<T>(int index)
        {
            try
            {
                return ((T)((object)genes[index]));
            }
            catch
            {
                throw new TypeAccessException("Incorrect Type T for " + GetType().Name);
            }
        }

        /// <summary>
        /// Gets the genes.
        /// </summary>
        /// <returns>The genes.</returns>
        public virtual T[] GetGenes<T>()
        {
            try
            {
                return genes.Select(g => (T)(object)g).ToArray();
            }
            catch
            {
                throw new TypeAccessException("Incorrect Type T for " + GetType().Name);
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="other">The other chromosome.</param>
        public int CompareTo(ChromosomeBase other)
        {
            if (other == null)
            {
                return -1;
            }

            var otherFitness = other.Fitness;

            if (Fitness == otherFitness)
            {
                return 0;
            }

            return Fitness > otherFitness ? 1 : -1;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="GeneticSharp.Domain.Chromosomes.ChromosomeBase"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as ChromosomeBase;

            if (other == null)
            {
                return false;
            }

            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Fitness.GetHashCode();
        }

        /// <summary>
        /// Creates the gene on specified index.
        /// <remarks>
        /// It's a shortcut to:  
        /// <code>
        /// ReplaceGene(index, GenerateGene(index));
        /// </code>
        /// </remarks>
        /// </summary>
        /// <param name="index">The gene index.</param>
        protected void CreateGene(int index)
        {
            ReplaceGene(index, GenerateGene());
        }

        /// <summary>
        /// Creates all genes
        /// <remarks>
        /// It's a shortcut to: 
        /// <code>
        /// for (int i = 0; i &lt; Length; i++)
        /// {
        ///     ReplaceGene(i, GenerateGene(i));
        /// }
        /// </code>
        /// </remarks>
        /// </summary>        
        protected virtual void CreateGenes()
        {
            for (int i = 0; i < Length; i++)
            {
                CreateGene(i);
            }
        }

        /// <summary>
        /// Validates the length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <exception cref="System.ArgumentException">The minimum length for a chromosome is 2 genes.</exception>
        private static void ValidateLength(int length)
        {
            if (length < 2)
            {
                throw new ArgumentException("The minimum length for a chromosome is 2 genes.", nameof(length));
            }
        }

        public object GetGene(int index)
        {
            return genes[index];
        }

        public object[] GetGenes()
        {
            return genes.Select(g => g as object).ToArray();
        }

        public abstract object GenerateGene();

        IOptimizable IOptimizable.CreateNew()
        {
            return CreateNewChromosome();
        }


        public object Clone()
        {
            return CloneChromosome();
        }

        public abstract bool IsValid();

        public void SetDataSequence<T>(T[] data)
        {
            if(data == null)
            {
                throw new NullReferenceException();
            }
            for(int i = 0; i < data.Length; i++)
            {
                ReplaceGene(i, data[i]);
            }
        }
        
        public T GetData<T>()
        {
            if(!(GetType() is T))
            {
                throw new TypeAccessException("Invalid type requested, " + GetType().ToString() + " is not instance of type: " + typeof(T).ToString());
            }
            return (T)(object)(this);
        }

        public T[] GetDataSquence<T>()
        {
            return genes.Select(g => (T)g).ToArray();
        }

        public void SetData<T>(T data)
        {
            throw new NotImplementedException();
        }

        public bool IsImmutable(int index)
        {
            return immutableIndexes.Contains(index);
        }

        public abstract void SetDeafult(int index);
        #endregion
    }
}
