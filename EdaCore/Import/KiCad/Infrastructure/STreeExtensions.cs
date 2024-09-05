using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    public static class STreeExtensions {

        public static double ValueAsDouble(this STree tree, SNode node) {

            if (node is SLeaf n) {
                string s = tree.GetNodeValue(n);
                double r = Double.Parse(s, CultureInfo.InvariantCulture);
                return r;
            }

            else
                throw new InvalidOperationException("No es un nodo terminal.");
        }

        public static int ValueAsInteger(this STree tree, SNode node) {

            if (node is SLeaf n) {
                string s = tree.GetNodeValue(n);
                return Int32.Parse(s);
            }

            else
                throw new InvalidOperationException("No es un nodo terminal.");
        }

        public static bool ValueAsBoolean(this STree tree, SNode node) {

            if (node is SLeaf n) {
                string s = tree.GetNodeValue(n);
                return Boolean.Parse(s);
            }

            else
                throw new InvalidOperationException("No es un nodo terminal.");
        }

        public static string ValueAsString(this STree tree, SNode node) {

            if (node is SLeaf n)
                return tree.GetNodeValue(n);

            else
                throw new InvalidOperationException("No es un nodo terminal.");
        }

        public static string ValueAsStrings(this STree tree, SNode node) {

            var sb = new StringBuilder();
            bool first = true;
            foreach (var n in (node as SBranch).Nodes) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(tree.GetNodeValue(n as SLeaf));
            }
            return sb.ToString();
        }

        public static string GetBranchName(this STree tree, SBranch branch) {

            return tree.GetNodeValue(branch[0] as SLeaf);
        }

        public static SBranch SelectBranch(this STree tree, SNode node, string name) {

            var branch = node as SBranch;
            if (branch == null)
                return null;

            else {
                foreach (var childBranch in branch.Nodes.OfType<SBranch>())
                    if (tree.GetBranchName(childBranch) == name)
                        return childBranch;

                return null;
            }
        }

        public static IEnumerable<SBranch> SelectBranches(this STree tree, SNode node, string name) {

            var branch = node as SBranch;
            if (branch == null)
                return Enumerable.Empty<SBranch>();

            else {
                var nodes = new List<SBranch>();

                foreach (var childNode in branch.Nodes.OfType<SBranch>())
                    if (tree.GetBranchName(childNode) == name)
                        nodes.Add(childNode);

                return nodes;
            }
        }
    }
}
