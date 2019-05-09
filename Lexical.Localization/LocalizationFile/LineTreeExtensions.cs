﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System.Linq;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="ILineTree"/>.
    /// </summary>
    public static class LineTreeExtensions
    {
        /// <summary>
        /// Flatten <paramref name="keyTree"/> to string lines.
        /// </summary>
        /// <param name="keyTree"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> ToStringLines(this ILineTree keyTree, IParameterPolicy policy)
            => keyTree.ToKeyLines().ToStringLines(policy);

        /// <summary>
        /// Flatten <paramref name="node"/> to key lines.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<ILine, IFormulationString>> ToKeyLines(this ILineTree node)
        {
            Queue<(ILineTree, ILine)> queue = new Queue<(ILineTree, ILine)>();
            queue.Enqueue((node, node.Key));
            while (queue.Count > 0)
            {
                // Next element
                (ILineTree, ILine) current = queue.Dequeue();

                // Yield values
                if (current.Item2 != null && current.Item1.HasValues)
                {
                    foreach (IFormulationString value in current.Item1.Values)
                        yield return new KeyValuePair<ILine, IFormulationString>(current.Item2, value);
                }

                // Enqueue children
                if (current.Item1.HasChildren)
                {
                    foreach (ILineTree child in current.Item1.Children)
                    {
                        ILine childKey = current.Item2 == null ? child.Key : current.Item2.Concat(child.Key);
                        queue.Enqueue((child, childKey));
                    }
                }
            }
        }

        /// <summary>
        /// Create an asset that uses <paramref name="tree"/>.
        /// 
        /// Trees are reloaded into the asset if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this ILineTree tree)
            => new LocalizationAsset().Add(new ILineTree[] { tree }).Load();

        /// <summary>
        /// Create an asset that uses <paramref name="trees"/>.
        /// 
        /// Trees are reloaded into the asset if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="trees"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<ILineTree> trees)
            => new LocalizationAsset().Add(trees).Load();

        /// <summary>
        /// Convert <paramref name="tree"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(ILineTree tree)
            => new LocalizationKeyTreeSource(tree == null ? new ILineTree[0] : new ILineTree[] { tree });

        /// <summary>
        /// Convert <paramref name="trees"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="trees"></param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<ILineTree> trees)
            => new LocalizationKeyTreeSource(trees);

        /// <summary>
        /// Search child by key.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="key"></param>
        /// <returns>child node or null if was not found</returns>
        public static ILineTree GetChild(this ILineTree tree, ILine key)
            => tree.GetChildren(key)?.FirstOrDefault();

        /// <summary>
        /// Tests if <paramref name="tree"/> has a child with key <paramref name="key"/>.
        /// 
        /// If <paramref name="key"/> is null, then returns always false.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasChild(this ILineTree tree, ILine key)
        {
            if (key == null) return false;
            if (!tree.HasChildren) return false;
            IEnumerable<ILineTree> children = tree.GetChildren(key);
            if (children == null) return false;
            return children.Count() > 0;
        }

        /// <summary>
        /// Tests if <paramref name="tree"/> has a <paramref name="value"/>.
        /// 
        /// If <paramref name="value"/> is null, then returns always false.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this ILineTree tree, IFormulationString value)
        {
            if (value == null) return false;
            if (!tree.HasValues) return false;
            foreach(IFormulationString linevalue in tree.Values)
                if (FormulationStringComparer.Instance.Equals(value, linevalue)) return true;
            return false;
        }

        /// <summary>
        /// Get or create node by parameters in <paramref name="key"/>.
        /// 
        /// If <paramref name="key"/> contains no parameters, returns <paramref name="node"/>.
        /// </summary>
        /// <param name="node">(optional) not to append to</param>
        /// <param name="key">key that contains parameters</param>
        /// <returns>existing child, new child, or <paramref name="node"/> if no parameters were provided</returns>
        public static ILineTree GetOrCreate(this ILineTree node, ILine key)
        {
            if (node == null) return null;

            // No parameters
            if (key.GetParameterCount() == 0) return node;

            // Get existing child
            ILineTree child = node.GetChild(key);
            if (child != null) return child;

            // Create child
            child = node.CreateChild();
            child.Key = key;
            return child;
        }

        /// <summary>
        /// Create child node with parameters in <paramref name="key"/>.
        /// </summary>
        /// <param name="node">(optional) not to append to</param>
        /// <param name="key">key that contains parameters</param>
        /// <returns>new child</returns>
        public static ILineTree Create(this ILineTree node, ILine key)
        {
            if (node == null) return null;

            // Create child
            ILineTree child = node.CreateChild();
            child.Key = key;
            return child;
        }

        /// <summary>
        /// Adds key and/or value.
        /// 
        /// If argument <paramref name="key"/> is given, then get-or-creates child node, otherwise uses <paramref name="node"/>.
        /// If argument <paramref name="value"/> is given, then adds value.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key">(optional) possible initial key to set.</param>
        /// <param name="value">(optional) possible initial value to add</param>
        /// <returns><paramref name="node"/></returns>
        public static ILineTree Add(this ILineTree node, ILine key, IFormulationString value)
        {
            ILineTree n = node;
            if (key != null) n = n.GetChild(key);
            if (value != null) n.Values.Add(value);
            return node;
        }

        /// <summary>
        /// Add key-value recursively
        /// 
        /// If argument <paramref name="key_parts"/> is given, then get-or-creates decendent node, otherwise uses <paramref name="node"/>.
        /// If argument <paramref name="value"/> is given then adds value.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key_parts">(optional) possible initial key to set.</param>
        /// <param name="value">(optional) possible initial value to add</param>
        /// <returns>the leaf node where the values was added</returns>
        public static ILineTree AddRecursive(this ILineTree node, IEnumerable<ILine> key_parts, IFormulationString value)
        {
            // Drill into leaf
            ILineTree leaf = node;
            foreach (ILine key in key_parts)
                leaf = leaf.GetOrCreate(key);

            // Add value
            if (value != null && !leaf.Values.Contains(value)) leaf.Values.Add(value);

            // Return leaf
            return leaf;
        }

        /// <summary>
        /// Visit every decendent node in level-order.
        /// 
        /// Example:
        ///   .child1
        ///   .child2
        ///   .child1.child1
        ///   .child1.child2
        ///   .child2.child1
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static IEnumerable<ILineTree> Decendents(this ILineTree tree)
        {
            if (tree == null) yield break;

            Queue<ILineTree> queue = new Queue<ILineTree>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                ILineTree n = queue.Dequeue();
                if (n != tree) yield return n;

                if (n.HasChildren)
                    foreach (ILineTree child in n.Children)
                        queue.Enqueue(child);
            }
        }

        /// <summary>
        /// Visit every decendent node and concatenated key in level-order.
        /// 
        /// 
        /// Example:
        ///   .child1
        ///   .child2
        ///   .child1.child1
        ///   .child1.child2
        ///   .child2.child1
        /// </summary>
        /// <param name="node"></param>
        /// <returns>nodes and keys</returns>
        public static IEnumerable<KeyValuePair<ILineTree, ILine>> DecendentsWithConcatenatedKeys(this ILineTree node)
        {
            if (node == null) yield break;

            Queue<(ILineTree, ILine)> queue = new Queue<(ILineTree, ILine)>();
            queue.Enqueue((node, node.GetConcatenatedKey()));
            while (queue.Count > 0)
            {
                (ILineTree n, ILine key) = queue.Dequeue();
                if (n != node) yield return new KeyValuePair<ILineTree, ILine>(n, key);

                if (n.HasChildren)
                    foreach (ILineTree child in n.Children)
                        queue.Enqueue((child, key.Concat(child.Key)));
            }
        }

        /// <summary>
        /// Tests if <paramref name="decendent"/> is decendent of <paramref name="anchestor"/>.
        /// </summary>
        /// <param name="decendent"></param>
        /// <param name="anchestor"></param>
        /// <returns></returns>
        public static bool IsDecendentOf(this ILineTree decendent, ILineTree anchestor)
        {
            for (ILineTree t = decendent; t != null; t = t.Parent)
                if (t == anchestor) return true;
            return false;
        }

        /// <summary>
        /// List parameters from root to tail.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>parameters</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetConcatenatedParameters(this ILineTree node)
        {
            IEnumerable<KeyValuePair<string, string>> result = null;
            if (node.Parent != null) result = node.Parent.GetConcatenatedParameters();
            IEnumerable<KeyValuePair<string, string>> key = node.Key.GetParameters();
            result = result == null ? key : result.Concat(key);
            return result;
        }

        /// <summary>
        /// Get concatenated key from root to <paramref name="node"/>.
        /// </summary>
        /// <returns>key</returns>
        public static ILine GetConcatenatedKey(this ILineTree node)
             => node.Parent != null ? node.Parent.GetConcatenatedKey().Concat(node.Key) : node.Key;

        /// <summary>
        /// Visit all nodes from root to <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static ILineTree[] VisitFromRoot(this ILineTree tree)
        {
            if (tree == null) return new ILineTree[0];
            int count = 0;
            for (ILineTree t = tree; t != null; t = t.Parent) count++;
            ILineTree[] result = new ILineTree[count];
            for (ILineTree t = tree; t != null; t = t.Parent)
                result[--count] = t;
            return result;
        }

        /// <summary>
        /// Get root node of <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns>root, or null if <paramref name="tree"/> is null</returns>
        public static ILineTree GetRoot(this ILineTree tree)
        {
            if (tree == null) return null;
            while (tree.Parent != null) tree = tree.Parent;
            return tree;
        }

        /// <summary>
        /// Calculate the level of <paramref name="tree"/>.
        /// <list>
        /// <item>0: Root</item>
        /// <item>1: First level</item>
        /// <item>2: Second level</item>
        /// </list>
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static int GetLevel(this ILineTree tree)
        {
            int level = 0;
            while (tree.Parent != null)
            {
                level++;
                tree = tree.Parent;
            }
            return level;
        }

        /// <summary>
        /// Get value count
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int ValueCount(this ILineTree node)
            => !node.HasValues ? 0 : node.Values.Count();

        /// <summary>
        /// Get child count.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int ChildCount(this ILineTree node)
            => !node.HasChildren ? 0 : node.Children.Count();

        /// <summary>
        /// Search decendents for tree nodes that have matching key. 
        /// </summary>
        /// <param name="node">node to start search.</param>
        /// <param name="searchKey">key to search</param>
        /// <returns>nodes that have matching key</returns>
        public static IEnumerable<ILineTree> Search(this ILineTree node, ILine searchKey)
        {
            List<ILineTree> result = new List<ILineTree>();
            _search(node, searchKey, node.Key, result);
            return result;
        }

        static void _search(ILineTree node, ILine searchKey, ILine concatenatedKeyOfNode, List<ILineTree> result)
        {
            // Check for mismatch
            for (ILine k = concatenatedKeyOfNode; k != null; k = k.GetPreviousPart())
            {
                string parameterName = k.GetParameterName();
                if (parameterName == null) continue;
                string parameterValue = k.GetParameterValue();

                bool parameterDetectedInSearchKey = false;
                for (ILine sk = searchKey; sk != null; sk = sk.GetPreviousPart())
                {
                    string _parameterName = sk.GetParameterName();
                    if (_parameterName == null) continue;
                    string _parameterValue = sk.GetParameterValue();
                    parameterDetectedInSearchKey |= _parameterValue == parameterValue;
                    if (parameterDetectedInSearchKey) break;
                }

                // node has a parameter that was not found in search key. This is wrong branch
                if (!parameterDetectedInSearchKey) return;
            }

            // Key match
            if (LineComparer.Default.Equals(concatenatedKeyOfNode, searchKey)) { result.Add(node); return; }

            // Recurse
            if (node.HasChildren)
            {
                foreach (ILineTree child in node.Children)
                {
                    ILine newConcatenatedKey = concatenatedKeyOfNode.Concat(child.Key);
                    _search(child, searchKey, newConcatenatedKey, result);
                }
            }
        }

    }
}