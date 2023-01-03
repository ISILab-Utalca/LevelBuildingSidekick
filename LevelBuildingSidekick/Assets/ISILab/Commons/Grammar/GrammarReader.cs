using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Recognition.SrgsGrammar;
using System;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class GrammarReader
{
    // Start is called before the first frame update
    public  GrammarReader()
    {
    }

    public static GrammarTree ReadGrammar(string path)
    {
        try
        {
            SrgsDocument Grammar = new SrgsDocument(path);
            return ParseGrammar(Grammar);
        }
        catch
        {
            Debug.LogError("Path does not lead to a SRGS Grammar: " + path);
            return null;
        }
    }

    public static void WriteGrammar(string path, SrgsDocument document)
    {
        string srgsDocumentFile = Path.Combine(path, "srgsDocumentFile.xml");
        Debug.Log(srgsDocumentFile);
        // Create an XmlWriter object and pass the file path.
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(srgsDocumentFile, settings);

        // Write the contents of the XmlWriter object to an SRGS-compatible XML file.  
        document.WriteSrgs(writer);
        writer.Close();
    }

    public static GrammarTree ParseGrammar(SrgsDocument grammarDoc)
    {
        var gt = new GrammarTree();
        gt.Root = ProcessSrgsRule(grammarDoc.Root, gt, grammarDoc);
        return gt;
    }


    private static GrammarNode ProcessSrgsElement(SrgsElement element, GrammarTree grammar, SrgsDocument doc, string id)
    {
        if (element is SrgsText)
        {
            return ProcessSrgsText(element as SrgsText, grammar);
        }
        if (element is SrgsOneOf)
        {
            return ProcessSrgsOneOf(element as SrgsOneOf, grammar, doc, id);
        }
        if (element is SrgsItem)
        {
            return ProcessSrgsItem(element as SrgsItem, grammar, doc, id);
        }
        if (element is SrgsRuleRef)
        {
            return ProcessSrgsRuleRef(element as SrgsRuleRef, grammar, doc);
        }
        return new TerminalNode("");
    }

    private static TerminalNode ProcessSrgsText(SrgsText text, GrammarTree grammar)
    {
        if (!grammar.Terminals.ContainsKey(text.Text))
        {
            grammar.Terminals.Add(text.Text, new TerminalNode(text.Text));
        }
        return grammar.Terminals[text.Text];
    }

    private static NonTerminalNode ProcessSrgsOneOf(SrgsOneOf oneOf, GrammarTree grammar, SrgsDocument doc, string id)
    {
        if (!grammar.NonTerminals.ContainsKey(id))
        {
            var node = new NonTerminalNode(id);
            grammar.NonTerminals.Add(id, node);
            for (int i = 0; i < oneOf.Items.Count; i++)
            {
                node.AppendNode(ProcessSrgsItem(oneOf.Items[i], grammar, doc, id + "." + i));
            }
        }
        
        return grammar.NonTerminals[id];
    }

    private static GrammarNode ProcessSrgsItem(SrgsItem item, GrammarTree grammar, SrgsDocument doc, string id)
    {
        bool b = id.StartsWith(doc.Root.Id.Trim('#')); // don not erase this line

        if (item.Elements.Count == 1 && b)
        {
            return ProcessSrgsElement(item.Elements[0], grammar, doc, id);
        }


        if (!grammar.Productions.ContainsKey(id))
        {
            var pn = new ProductionNode(id);
            grammar.Productions.Add(id, pn);
            for(int i = 0; i < item.Elements.Count; i++)
            {
                pn.AppendNode(ProcessSrgsElement(item.Elements[i], grammar, doc, id + "." + i));
            }
        }

        return grammar.Productions[id];
    }

    private static ProductionNode ProcessSrgsRuleRef(SrgsRuleRef ruleRef, GrammarTree grammar, SrgsDocument doc)
    {
        doc.Rules.TryGetValue(ruleRef.Uri.ToString().Trim('#'), out SrgsRule r);

        return ProcessSrgsRule(r, grammar, doc);
    }

    private static ProductionNode ProcessSrgsRule(SrgsRule rule, GrammarTree grammar, SrgsDocument doc)
    {
        if (!grammar.Productions.ContainsKey(rule.Id))
        {
            var r = new ProductionNode(rule.Id);
            grammar.Productions.Add(rule.Id, r);
            for(int i = 0; i < rule.Elements.Count; i++)
            {
                r.AppendNode(ProcessSrgsElement(rule.Elements[i], grammar, doc, rule.Id + "." + i));
            }
        }

        return grammar.Productions[rule.Id];
    }

}

