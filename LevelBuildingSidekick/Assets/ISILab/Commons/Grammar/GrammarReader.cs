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


    private static GrammarNode ProcessSrgsElement(SrgsElement element, GrammarTree grammar, SrgsDocument doc)
    {
        if (element is SrgsText)
        {
            return ProcessSrgsText(element as SrgsText, grammar);
        }
        if (element is SrgsOneOf)
        {
            return ProcessSrgsOneOf(element as SrgsOneOf, grammar, doc);
        }
        if (element is SrgsItem)
        {
            return ProcessSrgsItem(element as SrgsItem, grammar, doc);
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

    private static NonTerminalNode ProcessSrgsOneOf(SrgsOneOf oneOf, GrammarTree grammar, SrgsDocument doc)
    {
        List<GrammarNode> nodes = new List<GrammarNode>();
        foreach (var i in oneOf.Items)
        {
            nodes.Add(ProcessSrgsItem(i, grammar, doc));
        }
        return new NonTerminalNode(nodes);
    }

    private static GrammarNode ProcessSrgsItem(SrgsItem item, GrammarTree grammar, SrgsDocument doc)
    {
        if (item.Elements.Count == 1)
        {
            return ProcessSrgsElement(item.Elements[0], grammar, doc);
        }

        var pn = new ProductionNode();
        //perhaps unneded (?)
        string s = "";
        foreach (var e in item.Elements)
        {
            if (e is SrgsText)
            {
                s += (e as SrgsText).Text;
            }
            else if (e is SrgsRuleRef)
            {
                s += (e as SrgsRuleRef).Uri.ToString().Trim('#');
            }
        }

        if (!grammar.Productions.ContainsKey(s))
        {
            foreach (var e in item.Elements)
            {
                pn.AppendNode(ProcessSrgsElement(e, grammar, doc));
            }
        }

        return pn;
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
            var r = new ProductionNode();
            grammar.Productions.Add(rule.Id, r);
            foreach (var e in rule.Elements)
            {
                r.AppendNode(ProcessSrgsElement(e, grammar, doc));
            }
        }

        return grammar.Productions[rule.Id];
    }

}

