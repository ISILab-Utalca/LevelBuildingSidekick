using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition.SrgsGrammar;
using UnityEngine;

namespace ISILab.AI.Grammar
{
    public static class LBSGrammarReader
    {
        public class Grammar
        {
            // the string that is used
            public string name;

            public bool IsNonTerminal()
            {
                return GetTrimmed()[0] == '#';
            }
            
            public bool IsTerminal()
            {
                return GetTrimmed()[0] != '#';
            }

            public string GetTrimmed()
            {
                return name.Trim();
            }
        }

        public class TerminalGrammar : Grammar
        {
            
        }

        // Non-terminals have expansions that can be Grammars
        public class NonTerminal : Grammar
        {
            private List<Grammar> Expansions = new();

            public List<string> GetTerminals()
            {
                List<string> terminals = new();
                foreach (var expansion in Expansions)
                {
                    if (expansion is NonTerminal nonTerminal)
                    {
                        foreach (var terminal in nonTerminal.GetTerminals())
                        {
                            terminals.Add(terminal);
                        }
                    }
                    else if (expansion is TerminalGrammar terminalGrammar)
                    {
                        terminals.Add(terminalGrammar.name);
                    }
                }
                
                return terminals;
            }
        }
        
        public class RuleData
        {
            public string RuleName;
            public List<List<string>> Expansions = new(); // Each list is a sequence (can contain terminals and rule refs)
            
            public string GetFirstTerminal(Dictionary<string, RuleData> Rules, string ruleName)
            {
                if (Rules.TryGetValue(ruleName, out var rule))
                    return rule.GetFirstTerminal(Rules,ruleName);
                return null;
            }
        }

        public class GrammarStructure
        {
            public Dictionary<string, RuleData> Rules = new();
            public HashSet<string> Terminals = new();
            

        }

        public static GrammarStructure ReadGrammar(string path)
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
        ///  Parsing Grammar assumes that your grammar meets the following requirements
        /// Rules: Start with #, followed by a Cap Character
        /// Terminals: Start with Cap
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static GrammarStructure ParseGrammar(SrgsDocument doc)
        {
            var grammar = new GrammarStructure();

            foreach (var rule in doc.Rules)
            {
                if (!grammar.Rules.ContainsKey(rule.Id))
                {
                    grammar.Rules[rule.Id] = new RuleData { RuleName = rule.Id };
                }

                foreach (var element in rule.Elements)
                {
                    ExtractExpansionSequences(element, grammar.Rules[rule.Id].Expansions, grammar.Terminals);
                }
            }

            return grammar;
        }

        private static void ExtractExpansionSequences(SrgsElement element, List<List<string>> expansions, HashSet<string> terminals)
        {
            switch (element)
            {
                case SrgsItem item:
                    var sequence = new List<string>();

                    foreach (var subElement in item.Elements)
                    {
                        switch (subElement)
                        {
                            case SrgsText text:
                                var tokens = text.Text.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var token in tokens)
                                {
                                    // Further split each token by capital letter not preceded by #
                                    int start = 0;
                                    for (int i = 1; i < token.Length; i++)
                                    {
                                        if (char.IsUpper(token[i]) && token[i - 1] != '#')
                                        {
                                            string subToken = token.Substring(start, i - start).Trim();
                                            if (!string.IsNullOrEmpty(subToken))
                                            {
                                                sequence.Add(subToken);
                                                terminals.Add(subToken);
                                            }
                                            start = i;
                                        }
                                    }

                                    // Add the last sub-token
                                    string lastToken = token.Substring(start).Trim();
                                    if (!string.IsNullOrEmpty(lastToken))
                                    {
                                        sequence.Add(lastToken);
                                        terminals.Add(lastToken);
                                    }
                                }

                                break;

                            case SrgsRuleRef ruleRef:
                                var refName = $"#{ruleRef.Uri.ToString().Trim('#')}";
                                sequence.Add(refName);
                                break;

                            case SrgsItem subItem:
                                // Handle nested item recursively as an additional expansion
                                var nested = new List<List<string>>();
                                ExtractExpansionSequences(subItem, nested, terminals);
                                foreach (var nestedSeq in nested)
                                    sequence.AddRange(nestedSeq);
                                break;

                            case SrgsOneOf innerOneOf:
                                foreach (var alt in innerOneOf.Items)
                                {
                                    var altSeq = new List<List<string>>();
                                    ExtractExpansionSequences(alt, altSeq, terminals);
                                    foreach (var seq in altSeq)
                                    {
                                        var full = new List<string>(sequence);
                                        full.AddRange(seq);
                                        expansions.Add(full);
                                    }
                                }
                                return; // Exit because alternatives already added

                            default:
                                Debug.LogWarning($"[LBSGrammarReader] Unhandled subElement type: {subElement.GetType()}");
                                break;
                        }
                    }

                    if (sequence.Count > 0)
                        expansions.Add(sequence);
                    break;

                case SrgsOneOf oneOf:
                    foreach (var itemAlt in oneOf.Items)
                        ExtractExpansionSequences(itemAlt, expansions, terminals);
                    break;

                case SrgsRuleRef ruleRef:
                    expansions.Add(new List<string> { $"#{ruleRef.Uri.ToString().Trim('#')}" });
                    break;
            }
        }
    }
}
