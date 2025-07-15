using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition.SrgsGrammar;
using System.Xml.Linq;
using UnityEngine;

namespace ISILab.AI.Grammar
{
    /// <summary>
    /// Utility class to parse SRGS grammar XML files into usable in-game rule structures.
    /// </summary>
    public static class LBSGrammarReader
    {
        private static readonly XNamespace Namespace = "http://www.w3.org/2001/06/grammar";

        /// <summary>
        /// Extracts terminal (literal) action strings from an SRGS grammar file.
        /// Ignores references to other rules (rulerefs).
        /// </summary>
        /// <param name="path">File path to the SRGS XML file.</param>
        /// <returns>A HashSet of terminal (literal) action strings.</returns>
        public static HashSet<string> GetTerminalActions(string path)
        {
            var doc = XDocument.Load(path);
            var terminals = new HashSet<string>();

            // Search all <item> nodes and extract terminal text content
            foreach (var item in doc.Descendants(Namespace + "item"))
            {
                foreach (var token in ExtractTerminalsRecursive(item))
                {
                    var trimmed = token.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        terminals.Add(trimmed);
                }
            }

            return terminals;
        }

        /// <summary>
        /// Recursively traverses an XML node tree to extract terminal text content.
        /// Skips <ruleref> nodes, as they are references to other rules.
        /// </summary>
        /// <param name="element">The starting XElement node.</param>
        /// <returns>A list of extracted terminal strings.</returns>
        private static List<string> ExtractTerminalsRecursive(XElement element)
        {
            var result = new List<string>();

            foreach (var node in element.Nodes())
            {
                switch (node)
                {
                    case XText textNode:
                        // Split by period and trim whitespace
                        result.AddRange(textNode.Value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
                        break;

                    case XElement el when el.Name.LocalName == "item":
                        result.AddRange(ExtractTerminalsRecursive(el));
                        break;

                    // Ignore <ruleref> elements
                }
            }

            return result;
        }

        /// <summary>
        /// Loads and parses an SRGS grammar file into a dictionary of rules and their expansions.
        /// </summary>
        /// <param name="path">Path to the SRGS XML file.</param>
        /// <returns>A dictionary mapping rule IDs to sets of possible expansion phrases.</returns>
        public static Dictionary<string, HashSet<string>> ReadGrammar(string path)
        {
            try
            {
                var srgsDoc = new SrgsDocument(path);
                return ParseGrammar(srgsDoc);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LBSGrammarReader] Failed to parse SRGS grammar: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Converts an SrgsDocument into a dictionary of rule expansions.
        /// </summary>
        /// <param name="doc">The parsed SRGS document.</param>
        /// <returns>Dictionary of rule ID to a set of valid expansions.</returns>
        private static Dictionary<string, HashSet<string>> ParseGrammar(SrgsDocument doc)
        {
            var grammar = new Dictionary<string, HashSet<string>>();

            foreach (var rule in doc.Rules)
            {
                if (!grammar.ContainsKey(rule.Id))
                    grammar[rule.Id] = new HashSet<string>();

                foreach (var element in rule.Elements)
                    ExtractPhrases(element, grammar[rule.Id]);
            }

            return grammar;
        }

        /// <summary>
        /// Recursively extracts literal phrases from an SRGS element tree and stores them in the target set.
        /// </summary>
        /// <param name="element">The root SRGS element.</param>
        /// <param name="target">The HashSet to store found phrases.</param>
        private static void ExtractPhrases(SrgsElement element, HashSet<string> target)
        {
            switch (element)
            {
                case SrgsItem item:
                    // Build phrase from literal text elements
                    var phrase = string.Join(" ",
                        item.Elements.OfType<SrgsText>()
                                     .Select(t => t.Text.Trim())
                                     .Where(t => !string.IsNullOrEmpty(t)));

                    if (!string.IsNullOrEmpty(phrase))
                        target.Add(phrase);

                    // Recursively extract from all sub-elements
                    foreach (var subElement in item.Elements)
                        ExtractPhrases(subElement, target);
                    break;

                case SrgsOneOf oneOf:
                    foreach (var itemAlt in oneOf.Items)
                        ExtractPhrases(itemAlt, target);
                    break;

                case SrgsRuleRef ruleRef:
                    // Keep rule references as "#RuleName"
                    target.Add($"#{ruleRef.Uri.ToString().Trim('#')}");
                    break;
            }
        }
    }
}
